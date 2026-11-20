using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

internal static class Sim_NEW1_033o
{
    internal const string CardId = "NEW1_033o";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 1));
}
