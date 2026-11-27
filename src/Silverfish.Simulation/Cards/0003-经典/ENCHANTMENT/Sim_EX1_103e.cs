using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 鱼人怒吼
// Mrghlglhal
// +2生命值。
// +2 Health.
internal static class Sim_EX1_103e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_103e", new(StatModifierMode.Add, MaximumHealth: 2));
}
