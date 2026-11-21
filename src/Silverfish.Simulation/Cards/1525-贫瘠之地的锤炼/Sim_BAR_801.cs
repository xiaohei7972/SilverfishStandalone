using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 击伤猎物
// Wound Prey
// 造成$1点伤害。召唤一只1/1并具有突袭的土狼。
// Deal $1 damage. Summon a 1/1 Hyena with Rush.
internal static class Sim_BAR_801
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
