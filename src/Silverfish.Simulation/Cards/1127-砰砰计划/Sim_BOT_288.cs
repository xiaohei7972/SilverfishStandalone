using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 实验室招募员
// Lab Recruiter
// 战吼：将一个友方随从的三张复制洗入你的牌库。
// Battlecry: Shuffle 3 copies of a friendly minion into your deck.
internal static class Sim_BOT_288
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
