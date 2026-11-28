using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 刨根问底
// The Scoop
// 属性值提高。
// Increased stats.
internal static class Sim_CFM_851e
{
    internal const string CardId = "CFM_851e";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
