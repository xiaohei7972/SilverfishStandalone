using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 飞龙之眠
// Wyvern's Slumber
// 抉择：召唤两个休眠的魔种；或者对所有随从造成 $2点伤害。
// Choose One - Summon two Dormant Dreadseeds; or Deal $2 damage to all minions.
internal static class Sim_EDR_820
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
