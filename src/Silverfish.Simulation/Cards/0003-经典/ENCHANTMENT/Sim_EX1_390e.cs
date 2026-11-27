using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 激怒
// Enraged
// +3攻击力。
// +3 Attack.
internal static class Sim_EX1_390e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_390e", new(StatModifierMode.Add, Attack: 3));
}
