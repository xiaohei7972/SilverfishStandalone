using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 公正之剑
// Sword of Justice
// 在你召唤一个随从后，使其获得+1/+1，这把武器失去1点耐久度。
// After you summon a minion, give it +1/+1 and this loses 1_Durability.
[CardSim("EX1_366")]
internal static class Sim_EX1_366
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterMinionSummoned, TriggerRelation.Friendly,
            [
                Sim_EX1_366e.Rule.ApplyTo(StatTarget.TriggerSubject),
                new ModifyWeaponEffect(DurabilityDelta: -1)
            ])
        ]
    };
}
