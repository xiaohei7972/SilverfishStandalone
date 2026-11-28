namespace Silverfish.Simulation.Cards;

// 埃提耶什
// Atiesh
// 在你施放一个法术后，随机召唤一个法力值消耗相同的随从。 失去1点耐久度。
// After you cast a spell, summon a random minion of that Cost. Lose 1 Durability.
[CardSim("KAR_097t")]
internal static class Sim_KAR_097t
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterSpellResolved, TriggerRelation.Friendly, [new SummonMatchingCostEffect { UsePlayedCost = true, DurabilityLossOnSummon = 1 }]),
        ],
    };
}
