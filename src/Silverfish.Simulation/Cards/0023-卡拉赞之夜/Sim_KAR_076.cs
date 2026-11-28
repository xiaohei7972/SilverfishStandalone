using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 火焰之地传送门
// Firelands Portal
// 造成$6点伤害。随机召唤一个法力值消耗为（6）的随从。
// Deal $6 damage. Summon a random 6-Cost minion.
[CardSim("KAR_076")]
internal static class Sim_KAR_076
{
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.Selected, 6), new SummonMatchingMinionEffect(new CardFilter { MinimumPrintedCost = 6, MaximumPrintedCost = 6 })],
    };
}
