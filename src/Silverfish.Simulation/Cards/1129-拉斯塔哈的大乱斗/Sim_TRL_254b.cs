using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 迅猛龙群
// Raptor Pack
// 召唤两个 3/2的迅猛龙。
// Summon two 3/2 Raptors.
internal static class Sim_TRL_254b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
