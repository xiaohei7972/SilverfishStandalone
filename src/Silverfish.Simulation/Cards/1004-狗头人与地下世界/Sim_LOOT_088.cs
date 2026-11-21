using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 英勇药水
// Potion of Heroism
// 使一个随从获得圣盾。抽 一张牌。
// Give a minion Divine_Shield. Draw a card.
internal static class Sim_LOOT_088
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
