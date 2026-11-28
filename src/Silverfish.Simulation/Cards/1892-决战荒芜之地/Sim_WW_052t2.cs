using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 瓶装萤光
// Bottled Lightbugs
// 召唤余下的1/1并具有吸血的萤光虫。@召唤{0}只1/1并具有吸血的萤光虫。
// Summon the excess 1/1 Lightbugs with Lifesteal.@Summon {0} 1/1 Lightbugs with Lifesteal.
internal static class Sim_WW_052t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
