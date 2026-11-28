using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 野蛮印记
// Savage Mark
// +1/+1。
// +1/+1.
internal static class Sim_CFM_614e
{
    internal const string CardId = "CFM_614e";
    internal static StatEnchantmentRule Rule { get; } =
        new(CardId, new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
