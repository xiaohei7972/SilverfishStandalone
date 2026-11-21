using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 转生
// Reincarnate
// 消灭一个非传说随从，然后将其复活，并具有所有生命值。
// Destroy a non-Legendary minion, then return it to life with full Health.
internal static class Sim_FP1_025
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
