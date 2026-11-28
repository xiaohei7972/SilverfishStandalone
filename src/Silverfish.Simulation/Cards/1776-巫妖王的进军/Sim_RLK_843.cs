using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 奥术箭
// Arcane Bolt
// 造成$2点伤害。法力渴求（8）：改为造成$3点伤害。
// Deal $2 damage. Manathirst (8): Deal $3 damage instead.
internal static class Sim_RLK_843
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
