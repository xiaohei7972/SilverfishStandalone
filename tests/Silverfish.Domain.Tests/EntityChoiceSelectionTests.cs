using System.Collections.Immutable;
using Silverfish.Domain;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed class EntityChoiceSelectionTests
{
    private static EntityChoicePacket Packet(int min, int max, EntityChoiceType type = EntityChoiceType.Target,
        int[]? entries = null, int[]? excluded = null) =>
        new(123, type, new PlayerId(1), new EntityId(99), min, max,
            (entries ?? [11, 22, 33]).Select(id => new EntityId(id)).ToImmutableArray(),
            (excluded ?? []).Select(id => new EntityId(id)).ToImmutableArray());

    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(1, 1, true)]
    [InlineData(1, 2, false)]
    [InlineData(2, 2, false)]
    [InlineData(0, 2, false)]
    public void IsSingleChoiceMatchesLegacyClientBranch(int min, int max, bool expected)
    {
        Assert.Equal(expected, Packet(min, max).IsSingleChoice());
    }

    [Fact]
    public void OrderedClickRemovalAndReselectionPreserveSequenceAndImmutability()
    {
        var empty = EntityChoiceSelection.Begin(Packet(2, 3));
        var first = empty.Click(new(11)).Click(new(22)).Click(new(33));
        var changed = first.Click(new(22)).Click(new(22));

        Assert.Empty(empty.OrderedEntityIds);
        Assert.Equal(new[] { 11, 22, 33 }, first.OrderedEntityIds.Select(id => id.Value));
        Assert.Equal(new[] { 11, 33, 22 }, changed.OrderedEntityIds.Select(id => id.Value));
        Assert.True(changed.CanConfirm);
        Assert.Equal(new[] { 11, 33, 22 }, changed.Confirm(123).OrderedEntityIds.Select(id => id.Value));
    }

    [Fact]
    public void RejectsUnknownExcludedAndExcessTargets()
    {
        var selection = EntityChoiceSelection.Begin(Packet(1, 2, excluded: [22]));
        Assert.Throws<ArgumentException>(() => selection.Click(new(22)));
        Assert.Throws<ArgumentException>(() => selection.Click(new(44)));
        var full = selection.Click(new(11)).Click(new(33));
        Assert.Throws<ArgumentException>(() => full.Click(new(22)));
        var atLimit = EntityChoiceSelection.Begin(Packet(1, 2)).Click(new(11)).Click(new(22));
        Assert.Throws<InvalidOperationException>(() => atLimit.Click(new(33)));
    }

    [Fact]
    public void CannotConfirmBeforeMinimumOrAfterPacketChanges()
    {
        var selection = EntityChoiceSelection.Begin(Packet(2, 3)).Click(new(11));
        Assert.False(selection.CanConfirm);
        Assert.Throws<InvalidOperationException>(() => selection.Confirm(123));
        var complete = selection.Click(new(22));
        Assert.Throws<InvalidOperationException>(() => complete.Confirm(124));
        Assert.True(complete.CanConfirm);
    }

    [Fact]
    public void OptionalZeroSelectionsCanBeConfirmedWithoutAutofiring()
    {
        var optional = EntityChoiceSelection.Begin(Packet(0, 2));
        Assert.True(optional.CanConfirm);
        Assert.False(optional.IsSingleChoiceSubmissionCandidate);
        Assert.Empty(optional.Confirm(123).OrderedEntityIds);

        var zeroMax = EntityChoiceSelection.Begin(Packet(0, 0));
        Assert.True(zeroMax.Packet.IsSingleChoice());
        Assert.False(zeroMax.IsSingleChoiceSubmissionCandidate);
        Assert.Empty(zeroMax.Confirm(123).OrderedEntityIds);
        Assert.Throws<InvalidOperationException>(() => zeroMax.Click(new(11)));
    }

    [Fact]
    public void SingleChoiceReadinessDoesNotDispatchOrConflateModes()
    {
        var packet = Packet(1, 1, EntityChoiceType.General);
        var selection = EntityChoiceSelection.Begin(packet);
        Assert.False(selection.IsSingleChoiceSubmissionCandidate);
        Assert.True(selection.Click(new(11)).IsSingleChoiceSubmissionCandidate);
    }

    [Fact]
    public void RejectsMalformedPacketInsteadOfGuessing()
    {
        Assert.Throws<ArgumentException>(() => EntityChoiceSelection.Begin(Packet(3, 2)));
        Assert.Throws<ArgumentException>(() => EntityChoiceSelection.Begin(Packet(2, 2, excluded: [22, 33])));
        Assert.Throws<ArgumentException>(() => EntityChoiceSelection.Begin(Packet(1, 2, entries: [11, 11])));
        Assert.Throws<ArgumentException>(() => EntityChoiceSelection.Begin(Packet(0, 2, excluded: [44])));
    }
}
