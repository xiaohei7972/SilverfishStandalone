using Silverfish.Contracts;
using Silverfish.Domain;
using Silverfish.Simulation;

namespace Silverfish.Core;

/// <summary>验证调用方的候选动作，用独立模拟器搜索首步，并返回调用方原有的动作 ID。</summary>
public sealed class SimulationDecisionEngine(GameSimulator simulator)
{
    private const string EngineName = "simulation-v1";

    public BotDecision Decide(GameState state, string matchId, long snapshotSequence, IReadOnlyList<CandidateAction> candidates)
    {
        if (string.IsNullOrWhiteSpace(matchId) || snapshotSequence < 0)
            throw new ArgumentException("决策请求必须提供对局 ID 和非负快照序号。");
        if (candidates is null)
            throw new ArgumentNullException(nameof(candidates), "必须提供候选动作列表。");

        var legalActions = simulator.GetLegalActionSet(state);
        var legal = legalActions.Actions.ToHashSet();
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var byAction = new Dictionary<SimulationAction, CandidateAction>();
        foreach (var candidate in candidates)
        {
            if (candidate is null || string.IsNullOrWhiteSpace(candidate.Id) || candidate.ManaCost < 0)
                throw new ArgumentException("候选动作必须提供 ID，且法力费用不能为负数。", nameof(candidates));
            if (!ids.Add(candidate.Id))
                throw new ArgumentException($"候选动作 ID「{candidate.Id}」重复。", nameof(candidates));

            var action = ToSimulationAction(candidate, state.ActivePlayer);
            if (!legal.Contains(action))
                throw new ArgumentException($"候选动作「{candidate.Id}」在当前对局状态下不合法。", nameof(candidates));
            if (!byAction.TryAdd(action, candidate))
                throw new ArgumentException($"候选动作「{candidate.Id}」与其他候选项表示同一个动作。", nameof(candidates));
        }

        if (byAction.Count == 0)
            return new BotDecision
            {
                MatchId = matchId, SnapshotSequence = snapshotSequence, Engine = EngineName,
                Reason = "调用方未提供可选动作。"
            };

        var rootActions = legalActions.Filter(byAction.ContainsKey);
        var plan = new SimulationTurnPlanner(simulator, SimulationMaterialEvaluator.Evaluate).Plan(rootActions);
        return new BotDecision
        {
            MatchId = matchId, SnapshotSequence = snapshotSequence, Engine = EngineName,
            ChosenAction = byAction[plan.Actions[0]],
            Score = plan.Score,
            Reason = "已根据当前对局状态，在调用方提供的候选动作中搜索首步。"
        };
    }

    private static SimulationAction ToSimulationAction(CandidateAction candidate, PlayerId actor)
    {
        var kind = candidate.Kind switch
        {
            GameActionKind.EndTurn => SimulationActionKind.EndTurn,
            GameActionKind.PlayCard => SimulationActionKind.PlayCard,
            GameActionKind.Attack => SimulationActionKind.Attack,
            GameActionKind.HeroPower => SimulationActionKind.UseHeroPower,
            GameActionKind.ActivateLocation => SimulationActionKind.ActivateLocation,
            _ => throw new ArgumentException($"候选动作「{candidate.Id}」的类型「{candidate.Kind}」不受支持。", nameof(candidate))
        };
        EntityId? source = candidate.SourceEntityId is int from ? new(from) : null;
        EntityId? target = candidate.TargetEntityId is int to ? new(to) : null;
        return new(kind, actor, source, target, candidate.Position) { Choice = candidate.Choice };
    }
}
