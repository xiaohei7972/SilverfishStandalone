using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 盾牌猛击
// Shield Slam
// 你每有1点护甲值，便对一个随从造成1点伤害。
// Deal 1 damage to a minion for each Armor you have.
internal static class Sim_VAN_EX1_410
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
