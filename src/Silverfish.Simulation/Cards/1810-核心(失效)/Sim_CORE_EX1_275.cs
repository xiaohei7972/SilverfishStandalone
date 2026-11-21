using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰锥术
// Cone of Cold
// 冻结一个随从和其相邻的随从，并对它们造成$1点伤害。
// Freeze a minion and the minions next to it, and deal $1 damage to them.
internal static class Sim_CORE_EX1_275
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
