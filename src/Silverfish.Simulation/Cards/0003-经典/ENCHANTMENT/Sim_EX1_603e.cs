using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 鞭打成型
// Whipped Into Shape
// +2攻击力。
// +2 Attack.
internal static class Sim_EX1_603e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_603e", new(StatModifierMode.Add, Attack: 2));
}
