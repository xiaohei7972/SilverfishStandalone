using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 邪恶挥刺（等级3）
// Wicked Stab (Rank 3)
// 造成$6点伤害。
// Deal $6 damage.
internal static class Sim_BAR_319t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
