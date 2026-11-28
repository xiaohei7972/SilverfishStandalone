using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 走私
// Smuggling
// 军备宝箱使其获得+2/+2。
// +2/+2 from Hidden Cache.
internal static class Sim_CFM_026e
{
    internal const string CardId = "CFM_026e";
    internal static StatEnchantmentRule Rule { get; } =
        new(CardId, new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
