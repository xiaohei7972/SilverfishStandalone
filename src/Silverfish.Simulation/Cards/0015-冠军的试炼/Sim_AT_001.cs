using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 炎枪术
// Flame Lance
// 对一个随从造成$25点伤害。
// Deal $25 damage to a minion.
internal static class Sim_AT_001
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
