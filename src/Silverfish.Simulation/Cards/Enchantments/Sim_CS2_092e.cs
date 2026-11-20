using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

internal static class Sim_CS2_092e
{
    internal const string CardId = "CS2_092e";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 4, MaximumHealth: 4));
}
