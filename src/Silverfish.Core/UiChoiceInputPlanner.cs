using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Core;

/// <summary>
/// Intentions to operate the *visible game UI* with ordinary input, never
/// protocol packets or calls to the game's network methods.
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
/// and during execution. Each ClickEntity resolves its entity ID to a current
/// on-screen hit target; no fixed screen coordinates are embedded here.
/// </summary>
public sealed record UiChoiceInputPlan(
    string MatchId,
    long SnapshotSequence,
    int ChoicePacketId,
    ImmutableArray<UiChoiceStep> Steps);

/// <summary>
/// Converts a validated CHOICE intent to visual mouse-input steps.
/// The execution adapter owns OS input (e.g. Win32 SendInput) and must observe
/// the actual screen/game response before marking an action complete.
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
