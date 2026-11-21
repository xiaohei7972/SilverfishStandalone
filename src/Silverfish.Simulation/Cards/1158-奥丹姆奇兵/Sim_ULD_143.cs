using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 法老祝福
// Pharaoh's Blessing
// 使一个随从获得+4/+4，圣盾以及嘲讽。
// Give a minion +4/+4, Divine Shield, and Taunt.
internal static class Sim_ULD_143
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
