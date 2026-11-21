using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 混乱化形
// Chaos Creation
// 造成$6点伤害。随机召唤一个法力值消耗为（6）的随从。摧毁你牌库底的6张牌。
// Deal $6 damage. Summon a random 6-Cost minion. Destroy the bottom 6 cards of your deck.
internal static class Sim_DEEP_031
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
