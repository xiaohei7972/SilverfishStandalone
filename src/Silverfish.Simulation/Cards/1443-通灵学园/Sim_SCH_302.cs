using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 流光之赐
// Gift of Luminance
// 使一个随从获得圣盾，并召唤一个1/1的复制。
// Give a minion Divine Shield, then summon a_1/1 copy of it.
internal static class Sim_SCH_302
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
