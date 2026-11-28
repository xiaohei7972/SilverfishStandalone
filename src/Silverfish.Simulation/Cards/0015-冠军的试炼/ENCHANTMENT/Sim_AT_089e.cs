using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 白骨之盾
// Boneguarded
// 生命值提高。
// Increased Health.
internal static class Sim_AT_089e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_089e", new(StatModifierMode.Add, MaximumHealth: 1));
}
