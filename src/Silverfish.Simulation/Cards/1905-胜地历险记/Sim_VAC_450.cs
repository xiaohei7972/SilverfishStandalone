using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 悠闲的曲奇
// Carefree Cookie
// 恶魔猎手游客 在一个友方随从死亡后，随机召唤一个法力值消耗增加（1）点的随从。
// Demon Hunter Tourist After a friendly minion dies, summon a random minion that costs (1) more.
internal static class Sim_VAC_450
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_FROZEN_TARGET)],
        []);
    // </legacy-play-requirements>

}
