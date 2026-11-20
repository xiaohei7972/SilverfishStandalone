using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Core;

/// <summary>
/// Semantic instructions for a *game-internal* virtual input source. The
/// in-client adapter should feed IInput / InputCollection (like Ember's
/// VirtualInput and VirtualPointer), not move the Windows desktop cursor.
/// Never call game network methods to submit the selected option.
/// </summary>
public enum UiChoiceStepKind
{
    ClickEntity,
    ConfirmIfVisible,
    AwaitFreshSnapshot
}

public sealed record UiChoiceStep(UiChoiceStepKind Kind, EntityId? Entity = null);

/// <summary>
/// The host must validate MatchId, SnapshotSequence and ChoicePacketId before
/// and during execution. Each ClickEntity is resolved inside the Unity
/// client to the current Card/Actor's hit area, using its active camera.
/// No fixed screen coordinates or foreground-window requirement are embedded.
/// </summary>
public sealed record UiChoiceInputPlan(
    string MatchId,
    long SnapshotSequence,
    int ChoicePacketId,
    ImmutableArray<UiChoiceStep> Steps);

/// <summary>
/// Converts a validated CHOICE intent into game-side virtual pointer steps.
/// The in-client host owns a frame-driven IInput source and must observe
/// OptionsId/ChoiceId changes before reporting success. This class neither
/// touches Windows mouse APIs nor requires foreground focus.
/// </summary>
public static class UiChoiceInputPlanner
{
    public static UiChoiceInputPlan Plan(
        PendingEntityChoice pending, SimulationAction choiceAction,
        string matchId, long snapshotSequence)
    {
        ArgumentNullException.ThrowIfNull(pending);
        ArgumentNullException.ThrowIfNull(choiceAction);
        if (string.IsNullOrWhiteSpace(matchId) || snapshotSequence < 0)
            throw new ArgumentException("输入计划需要有效对局 ID 和快照序号。");
        pending.Validate();
        if (pending.Submitted is not null)
            throw new InvalidOperationException("选择已在本地记录，必须等待新的界面状态，不能重复操作。");

        if (choiceAction.Kind != SimulationActionKind.ChooseOption ||
            choiceAction.Actor != pending.Packet.Player ||
            choiceAction.ChoicePacketId != pending.Packet.ChoiceId ||
            choiceAction.Source is not null || choiceAction.Target is not null ||
            choiceAction.Position is not null || choiceAction.Choice is not null)
            throw new InvalidOperationException("选择动作与当前 CHOICE 包不一致。");

        // Validate without applying card effects or changing GameState.
        // The returned submission is a local selection intent, not an API call.
        _ = pending.Submit(pending.Packet.ChoiceId, choiceAction.OrderedChoiceTargets);

        var steps = ImmutableArray.CreateBuilder<UiChoiceStep>();
        foreach (var id in choiceAction.OrderedChoiceTargets)
            steps.Add(new(UiChoiceStepKind.ClickEntity, id));

        // DoNetworkChoice has additional UI conditions beyond IsSingleChoice.
        // The executor must detect the actual confirmation button; this step
        // does NOT assume it is present or call Network.SendChoices().
        steps.Add(new(UiChoiceStepKind.ConfirmIfVisible));
        steps.Add(new(UiChoiceStepKind.AwaitFreshSnapshot));
        return new(matchId, snapshotSequence, pending.Packet.ChoiceId, steps.ToImmutable());
    }
}
