using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 巫卜
// Divination
// 消灭一个 友方小精灵以 抽三张牌。
// Destroy a friendly Wisp to draw 3 cards.
internal static class Sim_EDR_804
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_MUST_HAVE_TAG, (GAME_TAG)3881)],
        []);
    // </legacy-play-requirements>

}
