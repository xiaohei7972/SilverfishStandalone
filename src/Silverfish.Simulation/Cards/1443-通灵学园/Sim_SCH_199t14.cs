using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 转校生
// Transfer Student
// 亡语：随机将一张巫妖王牌置入你的 手牌。
// Deathrattle: Add a random Lich King card to your_hand.
internal static class Sim_SCH_199t14
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
