using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 邪脊吞噬者
// Vilespine Slayer
// 连击： 消灭一个随从。
// Combo: Destroy a minion.
internal static class Sim_UNG_064
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_FOR_COMBO), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
