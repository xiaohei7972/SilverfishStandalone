using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 生长
// Growth
// 格鲁尔正在变得更强......
// Gruul is growing...
internal static class Sim_NEW1_038o
{
    internal static StatEnchantmentRule Rule { get; } =
        new("NEW1_038o", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
