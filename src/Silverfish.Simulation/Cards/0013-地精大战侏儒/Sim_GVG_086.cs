using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 重型攻城战车
// Siege Engine
// 每当你获得 护甲值，使本随从获得+1攻击力。
// Whenever you gain Armor, give this minion +1 Attack.
[CardSim("GVG_086")]
internal static class Sim_GVG_086
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.ArmorGained,
                TriggerRelation.Friendly,
                [
                    new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, 1))
                    {
                        EnchantmentCardId = Sim_GVG_086e.CardId
                    }
                ])
        ]
    };
}
