using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 闪电链（等级1）
// Chain Lightning (Rank 1)
// 对一个随从和随机一个相邻随从造成$2点伤害。（当你有5点法力值时升级。）
// Deal $2 damage to a minion and a random adjacent one. (Upgrades when you have 5 Mana.)
internal static class Sim_BAR_044
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
