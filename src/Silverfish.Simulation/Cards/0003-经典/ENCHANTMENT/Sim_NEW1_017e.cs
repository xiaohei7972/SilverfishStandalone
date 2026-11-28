using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 吃饱了
// Full Belly
// +2/+2。一个鱼人就吃饱了。
// +2/+2. Full of Murloc.
internal static class Sim_NEW1_017e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("NEW1_017e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
