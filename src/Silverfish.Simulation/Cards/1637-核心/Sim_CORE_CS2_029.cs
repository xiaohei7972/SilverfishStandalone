using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火球术
// Fireball
// 造成$6点伤害。
// Deal $6 damage.
internal static class Sim_CORE_CS2_029
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
