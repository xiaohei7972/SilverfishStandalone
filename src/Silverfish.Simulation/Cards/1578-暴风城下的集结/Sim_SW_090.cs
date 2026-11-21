using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 纳斯雷兹姆之触
// Touch of the Nathrezim
// 对一个随从造成$2点伤害。如果该随从死亡，则为你的英雄恢复#3点生命值。
// Deal $2 damage to a minion. If it dies, restore #3 Health to your hero.
internal static class Sim_SW_090
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
