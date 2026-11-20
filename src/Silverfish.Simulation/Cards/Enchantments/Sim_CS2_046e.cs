using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

internal static class Sim_CS2_046e
{
    internal const string CardId = "CS2_046e";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 3), EnchantmentDuration.CurrentTurn);
}
