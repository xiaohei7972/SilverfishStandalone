using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

internal static class Sim_GIL_803e
{
    internal const string CardId = "GIL_803e";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 3), EnchantmentDuration.CurrentTurn);
}
