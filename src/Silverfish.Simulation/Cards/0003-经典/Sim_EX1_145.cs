using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 伺机待发
// Preparation
// 在本回合中，你所施放的下一个法术的法力值消耗减少（2）点。
// The next spell you cast this turn costs (2) less.
[CardSim("EX1_145")]
internal static class Sim_EX1_145
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new AddTemporaryCostAuraEffect(EntityKind.Spell, CostModifierMode.Add, -2,
                ConsumedOnPlay: true, EnchantmentCardId: Sim_EX1_145o.CardId)
        ]
    };
}
