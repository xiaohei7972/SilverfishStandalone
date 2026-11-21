using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 飞蝗虫群
// Swarm of Locusts
// 召唤七只1/1并具有突袭的 蝗虫。
// Summon seven 1/1 Locusts with Rush.
internal static class Sim_ULD_713
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
