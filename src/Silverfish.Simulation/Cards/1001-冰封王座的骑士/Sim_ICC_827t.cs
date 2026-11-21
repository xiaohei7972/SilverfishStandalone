using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗影映像
// Shadow Reflection
// 每当你使用一张牌，变形成为该卡牌的复制。
// Each time you play a card, transform this into a copy of it.
internal static class Sim_ICC_827t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MUST_PLAY_OTHER_CARD_FIRST)],
        []);
    // </legacy-play-requirements>

}
