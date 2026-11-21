using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 图腾崇拜
// Totemic Slam
// 英雄技能 召唤一个你想要的图腾。
// Hero Power Summon a Totem of your choice.
internal static class Sim_HERO_02fbp2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
