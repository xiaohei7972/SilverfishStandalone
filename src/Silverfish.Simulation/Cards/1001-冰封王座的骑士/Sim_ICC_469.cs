using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 强制牺牲
// Unwilling Sacrifice
// 选择一个友方随从，消灭该随从和一个随机敌方随从。
// Choose a friendly minion. Destroy it and a random enemy minion.
internal static class Sim_ICC_469
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
