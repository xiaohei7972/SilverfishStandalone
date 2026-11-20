using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

internal static class Sim_EX1_619e
{
    internal const string CardId = "EX1_619e";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Set, MaximumHealth: 1));
}
