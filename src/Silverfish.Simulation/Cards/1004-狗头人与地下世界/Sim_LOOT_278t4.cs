using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 希望药剂
// Elixir of Hope
// 使一个随从获得+2/+2，以及 “亡语：将本随从移回你的手牌。”
// Give a minion +2/+2 and "Deathrattle: Return this minion to your hand."
internal static class Sim_LOOT_278t4
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
