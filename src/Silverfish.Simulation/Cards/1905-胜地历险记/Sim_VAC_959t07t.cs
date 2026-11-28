using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 守卫护符
// Amulet of Warding
// 造成$6点伤害。
// Deal $6 damage.
internal static class Sim_VAC_959t07t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
