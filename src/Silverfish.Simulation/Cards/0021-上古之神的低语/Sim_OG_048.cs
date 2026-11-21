using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 亚煞极印记
// Mark of Y'Shaarj
// 使一个随从获得+2/+2。 如果该随从是野兽，抽一张牌。
// Give a minion +2/+2. If it's a Beast, draw a card.
internal static class Sim_OG_048
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
