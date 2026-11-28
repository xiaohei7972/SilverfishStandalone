using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 生来狂野
// Savage
// 在本回合中，+2攻击力。
// +2 Attack this turn.
internal static class Sim_AT_039e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_039e", new(StatModifierMode.Add, Attack: 2), EnchantmentDuration.CurrentTurn);
}
