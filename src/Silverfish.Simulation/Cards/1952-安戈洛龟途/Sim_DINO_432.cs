using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 奔行豹面具
// Panther Mask
// 将一个随从的属性值变为5/4并使其获得潜行。抽两张牌。
// Set a minion's stats to 5/4 and give it Stealth. Draw 2 cards.
internal static class Sim_DINO_432
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
