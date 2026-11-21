using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 空中团战
// Air Raid
// 双生法术 召唤两个{0}并具有嘲讽的白银之手新兵。
// Twinspell Summon two {0} Silver_Hand Recruits with Taunt.
internal static class Sim_YOD_012
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
