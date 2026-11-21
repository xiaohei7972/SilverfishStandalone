using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 铁喙猫头鹰
// Ironbeak Owl
// 战吼： 沉默一个随从。
// Battlecry: Silence a_minion.
internal static class Sim_VAN_CS2_203
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
