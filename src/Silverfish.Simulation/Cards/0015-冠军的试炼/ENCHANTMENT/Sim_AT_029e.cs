using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 暗钩手
// Extra Stabby
// +1攻击力
// +1 Attack
internal static class Sim_AT_029e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_029e", new(StatModifierMode.Add, Attack: 1));
}
