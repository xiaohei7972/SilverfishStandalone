using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 守卫梦境之路
// Dreamway Guardians
// 召唤两个1/2并具有吸血的 树妖。
// Summon two 1/2 Dryads with Lifesteal.
internal static class Sim_DAL_733
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
