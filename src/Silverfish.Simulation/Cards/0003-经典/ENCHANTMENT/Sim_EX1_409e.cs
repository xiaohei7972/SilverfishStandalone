using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 升级
// Upgraded
// +1攻击力和+1耐久度。
// +1 Attack and +1 Durability.
internal static class Sim_EX1_409e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_409e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
