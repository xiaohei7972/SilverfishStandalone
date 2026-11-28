using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 进食充分
// Well Fed
// +1/+1。
// +1/+1.
internal static class Sim_KAR_095e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("KAR_095e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
