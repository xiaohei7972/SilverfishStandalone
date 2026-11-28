using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 激怒
// Enraged
// +3攻击力。
// +3 Attack.
internal static class Sim_OG_218e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_218e", new(StatModifierMode.Add, Attack: 3));
}
