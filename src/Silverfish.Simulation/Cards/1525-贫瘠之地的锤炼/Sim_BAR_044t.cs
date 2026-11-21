using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 闪电链（等级2）
// Chain Lightning (Rank 2)
// 对一个随从和随机一个相邻随从造成$3点伤害。（当你有10点法力值时升级。）
// Deal $3 damage to a minion and a random adjacent one. (Upgrades when you have 10 Mana.)
internal static class Sim_BAR_044t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
