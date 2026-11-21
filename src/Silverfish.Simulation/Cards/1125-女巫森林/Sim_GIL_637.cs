using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 凶猛咆哮
// Ferocious Howl
// 抽一张牌。你每有一张手牌，便获得1点护甲值。
// Draw a card. Gain 1 Armor for each card in your hand.
internal static class Sim_GIL_637
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
