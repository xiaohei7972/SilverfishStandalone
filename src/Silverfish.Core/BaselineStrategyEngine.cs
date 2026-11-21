using Silverfish.Contracts;

namespace Silverfish.Core;

/// <summary>按固定规则为输入的候选动作打分，用于验证快照决策接口。</summary>
public sealed class BaselineStrategyEngine : IStrategyEngine
{
    public string Name => "baseline-v1-not-silverfish";

    public Task<BotDecision> DecideAsync(
        GameSnapshot snapshot,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("快照决策已取消。", cancellationToken);
        SnapshotValidator.Validate(snapshot);

        if (!snapshot.IsOurTurn || snapshot.Phase != GamePhase.Main)
            return Task.FromResult(NoDecision(snapshot, "当前阶段无需选择动作。"));

        if (snapshot.LegalActions.Count == 0)
            return Task.FromResult(NoDecision(snapshot, "快照未提供可选动作。"));

        var ranking = snapshot.LegalActions
            .Select(action => new ScoredAction
            {
                Action = action,
                Score = Evaluate(snapshot, action)
            })
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Action.Id, StringComparer.Ordinal)
            .ToArray();

        var best = ranking[0];
        return Task.FromResult(new BotDecision
        {
            MatchId = snapshot.MatchId,
            SnapshotSequence = snapshot.Sequence,
            Engine = Name,
            ChosenAction = best.Action,
            Score = best.Score,
            Reason = "根据快照中的候选动作进行基线评分。",
            Alternatives = ranking.Take(5).ToArray()
        });
    }

    private BotDecision NoDecision(GameSnapshot snapshot, string reason) => new()
    {
        MatchId = snapshot.MatchId,
        SnapshotSequence = snapshot.Sequence,
        Engine = Name,
        Reason = reason
    };

    private static double Evaluate(GameSnapshot snapshot, CandidateAction action)
    {
        return action.Kind switch
        {
            GameActionKind.EndTurn => -1000,
            GameActionKind.PlayCard => 20 + Math.Min(action.ManaCost, snapshot.AvailableMana) * 3,
            GameActionKind.HeroPower or GameActionKind.ActivateLocation => 12,
            GameActionKind.Attack => EvaluateAttack(snapshot, action),
            GameActionKind.ChooseOption => 0,
            _ => -2000
        };
    }

    private static double EvaluateAttack(GameSnapshot snapshot, CandidateAction action)
    {
        var attacker = snapshot.OurBoard.FirstOrDefault(m => m.EntityId == action.SourceEntityId);
        var attack = attacker?.Attack ??
            (action.SourceEntityId == snapshot.OurHero.EntityId ? snapshot.OurHero.Attack : 0);

        if (action.TargetEntityId == snapshot.EnemyHero.EntityId)
        {
            var effectiveHealth = Math.Max(0, snapshot.EnemyHero.Health) +
                                  Math.Max(0, snapshot.EnemyHero.Armor);

            // 基线只按当前攻击力与英雄的生命和护甲估算直接伤害。
            return (attack > 0 && attack >= effectiveHealth ? 10000 : 0) + 50 + attack * 10;
        }

        var defender = snapshot.EnemyBoard.FirstOrDefault(m => m.EntityId == action.TargetEntityId);
        if (defender is null)
            return -500;

        var tradeValue = Math.Min(Math.Max(attack, 0), Math.Max(defender.Health, 0)) * 8;
        return 40 + tradeValue + (defender.HasTaunt ? 30 : 0);
    }
}
