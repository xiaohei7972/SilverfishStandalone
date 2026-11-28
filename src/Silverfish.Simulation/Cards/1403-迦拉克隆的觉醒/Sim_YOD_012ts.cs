using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 空中团战
// Air Raid
// 召唤两个{0}并具有嘲讽的白银之手新兵。
// Summon two {0} Silver Hand Recruits with Taunt.
internal static class Sim_YOD_012ts
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
