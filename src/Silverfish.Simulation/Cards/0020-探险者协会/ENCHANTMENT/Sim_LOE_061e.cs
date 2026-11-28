using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 泰坦之力
// Power of the Titans
// +3/+3。
// +3/+3.
internal static class Sim_LOE_061e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("LOE_061e", new(StatModifierMode.Add, Attack: 3, MaximumHealth: 3));
}
