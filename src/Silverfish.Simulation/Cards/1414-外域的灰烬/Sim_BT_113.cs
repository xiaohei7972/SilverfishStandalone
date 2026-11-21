using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 图腾映像
// Totemic Reflection
// 使一个随从获得+2/+2。如果该随从是图腾，召唤一个它的复制。
// Give a minion +2/+2. If it's a Totem, summon a copy of it.
internal static class Sim_BT_113
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
