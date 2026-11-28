using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 被注视
// Watched
// 属性值变成3/3。
// Stats changed to 3/3.
internal static class Sim_LOE_017e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("LOE_017e", new(StatModifierMode.Set, Attack: 3, MaximumHealth: 3));
}
