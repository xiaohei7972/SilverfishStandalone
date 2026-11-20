using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    private static PendingEntityChoice ChoicePacket(int min = 2, int max = 3, int[]? excluded = null) =>
        new(new EntityChoicePacket(123, EntityChoiceType.Target, First, FirstHero, min, max,
            [new EntityId(11), new EntityId(22), new EntityId(33)],
            (excluded ?? []).Select(id => new EntityId(id)).ToImmutableArray()));

    private static GameState ChoiceGame(GameSimulator simulator, PendingEntityChoice pending) =>
        simulator.CreateGame(
            [Player(First, FirstHero), Player(Second, SecondHero)],
            [InPlay(10, "hero", First, 1, 0, 30), InPlay(20, "hero", Second, 2, 0, 30)],
            First, pendingChoice: pending);

    private static SimulationAction Choose(params int[] ids) =>
        new(SimulationActionKind.ChooseOption, First)
        {
            ChoicePacketId = 123, OrderedChoiceTargets = ids.Select(id => new EntityId(id)).ToImmutableArray()
        };

    [Fact]
    public void PendingChoiceBlocksNormalPlayButAcceptsOrderedSubmission()
    {
        var simulator = new GameSimulator([Hero]);
        var original = ChoiceGame(simulator, ChoicePacket());
        Assert.Empty(simulator.LegalActions(original));
        Assert.Throws<InvalidOperationException>(() =>
            simulator.Apply(original, new(SimulationActionKind.EndTurn, First)));

        var result = simulator.Apply(original, Choose(11, 33));
        Assert.Null(original.PendingChoice!.Submitted);
        Assert.NotNull(result.State.PendingChoice!.Submitted);
        Assert.Equal([11, 33], result.State.PendingChoice.Submitted.OrderedEntityIds.Select(id => id.Value));
        Assert.Equal(SimulationEventKind.ChoiceSubmitted, Assert.Single(result.Events).Kind);
        Assert.Equal(123, result.Events[0].ChoiceId);
        Assert.Equal([11, 33], result.Events[0].OrderedChoiceTargets!.Value.Select(id => id.Value));
        Assert.Equal(original.Players, result.State.Players);
        Assert.Equal(original.Entities, result.State.Entities);
        Assert.Equal(original.Turn, result.State.Turn);
        Assert.Empty(simulator.LegalActions(result.State));
        Assert.Throws<InvalidOperationException>(() => simulator.Apply(result.State, Choose(11, 33)));
    }

    [Fact]
    public void RejectedSubmissionsCannotAlterOrSkipThePendingChoice()
    {
        var simulator = new GameSimulator([Hero]);
        var original = ChoiceGame(simulator, ChoicePacket(excluded: [22]));
        Assert.Throws<InvalidOperationException>(() => simulator.Apply(original, Choose(11)));
        Assert.Throws<ArgumentException>(() => simulator.Apply(original, Choose(11, 22)));
        Assert.Throws<ArgumentException>(() => simulator.Apply(original, Choose(11, 11)));
        Assert.Throws<InvalidOperationException>(() =>
            simulator.Apply(original, Choose(11, 33) with { ChoicePacketId = 999 }));
        Assert.Throws<InvalidOperationException>(() =>
            simulator.Apply(original, Choose(11, 33) with { Actor = Second }));
        Assert.Throws<InvalidOperationException>(() =>
            simulator.Apply(original, Choose(11, 33) with { Target = SecondHero }));
        Assert.Null(original.PendingChoice!.Submitted);
    }

    [Fact]
    public void EmptyOptionalChoiceIsSubmittedWithoutPretendingToResolveEffects()
    {
        var simulator = new GameSimulator([Hero]);
        var original = ChoiceGame(simulator, ChoicePacket(min: 0, max: 0));
        var result = simulator.Apply(original, Choose());
        Assert.Empty(result.State.PendingChoice!.Submitted!.OrderedEntityIds);
        Assert.Empty(simulator.LegalActions(result.State));
        Assert.Equal(First, result.State.ActivePlayer);
    }

    [Fact]
    public void RestoringSubmittedChoiceRetainsOrderedIdsAndBlocksRepeatedPlay()
    {
        var simulator = new GameSimulator([Hero]);
        var root = ChoiceGame(simulator, ChoicePacket());
        var submitted = simulator.Apply(root, Choose(33, 11)).State;
        var continuation = new GameContinuation(submitted.NextEntity, submitted.NextPlayOrder,
            submitted.Outcome, submitted.Winner);
        var restored = simulator.RestoreGame(submitted.Players.Values, submitted.Entities.Values,
            submitted.ActivePlayer, submitted.Turn, submitted.MinionDeaths, submitted.RandomState,
            continuation, submitted.PendingChoice);
        Assert.Equal([33, 11], restored.PendingChoice!.Submitted!.OrderedEntityIds.Select(id => id.Value));
        Assert.Empty(simulator.LegalActions(restored));
        Assert.Throws<InvalidOperationException>(() =>
            simulator.Apply(restored, new(SimulationActionKind.EndTurn, First)));
    }

    [Fact]
    public void ImportRejectsMalformedSubmittedChoices()
    {
        var simulator = new GameSimulator([Hero]);
        var original = ChoicePacket();
        var duplicate = original with
        {
            Submitted = new EntityChoiceSubmission(123, [new EntityId(11), new EntityId(11)])
        };
        Assert.Throws<ArgumentException>(() => ChoiceGame(simulator, duplicate));
        var wrongId = original with
        {
            Submitted = new EntityChoiceSubmission(124, [new EntityId(11), new EntityId(33)])
        };
        Assert.Throws<InvalidOperationException>(() => ChoiceGame(simulator, wrongId));
    }

    [Fact]
    public void ApplyDistributionForChoiceIsDeterministicOnlyForTheAcknowledgement()
    {
        var simulator = new GameSimulator([Hero]);
        var root = ChoiceGame(simulator, ChoicePacket());
        var distribution = simulator.ApplyDistribution(root, Choose(33, 11));
        Assert.True(distribution.IsExact);
        var outcome = Assert.Single(distribution.Outcomes);
        Assert.Equal(1.0, outcome.Probability);
        Assert.NotNull(outcome.Result.State.PendingChoice!.Submitted);
        Assert.Empty(simulator.LegalActions(outcome.Result.State));
    }
}
