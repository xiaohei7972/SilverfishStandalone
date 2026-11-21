using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 愈合
// Mend
// 为一个随从恢复所有生命值。抽一张牌。
// Restore a minion to full Health. Draw a card.
internal static class Sim_CATA_302
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
