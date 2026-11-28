using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 石南草
// Briarthorn
// +3攻击力。
// +3 Attack.
internal static class Sim_OG_080ee
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_080ee", new(StatModifierMode.Add, Attack: 3));
}
