using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 虚灵改装师
// Ethereal Augmerchant
// 战吼：对一个随从造成1点伤害，并使其获得法术伤害+1。
// Battlecry: Deal 1 damage to a minion and give it Spell Damage +1.
internal static class Sim_BT_724
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
