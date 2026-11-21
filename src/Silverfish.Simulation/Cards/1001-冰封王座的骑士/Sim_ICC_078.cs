using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 雪崩
// Avalanche
// 冻结一个随从，并对其相邻的随从造成$3点伤害。
// Freeze a minion and deal $3 damage to adjacent ones.
internal static class Sim_ICC_078
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
