using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 大自然的防线
// Nature's Defense
// 召唤5个小精灵。
// Summon 5 Wisps.
internal static class Sim_GVG_041b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
