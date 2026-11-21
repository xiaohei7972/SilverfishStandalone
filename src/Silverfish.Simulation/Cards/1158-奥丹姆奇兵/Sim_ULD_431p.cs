using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 帝王裹布
// Emperor Wraps
// 召唤一个友方随从的2/2的复制。
// Summon a 2/2 copy of a friendly minion.
internal static class Sim_ULD_431p
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
