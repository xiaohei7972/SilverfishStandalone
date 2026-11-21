using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰冷智慧
// Brain Freeze
// 冻结一个随从。连击：并对其造成$3点伤害。
// Freeze a minion. Combo: Also deal $3 damage to it.
internal static class Sim_SCH_509
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
