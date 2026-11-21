using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰霜摆件
// Frosty Décor
// 召唤两个2/4并具有嘲讽和“亡语：获得4点护甲值”的元素。
// Summon two 2/4 Elementals with Taunt and "Deathrattle: Gain 4 Armor".
internal static class Sim_VAC_305
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
