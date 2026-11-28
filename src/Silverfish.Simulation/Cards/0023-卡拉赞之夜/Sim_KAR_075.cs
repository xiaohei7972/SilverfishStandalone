using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 月光林地传送门
// Moonglade Portal
// 恢复#6点生命值。随机召唤一个法力值消耗为（6）的 随从。
// Restore #6 Health. Summon a random 6-Cost minion.
[CardSim("KAR_075")]
internal static class Sim_KAR_075
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new HealEffect(HealTarget.Selected, 6), new SummonMatchingMinionEffect(new CardFilter { MinimumPrintedCost = 6, MaximumPrintedCost = 6 })],
    };
}
