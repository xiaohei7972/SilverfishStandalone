using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 萤光虫群
// Swarm of Lightbugs
// 召唤10只1/1并具有吸血的萤光虫。将超过随从数量上限的萤光虫存入法力值消耗为（1）的瓶子。
// Summon 10 1/1 Lightbugs with Lifesteal. Save any excess in a 1-Cost Bottle.
internal static class Sim_WW_052
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
