using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 迷失丛林
// Lost in the Jungle
// 召唤两个{0}的白银之手新兵。
// Summon two {0} Silver Hand Recruits.
internal static class Sim_UNG_960
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
