using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 托林的酒杯
// Tolin's Goblet
// 抽一张牌。用它的复制填满你的手牌。
// Draw a card. Fill your hand with copies of it.
internal static class Sim_LOOT_998h
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
