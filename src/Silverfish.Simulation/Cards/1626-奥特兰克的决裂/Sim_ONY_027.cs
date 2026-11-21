using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 勇气之戒
// Ring of Courage
// 可交易 使一个随从获得+1/+1。每有一个敌方随从，重复一次。
// Tradeable Give a minion +1/+1. Repeat for each enemy minion.
internal static class Sim_ONY_027
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
