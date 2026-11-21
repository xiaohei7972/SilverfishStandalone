using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 海獭欢游
// Romp of Otters
// 召唤六只1/1并具有突袭的海獭。
// Summon six 1/1 Otters with Rush.
internal static class Sim_TSC_650d
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
