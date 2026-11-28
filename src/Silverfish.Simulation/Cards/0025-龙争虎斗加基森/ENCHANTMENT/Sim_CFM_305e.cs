using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 走私
// Smuggling
// 风驰电掣使其获得+1/+1。
// +1/+1 from Smuggler's Run.
internal static class Sim_CFM_305e
{
    internal const string CardId = "CFM_305e";
    internal static StatEnchantmentRule Rule { get; } =
        new(CardId, new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
