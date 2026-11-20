using System.Collections.Immutable;
using Silverfish.Core;
using Silverfish.Domain;
using Xunit;

namespace Silverfish.Tests;

public sealed class UiChoiceInputPlannerTests
{
    private static PendingEntityChoice Packet(int countMin = 2, int countMax = 3) =>
        new(new EntityChoicePacket(
            123, EntityChoiceType.Target, new PlayerId(1), new EntityId(99), countMin, countMax,
            [new EntityId(11), new EntityId(22), new EntityId(33)],
            ImmutableArray<EntityId>.Empty));

    private static SimulationAction Choose(params int[] targets) =>
        new(SimulationActionKind.ChooseOption, new PlayerId(1))
        {
            ChoicePacketId = 123,
            OrderedChoiceTargets = targets.Select(v => new EntityId(v)).ToImmutableArray()
        };

    [Fact]
    public void PlansVisibleClicksInExactSelectionOrderWithoutProtocolCalls()
    {
        var pending = Packet();
        var plan = UiChoiceInputPlanner.Plan(pending, Choose(33, 11, 22), "match-4", 91);

        Assert.Equal("match-4", plan.MatchId);
        Assert.Equal(91, plan.SnapshotSequence);
        Assert.Equal(123, plan.ChoicePacketId);
        Assert.Equal([
            UiChoiceStepKind.ClickEntity, UiChoiceStepKind.ClickEntity, UiChoiceStepKind.ClickEntity,
            UiChoiceStepKind.ConfirmIfVisible, UiChoiceStepKind.AwaitFreshSnapshot
        ], plan.Steps.Select(s => s.Kind));
        Assert.Equal([33, 11, 22], plan.Steps.Where(s => s.Kind == UiChoiceStepKind.ClickEntity)
            .Select(s => s.Entity!.Value.Value));
        Assert.Null(pending.Submitted);
    }

    [Fact]
    public void OneTargetStillRequiresAnObservedUICheckInsteadOfAssumingAutoSubmit()
    {
        var plan = UiChoiceInputPlanner.Plan(Packet(1, 1), Choose(22), "match-4", 0);
        Assert.Equal([
            UiChoiceStepKind.ClickEntity, UiChoiceStepKind.ConfirmIfVisible, UiChoiceStepKind.AwaitFreshSnapshot
        ], plan.Steps.Select(s => s.Kind));
    }

    [Fact]
    public void OptionalZeroTargetSelectionDoesNotProducePhantomClicks()
    {
        var plan = UiChoiceInputPlanner.Plan(Packet(0, 2), Choose(), "match-4", 0);
        Assert.Equal([UiChoiceStepKind.ConfirmIfVisible, UiChoiceStepKind.AwaitFreshSnapshot],
            plan.Steps.Select(s => s.Kind));
    }

    [Fact]
    public void RejectsStaleOrIllegalPlansBeforeAnyUiActions()
    {
        var pending = Packet();
        Assert.Throws<InvalidOperationException>(() =>
            UiChoiceInputPlanner.Plan(pending, Choose(11), "match", 0));
        Assert.Throws<InvalidOperationException>(() =>
            UiChoiceInputPlanner.Plan(pending, Choose(11, 22) with { ChoicePacketId = 9 }, "match", 0));
        Assert.Throws<ArgumentException>(() =>
            UiChoiceInputPlanner.Plan(pending, Choose(11, 11), "match", 0));
        Assert.Throws<ArgumentException>(() =>
            UiChoiceInputPlanner.Plan(pending, Choose(11, 22), "", 0));
        Assert.Throws<InvalidOperationException>(() =>
            UiChoiceInputPlanner.Plan(pending.Submit(123, [new EntityId(11), new EntityId(22)]),
                Choose(11, 22), "match", 0));
    }
}
