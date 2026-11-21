using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 城防守卫
// City Defenses
// 召唤两个0/6并具有嘲讽的守卫。守卫在受到伤害时会获得+1攻击力。
// Summon two 0/6 Security with Taunt. They gain +1 Attack when damaged.
internal static class Sim_TLC_622
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
