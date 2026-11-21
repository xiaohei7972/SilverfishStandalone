using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 米莎
// Misha
// 嘲讽
// Taunt
[CardSim("NEW1_032")]
internal static class Sim_NEW1_032
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new();
}
