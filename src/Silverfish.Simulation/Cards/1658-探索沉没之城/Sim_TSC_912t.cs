using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 沉没的舰船
// Sunken Vessel
// 抽到时施放 召唤两个3/3并具有潜行的海盗。
// Casts When Drawn Summon two 3/3 Pirates with Stealth.
internal static class Sim_TSC_912t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
