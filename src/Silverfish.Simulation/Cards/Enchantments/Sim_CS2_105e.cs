using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

internal static class Sim_CS2_105e
{
    internal const string CardId = "CS2_105e";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 4), EnchantmentDuration.CurrentTurn);
}
