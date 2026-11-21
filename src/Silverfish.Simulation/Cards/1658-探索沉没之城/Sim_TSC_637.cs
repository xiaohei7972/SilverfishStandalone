using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 间歇热泉
// Scalding Geyser
// 造成$2点伤害。探底。
// Deal $2 damage. Dredge.
internal static class Sim_TSC_637
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
