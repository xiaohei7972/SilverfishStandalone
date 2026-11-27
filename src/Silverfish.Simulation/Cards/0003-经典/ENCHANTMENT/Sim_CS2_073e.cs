using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 冷血
// Cold Blood
// +2攻击力。
// +2 Attack.
internal static class Sim_CS2_073e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("CS2_073e", new(StatModifierMode.Add, Attack: 2));
}
