using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 装备
// Equipped
// +2攻击力。
// +2 Attack.
internal static class Sim_AT_084e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_084e", new(StatModifierMode.Add, Attack: 2));
}
