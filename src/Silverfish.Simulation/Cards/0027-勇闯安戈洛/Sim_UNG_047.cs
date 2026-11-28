using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 饥饿的翼手龙
// Ravenous Pterrordax
// 战吼： 消灭一个友方随从，并连续进化两次。
// Battlecry: Destroy a friendly minion to Adapt_twice.
internal static class Sim_UNG_047
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
