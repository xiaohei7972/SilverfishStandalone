using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火焰花毒素
// Firebloom Toxin
// 造成$2点伤害。
// Deal $2 damage.
internal static class Sim_OG_080f
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
