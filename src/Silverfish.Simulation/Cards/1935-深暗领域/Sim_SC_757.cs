using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 幻像
// Hallucination
// 召唤一个友方 星灵随从的一个复制。该复制受到的伤害翻倍。
// Summon a copy of a friendly Protoss minion. It takes double damage.
internal static class Sim_SC_757
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_MUST_HAVE_TAG, GAME_TAG.PROTOSS)],
        []);
    // </legacy-play-requirements>

}
