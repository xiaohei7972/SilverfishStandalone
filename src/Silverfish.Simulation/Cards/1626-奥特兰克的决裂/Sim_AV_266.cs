using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冷风
// Windchill
// 冻结一个随从。抽一张牌。
// Freeze a minion. Draw a card.
internal static class Sim_AV_266
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
