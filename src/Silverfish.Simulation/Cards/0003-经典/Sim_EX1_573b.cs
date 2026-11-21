using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恩师的教诲
// Shan'do's Lesson
// 召唤两个2/2并具有嘲讽的树人。
// Summon two 2/2 Treants with Taunt.
internal static class Sim_EX1_573b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
