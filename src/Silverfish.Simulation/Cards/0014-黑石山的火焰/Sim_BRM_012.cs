using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 火焰驱逐者
// Fireguard Destroyer
// 战吼：获得1-4点攻击力。过载：（1）
// Battlecry: Gain 1-4 Attack. Overload: (1)
[CardSim("BRM_012")]
internal static class Sim_BRM_012
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new RandomEnchantStatsEffect(
                StatTarget.Source,
                1,
                4,
                EnchantmentCardId: Sim_BRM_012e.CardId)
        ]
    };
}
