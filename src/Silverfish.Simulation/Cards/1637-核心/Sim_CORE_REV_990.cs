using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 赤红深渊
// Sanguine Depths
// 对一个随从造成1点伤害，并使其获得+2攻击力。
// Deal 1 damage to a minion and give it +2 Attack.
internal static class Sim_CORE_REV_990
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
