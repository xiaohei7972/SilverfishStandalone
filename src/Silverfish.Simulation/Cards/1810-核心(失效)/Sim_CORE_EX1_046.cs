using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 黑铁矮人
// Dark Iron Dwarf
// 战吼：在本回合中，使一个随从获得+2攻击力。
// Battlecry: Give a minion +2_Attack this turn.
internal static class Sim_CORE_EX1_046
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
