using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 图腾召唤
// Totemic Call
// 英雄技能 随机召唤一个基础图腾。
// Hero Power Summon a random basic Totem.
internal static class Sim_CS2_049_H2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ALL_BASIC_TOTEMS_NOT_IN_PLAY), new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
