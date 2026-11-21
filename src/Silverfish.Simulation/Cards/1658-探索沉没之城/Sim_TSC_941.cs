using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 保卫城市
// Guard the City
// 获得3点护甲值。召唤一个2/3并具有嘲讽的纳迦。
// Gain 3 Armor. Summon a 2/3 Naga with Taunt.
internal static class Sim_TSC_941
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
