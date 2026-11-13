using Silverfish.Contracts;

namespace Silverfish.Core;

/// <summary>
/// Small deterministic pipeline smoke-test engine. This is NOT Silverfish's
/// original playfield simulation, search, or card evaluator.
/// </summary>
public sealed class BaselineStrategyEngine : IStrategyEngine
{
    public string Name => "baseline-v1-not-silverfish";

    public Task<BotDecision> DecideAsync(
        GameSnapshot snapshot,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SnapshotValidator.Validate(snapshot);

        if (!snapshot.IsOurTurn || snapshot.Phase != GamePhase.Main)
            return Task.FromResult(NoDecision(snapshot, "No main-phase decision is required."));

        if (snapshot.LegalActions.Count == 0)
            return Task.FromResult(NoDecision(snapshot, "No supplied legal action candidates."));

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
            Reason = "Baseline fixture scoring only; not a Silverfish search result.",
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
            GameActionKind.HeroPower => 12,
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

            // Simple direct-damage approximation. It does not model immunity,
            // secrets, prevention, reactive triggers or other game mechanics.
            return (attack > 0 && attack >= effectiveHealth ? 10000 : 0) + 50 + attack * 10;
        }

        var defender = snapshot.EnemyBoard.FirstOrDefault(m => m.EntityId == action.TargetEntityId);
        if (defender is null)
            return -500;

        var tradeValue = Math.Min(Math.Max(attack, 0), Math.Max(defender.Health, 0)) * 8;
        return 40 + tradeValue + (defender.HasTaunt ? 30 : 0);
    }
}
