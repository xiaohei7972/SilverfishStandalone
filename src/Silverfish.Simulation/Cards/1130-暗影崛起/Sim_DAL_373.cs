using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 急速射击
// Rapid Fire
// 双生法术 造成$2点伤害。
// Twinspell Deal $2 damage.
internal static class Sim_DAL_373
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
