using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// Mrglllraawrrrglrur!
// Mrglllraawrrrglrur!
// +2/+2。
// +2/+2.
internal static class Sim_LOE_113e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("LOE_113e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
