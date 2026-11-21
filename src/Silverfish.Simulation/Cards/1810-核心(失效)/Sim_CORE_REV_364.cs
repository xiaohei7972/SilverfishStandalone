using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 雄鹿冲锋
// Stag Charge
// 造成$3点伤害。随机召唤一个休眠的灵种。
// Deal $3 damage. Summon a random Dormant Wildseed.
internal static class Sim_CORE_REV_364
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
