using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 新鲜气息
// Fresh Scent
// +2/+2。
// +2/+2.
internal static class Sim_YOD_005e
{
    internal const string CardId = "YOD_005e";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 2, MaximumHealth: 2));
}
