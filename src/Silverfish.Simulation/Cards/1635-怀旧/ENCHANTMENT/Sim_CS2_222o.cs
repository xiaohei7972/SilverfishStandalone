using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 暴风城之力
// Might of Stormwind
// +1/+1。
// +1/+1.
internal static class Sim_CS2_222o
{
    internal const string CardId = "CS2_222o";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
