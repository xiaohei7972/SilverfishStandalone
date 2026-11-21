using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 变节
// Treachery
// 选择一个友方随从，交给你的 对手。
// Choose a friendly minion and give it to_your opponent.
internal static class Sim_ICC_206
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
