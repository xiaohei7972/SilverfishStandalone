using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 鲜血圣印
// Seal of Blood
// 使一个随从获得+3/+3和圣盾。对你的英雄造成$3点伤害。
// Give a minion +3/+3 and Divine Shield. Deal $3 damage to your hero.
internal static class Sim_RLK_922
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
