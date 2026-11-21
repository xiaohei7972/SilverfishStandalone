using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 怒火中烧
// Inner Rage
// 对一个随从造成$1点伤害，并使其获得+2攻击力。
// Deal $1 damage to a minion and give it +2_Attack.
internal static class Sim_VAN_EX1_607
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
