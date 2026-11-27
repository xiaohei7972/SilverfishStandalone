using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 锋利无比！
// Sharp!
// 恶毒铁匠使其获得+2攻击力。
// +2 Attack from Spiteful Smith.
internal static class Sim_CS2_221e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("CS2_221e", new(StatModifierMode.Add, Attack: 2));
}
