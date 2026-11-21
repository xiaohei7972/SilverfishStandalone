using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 电磁炮
// Zap Cannon
// 造成$3点伤害。每回合切换。
// Deal $3 damage. Swaps each turn.
internal static class Sim_BOT_238p1
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
