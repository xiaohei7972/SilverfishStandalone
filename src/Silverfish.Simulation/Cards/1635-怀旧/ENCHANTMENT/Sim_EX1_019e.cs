using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 祭司的祝福
// Cleric's Blessing
// +1/+1。
// +1/+1.
internal static class Sim_EX1_019e
{
    internal const string CardId = "EX1_019e";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
