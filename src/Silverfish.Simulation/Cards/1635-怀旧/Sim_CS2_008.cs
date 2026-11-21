using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 月火术
// Moonfire
// 造成$1点伤害。
// Deal $1 damage.
internal static class Sim_CS2_008
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
