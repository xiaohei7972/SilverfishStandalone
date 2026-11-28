using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恶魔交易
// Demonic Deal
// 吸血 对一个随从造成$4点伤害。将一张法力值消耗大于或等于（5）点的随机恶魔牌置于你的牌库顶。
// Lifesteal. Deal $4 damage to a minion. Put a random Demon that costs (5) or more on top of your deck.
internal static class Sim_WORK_014
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
