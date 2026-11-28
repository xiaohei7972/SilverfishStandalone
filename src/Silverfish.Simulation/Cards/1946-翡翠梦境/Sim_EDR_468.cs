using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 捣蛋狂魔
// Eggbasher
// 战吼：对一个随从造成1点伤害，并使其获得+4攻击力。
// Battlecry: Deal 1 damage to a minion and give it +4 Attack.
internal static class Sim_EDR_468
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
