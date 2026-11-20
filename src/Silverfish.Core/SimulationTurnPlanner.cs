using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Simulation;

namespace Silverfish.Core;

public sealed record TurnPlan(ImmutableArray<SimulationAction> Actions, double Score,
    MatchOutcome Outcome, PlayerId? Winner, int Transitions)
{
    public double WinProbability { get; init; }
    public double LossProbability { get; init; }
    public bool ReplanAfterLastAction { get; init; }
    public bool UsesSamples { get; init; }
}

/// <summary>在给定预算内搜索当前回合，并按随机结果分别计算后续动作。</summary>
public sealed class SimulationTurnPlanner(GameSimulator simulator, Func<GameState, PlayerId, double> evaluate)
{
    private const int ResolutionWorkPerAction = 64;

    public TurnPlan Plan(GameState root, int depth = 5, int beamWidth = 16, int transitionLimit = 512) =>
        Plan(root, initialActions: null, depth, beamWidth, transitionLimit);

    internal TurnPlan Plan(GameSimulator.LegalActionSet initialActions) =>
        Plan(initialActions.State, initialActions);

    private TurnPlan Plan(GameState root, GameSimulator.LegalActionSet? initialActions,
        int depth = 5, int beamWidth = 16, int transitionLimit = 512)
    {
        if (depth < 1 || beamWidth < 1 || transitionLimit < 1)
            throw new ArgumentOutOfRangeException(nameof(depth), "搜索深度、保留分支数和结算次数上限必须大于零。");

        var perspective = root.ActivePlayer;
        var transitions = 0;
        var best = Search(root, depth, transitionLimit, mayStop: false);
        PlayerId? winner = best.Guarantee == GuaranteedResult.Win ? perspective :
            best.Guarantee == GuaranteedResult.Loss ? root.OpponentOf(perspective) : null;
        var outcome = winner is not null ? MatchOutcome.Won :
            best.Guarantee == GuaranteedResult.Draw ? MatchOutcome.Draw : MatchOutcome.InProgress;
        return new TurnPlan(best.Actions, best.Score, outcome, winner, transitions)
        {
            WinProbability = Math.Clamp(best.Win, 0, 1), LossProbability = Math.Clamp(best.Loss, 0, 1),
            ReplanAfterLastAction = best.Replan, UsesSamples = best.Sampled
        };

        Prediction Search(GameState state, int remainingDepth, int limit, bool mayStop = true)
        {
            var baseline = Evaluate(state);
            if (remainingDepth == 0 || transitions >= limit || state.Outcome != MatchOutcome.InProgress || state.ActivePlayer != perspective)
                return baseline;

            Prediction? selected = mayStop ? baseline : null;
            var legal = mayStop || initialActions is null ? simulator.GetLegalActionSet(state) : initialActions;
            var actions = legal.Actions;
            var candidates = new List<Candidate>();
            for (var index = 0; index < actions.Length && transitions < limit; index++)
            {
                var action = actions[index];
                var otherActions = actions.Length - index - 1;
                var budget = Math.Min(ResolutionWorkPerAction, Math.Max(1, limit - transitions - otherActions));
                var distribution = legal.ApplyDistribution(index, budget);
                transitions += distribution.Resolutions;
                var immediate = Forecast(action, distribution, 0, transitions);
                if (selected is null || Rank(immediate).CompareTo(Rank(selected)) > 0)
                    selected = immediate;
                if (immediate.Guarantee == GuaranteedResult.Win)
                    return immediate;
                candidates.Add(new(action, distribution, immediate));
            }

            if (remainingDepth <= 1)
                return selected ?? baseline;

            var promising = candidates.OrderByDescending(candidate => Rank(candidate.Immediate)).Take(beamWidth).ToArray();
            for (var index = 0; index < promising.Length && transitions < limit; index++)
            {
                var candidate = promising[index];
                var share = (limit - transitions) / (promising.Length - index);
                if (share == 0)
                    continue;
                var prediction = Forecast(candidate.Action, candidate.Distribution, remainingDepth - 1, transitions + share);
                if (selected is null || Rank(prediction).CompareTo(Rank(selected)) > 0)
                    selected = prediction;
            }
            return selected ?? baseline;
        }

        Prediction Forecast(SimulationAction action, SimulationDistribution distribution, int remainingDepth, int limit)
        {
            if (distribution.IsExact && distribution.Outcomes is [var only])
            {
                var after = Search(only.Result.State, remainingDepth, limit);
                return after with { Actions = after.Actions.Insert(0, action) };
            }

            var score = 0.0;
            var win = 0.0;
            var loss = 0.0;
            var sampled = !distribution.IsExact;
            var guarantee = GuaranteedResult.None;
            for (var index = 0; index < distribution.Outcomes.Length; index++)
            {
                var outcome = distribution.Outcomes[index];
                var share = (limit - transitions) / (distribution.Outcomes.Length - index);
                var branch = Search(outcome.Result.State, remainingDepth, transitions + share);
                score += outcome.Probability * branch.Score;
                win += outcome.Probability * branch.Win;
                loss += outcome.Probability * branch.Loss;
                sampled |= branch.Sampled;
                if (index == 0)
                    guarantee = branch.Guarantee;
                else if (branch.Guarantee != guarantee)
                    guarantee = GuaranteedResult.None;
            }
            return new([action], score, win, loss, sampled ? GuaranteedResult.None : guarantee, Replan: true, Sampled: sampled);
        }

        Prediction Evaluate(GameState state)
        {
            var guarantee = state.Outcome switch
            {
                MatchOutcome.Won => state.Winner == perspective ? GuaranteedResult.Win : GuaranteedResult.Loss,
                MatchOutcome.Draw => GuaranteedResult.Draw,
                _ => GuaranteedResult.None
            };
            return new([], evaluate(state, perspective), guarantee == GuaranteedResult.Win ? 1 : 0,
                guarantee == GuaranteedResult.Loss ? 1 : 0, guarantee);
        }
    }

    private static (int Guaranteed, double NetWins, double Score, int Brevity) Rank(Prediction prediction) =>
        (prediction.Guarantee == GuaranteedResult.Win ? 1 : prediction.Guarantee == GuaranteedResult.Loss ? -1 : 0,
            prediction.Win - prediction.Loss, prediction.Score, -prediction.Actions.Length);

    private enum GuaranteedResult { None, Win, Loss, Draw }

    private sealed record Prediction(ImmutableArray<SimulationAction> Actions, double Score,
        double Win, double Loss, GuaranteedResult Guarantee, bool Replan = false, bool Sampled = false);

    private sealed record Candidate(SimulationAction Action, SimulationDistribution Distribution, Prediction Immediate);
}

public static class SimulationMaterialEvaluator
{
    public static double Evaluate(GameState state, PlayerId perspective) =>
        PlayerScore(state, perspective) - PlayerScore(state, state.OpponentOf(perspective));

    private static double PlayerScore(GameState state, PlayerId id)
    {
        var player = state.Players[id];
        var score = 0.25 * player.In(GameZone.Hand).Length;
        if (player.EquippedWeapon is EntityId weaponId && state.Entities[weaponId].Weapon is { } weapon)
            score += 1.5 * weapon.Attack * weapon.Durability;
        foreach (var entity in player.In(GameZone.Play))
        {
            if (state.Entities[entity].Character is not { } character)
                continue;
            score += entity == player.Hero
                ? 3.0 * (character.Health + (double)character.Armor)
                : character.Health + 2.0 * character.Attack + (character.DivineShield ? 0.5 : 0);
        }
        return score;
    }
}
