using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 真言术：仿
// Power Word: Replicate
// 选择一个友方随从，召唤一个该随从的5/5复制。
// Choose a friendly minion. Summon a 5/5 copy of it.
internal static class Sim_BOT_529
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
