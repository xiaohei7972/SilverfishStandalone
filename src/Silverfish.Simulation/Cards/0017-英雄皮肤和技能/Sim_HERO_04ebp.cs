using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 援军
// Reinforce
// 英雄技能 召唤一个{0}的白银之手新兵。
// Hero Power Summon a {0} Silver Hand Recruit.
internal static class Sim_HERO_04ebp
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
