using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 莫克纳萨将狮
// Mok'Nathal Lion
// 突袭，战吼：选择一个友方随从，获得其亡语的复制。
// Rush. Battlecry: Choose a friendly minion. Gain a copy of its Deathrattle.
internal static class Sim_BT_212
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
