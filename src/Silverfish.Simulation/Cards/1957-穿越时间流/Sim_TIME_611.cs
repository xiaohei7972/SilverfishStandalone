using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 时间停滞
// Timestop
// 造成$3点伤害。随机冻结两个敌方随从。
// Deal $3 damage. Freeze two random enemy minions.
internal static class Sim_TIME_611
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
