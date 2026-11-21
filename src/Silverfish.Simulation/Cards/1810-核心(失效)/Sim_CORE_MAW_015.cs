using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 陪审义务
// Jury Duty
// 召唤两个白银之手新兵。使你的白银之手新兵获得+1/+1。
// Summon two Silver Hand Recruits. Give your Silver Hand Recruits +1/+1.
internal static class Sim_CORE_MAW_015
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
