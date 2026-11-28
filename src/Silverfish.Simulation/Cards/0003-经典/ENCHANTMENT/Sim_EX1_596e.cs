using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 恶魔之火
// Demonfire
// 该恶魔拥有+2/+2。
// This Demon has +2/+2.
internal static class Sim_EX1_596e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_596e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
