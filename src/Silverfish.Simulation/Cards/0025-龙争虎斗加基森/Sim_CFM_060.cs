using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 猩红法力浮龙
// Red Mana Wyrm
// 每当你施放一个法术，便获得 +2攻击力。
// Whenever you cast a spell, gain +2 Attack.
[CardSim("CFM_060")]
internal static class Sim_CFM_060
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.BeforeSpellResolved, TriggerRelation.Friendly,
                [new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, Attack: 2))
                    { EnchantmentCardId = Sim_CFM_060e.CardId }]),
        ],
    };
}
