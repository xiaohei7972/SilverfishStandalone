using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Simulation;

namespace Silverfish.Core;

/// <summary>Offline forecast only: never sends a choice or guesses unknown card effects.</summary>
public sealed record EntityChoiceForecast(
    SimulationAction? Action, double Score, int EvaluatedCandidates, bool IsComplete, string Reason);

/// <summary>
/// Exhaustively compares ordered CHOICE submissions using explicitly supplied
/// effect rules and the existing simulator's exact random distribution engine.
/// If any candidate cannot be evaluated within budget, abstains rather than
/// ranking a partial set of options.
/// </summary>
public sealed class EntityChoiceForecastPlanner(
    GameSimulator simulator, Func<GameState, PlayerId, double> evaluate)
{
    public EntityChoiceForecast Plan(GameState state, ChoicePreviewRules knownRules,
        int candidateLimit = 256, int distributionLimit = 64)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(knownRules);
        ArgumentNullException.ThrowIfNull(simulator);
        ArgumentNullException.ThrowIfNull(evaluate);
        if (candidateLimit < 1 || distributionLimit < 1)
            throw new ArgumentOutOfRangeException(nameof(candidateLimit), "候选数和分布计算预算必须大于零。");

        var packet = state.PendingChoice?.Packet;
        if (packet is null || state.PendingChoice!.Submitted is not null ||
            state.Outcome != MatchOutcome.InProgress)
            return new(null, 0, 0, false, "没有尚未提交的 CHOICE 包，无法进行离线选择预演。");
        if (knownRules.ChoiceId != packet.ChoiceId || knownRules.Type != packet.Type)
            return new(null, 0, 0, false, "离线规则和当前选择包不匹配；拒绝猜测效果。");

        var available = packet.Entities.Except(packet.UnchoosableEntities).ToArray();
        var chosen = new List<EntityId>();
        var count = 0;
        var complete = true;
        var bestScore = double.NegativeInfinity;
        SimulationAction? best = null;

        Explore();

        if (!complete)
            return new(null, 0, count, false, "候选组合或随机分支超出预算；不使用不完整的选择排名。");
        if (best is null)
            return new(null, 0, count, false, "没有可依据已知规则估值的选择。");
        return new(best, bestScore, count, true,
            "已比较所有合法有序选择；该结果是基于已知规则的离线预测，真实提交仍需新快照确认。");

        void Explore()
        {
            if (!complete)
                return;

            if (chosen.Count >= packet.CountMin)
            {
                if (count == candidateLimit)
                {
                    complete = false;
                    return;
                }
                var action = new SimulationAction(SimulationActionKind.ChooseOption, packet.Player)
                {
                    ChoicePacketId = packet.ChoiceId,
                    OrderedChoiceTargets = chosen.ToImmutableArray()
                };
                var distribution = simulator.PreviewChoiceDistribution(
                    state, action, knownRules, distributionLimit);
                count++;
                if (!distribution.IsExact || distribution.Outcomes.IsEmpty)
                {
                    complete = false;
                    return;
                }

                var expected = 0.0;
                foreach (var outcome in distribution.Outcomes)
                {
                    var next = outcome.Result.State;
                    var score = next.Outcome switch
                    {
                        MatchOutcome.Won => next.Winner == packet.Player ? 1e9 : -1e9,
                        MatchOutcome.Draw => 0,
                        _ => evaluate(next, packet.Player)
                    };
                    if (!double.IsFinite(score))
                        throw new InvalidOperationException("评估函数必须返回有限分数。");
                    expected += outcome.Probability * score;
                }

                if (best is null || expected > bestScore)
                {
                    best = action;
                    bestScore = expected;
                }
            }

            if (chosen.Count == packet.CountMax)
                return;
            foreach (var id in available)
            {
                if (chosen.Contains(id))
                    continue;
                chosen.Add(id);
                Explore();
                chosen.RemoveAt(chosen.Count - 1);
                if (!complete)
                    return;
            }
        }
    }
}
