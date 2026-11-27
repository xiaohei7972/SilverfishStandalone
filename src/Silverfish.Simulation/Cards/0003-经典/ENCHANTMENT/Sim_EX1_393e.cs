using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 激怒
// Enraged
// +3攻击力。
// +3 Attack.
internal static class Sim_EX1_393e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_393e", new(StatModifierMode.Add, Attack: 3));
}
