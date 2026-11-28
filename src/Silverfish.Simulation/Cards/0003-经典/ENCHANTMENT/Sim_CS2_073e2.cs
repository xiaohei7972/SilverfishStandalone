using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 冷血
// Cold Blood
// +4攻击力。
// +4 Attack.
internal static class Sim_CS2_073e2
{
    internal static StatEnchantmentRule Rule { get; } =
        new("CS2_073e2", new(StatModifierMode.Add, Attack: 4));
}
