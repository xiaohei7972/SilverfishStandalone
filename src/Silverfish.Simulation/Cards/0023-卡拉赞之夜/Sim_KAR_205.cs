namespace Silverfish.Simulation.Cards;

// 镀银魔像
// Silverware Golem
// 如果你弃掉了这张随从牌，则会召唤它。
// If you discard this minion, summon it.
[CardSim("KAR_205")]
internal static class Sim_KAR_205
{
    internal static CardRules Rules { get; } = new()
    {
        WhenDiscarded = [new SummonDiscardedMinionEffect()],
    };
}
