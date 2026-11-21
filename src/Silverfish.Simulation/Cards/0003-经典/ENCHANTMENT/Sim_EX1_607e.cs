using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 怒火中烧
// Inner Rage
// +2攻击力。
// +2 Attack.
internal static class Sim_EX1_607e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_607e", new(StatModifierMode.Add, Attack: 2));
}
