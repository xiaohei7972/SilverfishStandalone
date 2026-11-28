using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 体型增大
// Size Increase
// 属性值提高。
// Increased stats.
internal static class Sim_CFM_064e
{
    internal const string CardId = "CFM_064e";
    internal static StatEnchantmentRule Rule { get; } =
        new(CardId, new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
