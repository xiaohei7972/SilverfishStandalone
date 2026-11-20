using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

internal static class Sim_CS2_017o
{
    internal const string CardId = "CS2_017o";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 1), EnchantmentDuration.CurrentTurn);
}
