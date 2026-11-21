using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 玛里苟斯的火球术
// Malygos's Fireball
// 造成$8点伤害。
// Deal $8 damage.
internal static class Sim_DRG_270t9
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
