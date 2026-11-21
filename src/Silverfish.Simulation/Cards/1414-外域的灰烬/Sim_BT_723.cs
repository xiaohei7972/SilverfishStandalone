using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火箭改装师
// Rocket Augmerchant
// 战吼：对一个随从造成1点伤害，并使其获得突袭。
// Battlecry: Deal 1 damage to a minion and give it Rush.
internal static class Sim_BT_723
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
