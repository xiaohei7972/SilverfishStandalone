using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 幻影
// Illusion
// 抽到时召唤 嘲讽
// Summoned When Drawn Taunt
internal static class Sim_EDR_260t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
