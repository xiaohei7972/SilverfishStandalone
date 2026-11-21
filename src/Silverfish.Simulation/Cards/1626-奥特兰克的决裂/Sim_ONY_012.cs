using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 灵魂坐骑
// Spirit Mount
// 使一个随从获得+1/+2和法术伤害+1。当该随从死亡时，召唤一只灵魂迅猛龙。
// Give a minion +1/+2 and Spell Damage +1. When it dies, summon a Spirit Raptor.
internal static class Sim_ONY_012
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
