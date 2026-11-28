using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 击碎者克里丹
// Keli'dan the Breaker
// 战吼：消灭一个随从。如果在本回合被抽到，则改为消灭除本随从外的所有随从。
// Battlecry: Destroy a minion. If drawn this turn, instead destroy all minions except this one.
internal static class Sim_BT_196
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
