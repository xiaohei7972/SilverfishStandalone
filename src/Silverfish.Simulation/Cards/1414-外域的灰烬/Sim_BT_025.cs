using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 智慧圣契
// Libram of Wisdom
// 使一个随从获得+1/+1，以及“亡语：将一张‘智慧圣契’法术牌置入你的手牌。”
// Give a minion +1/+1 and "Deathrattle: Add a 'Libram of Wisdom' spell to your hand."
internal static class Sim_BT_025
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
