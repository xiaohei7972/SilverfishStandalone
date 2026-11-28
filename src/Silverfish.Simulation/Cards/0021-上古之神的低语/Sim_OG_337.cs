using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 巨型独眼怪
// Cyclopian Horror
// 嘲讽，战吼：每有一个敌方随从，便获得+1生命值。
// Taunt. Battlecry: Gain +1 Health for each enemy minion.
[CardSim("OG_337")]
internal static class Sim_OG_337
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ScaledEnchantStatsEffect(StatTarget.Source, StatScaleSource.EnemyMinions, HealthPerUnit: 1, EnchantmentCardId: Sim_OG_337e.CardId),
        ],
    };
}
