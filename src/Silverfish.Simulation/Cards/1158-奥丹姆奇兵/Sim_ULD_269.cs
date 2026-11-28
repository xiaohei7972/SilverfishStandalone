using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 卑劣的回收者
// Wretched Reclaimer
// 战吼：消灭一个友方随从，然后将其复活，并具有所有生命值。
// Battlecry: Destroy a friendly minion, then return it to life with full Health.
internal static class Sim_ULD_269
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
