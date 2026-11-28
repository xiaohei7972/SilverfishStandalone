using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 图腾团聚
// Totally Totems
// 召唤全部五种（？！）基础图腾。过载：（1）
// Summon all FIVE?! basic Totems. Overload: (1)
internal static class Sim_WON_091
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
