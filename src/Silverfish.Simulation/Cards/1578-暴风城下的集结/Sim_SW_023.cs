using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 挑衅
// Provoke
// 可交易 选择一个友方随从，使所有敌方随从攻击该随从。
// Tradeable Choose a friendly minion. Enemy minions attack it.
internal static class Sim_SW_023
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
