using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 防护改装师
// Guardian Augmerchant
// 战吼：对一个随从造成1点伤害，并使其获得圣盾。
// Battlecry: Deal 1 damage to a minion and give it Divine Shield.
internal static class Sim_BT_722
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
