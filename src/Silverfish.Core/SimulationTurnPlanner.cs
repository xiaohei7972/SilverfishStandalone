using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Simulation;

namespace Silverfish.Core;

public sealed record TurnPlan(ImmutableArray<SimulationAction> Actions, double Score,
    MatchOutcome Outcome, PlayerId? Winner, int Transitions);

/// <summary>Bounded search over the current turn of a fully specified offline simulation.</summary>
public sealed class SimulationTurnPlanner(GameSimulator simulator, Func<GameState, PlayerId, double> evaluate)
{
    public TurnPlan Plan(GameState root, int depth = 5, int beamWidth = 16, int transitionLimit = 512)
    {
        if (depth < 1 || beamWidth < 1 || transitionLimit < 1)
            throw new ArgumentOutOfRangeException(nameof(depth), "Search limits must be positive.");

        var perspective = root.ActivePlayer;
        var initial = new Node(root, [], evaluate(root, perspective));
        var frontier = new List<Node> { initial };
        Node? best = null;
        var transitions = 0;

        for (var ply = 0; ply < depth && frontier.Count > 0 && transitions < transitionLimit; ply++)
        {
            var next = new List<Node>();
            foreach (var node in frontier)
            {
                foreach (var action in simulator.LegalActions(node.State))
                {
                    if (transitions == transitionLimit)
                        break;

                    var state = simulator.Apply(node.State, action).State;
                    var child = new Node(state, node.Actions.Add(action), evaluate(state, perspective));
                    transitions++;
                    if (best is null || Rank(child, perspective).CompareTo(Rank(best, perspective)) > 0)
                        best = child;
                    if (state.Outcome == MatchOutcome.Won && state.Winner == perspective)
                        return Result(child);
                    if (state.Outcome == MatchOutcome.InProgress && state.ActivePlayer == perspective)
                        next.Add(child);
                }
            }

            frontier = next.OrderByDescending(node => Rank(node, perspective)).Take(beamWidth).ToList();
        }
        return Result(best ?? initial);

        TurnPlan Result(Node node) => new(node.Actions, node.Score, node.State.Outcome, node.State.Winner, transitions);
    }

    private static (int Outcome, double Score, int Brevity) Rank(Node node, PlayerId perspective) =>
        (node.State.Outcome == MatchOutcome.Won ? node.State.Winner == perspective ? 1 : -1 : 0,
            node.Score, -node.Actions.Length);

    private sealed record Node(GameState State, ImmutableArray<SimulationAction> Actions, double Score);
}

public static class SimulationMaterialEvaluator
{
    public static double Evaluate(GameState state, PlayerId perspective) =>
        PlayerScore(state, perspective) - PlayerScore(state, state.OpponentOf(perspective));

    private static double PlayerScore(GameState state, PlayerId id)
    {
        var player = state.Players[id];
        var score = 0.25 * player.In(GameZone.Hand).Length;
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
