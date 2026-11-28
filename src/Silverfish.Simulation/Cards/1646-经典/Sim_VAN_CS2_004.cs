using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 真言术：盾
// Power Word: Shield
// 使一个随从获得+2生命值。 抽一张牌。
// Give a minion +2_Health. Draw a card.
internal static class Sim_VAN_CS2_004
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
