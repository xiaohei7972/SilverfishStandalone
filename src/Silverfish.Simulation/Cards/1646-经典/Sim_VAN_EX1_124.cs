using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 刺骨
// Eviscerate
// 造成$2点伤害；连击：改为造成$4点伤害。
// Deal $2 damage. Combo: Deal $4 damage instead.
internal static class Sim_VAN_EX1_124
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
