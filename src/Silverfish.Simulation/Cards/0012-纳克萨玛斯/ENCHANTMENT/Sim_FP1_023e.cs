using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 通灵教诲
// Power of the Ziggurat
// +3生命值。
// +3 Health.
internal static class Sim_FP1_023e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("FP1_023e", new(StatModifierMode.Add, Attack: 0, MaximumHealth: 3));
}
