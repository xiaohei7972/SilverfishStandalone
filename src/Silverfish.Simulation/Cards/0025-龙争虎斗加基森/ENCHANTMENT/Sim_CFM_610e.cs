using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 噬魂暗影
// Serrated Shadows
// +1/+1。
// +1/+1.
internal static class Sim_CFM_610e
{
    internal const string CardId = "CFM_610e";
    internal static StatEnchantmentRule Rule { get; } =
        new(CardId, new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
