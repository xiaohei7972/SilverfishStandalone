using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 淬火
// Tempered
// 在本回合中，+2攻击力。
// +2 Attack this turn.
internal static class Sim_EX1_046e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_046e", new(StatModifierMode.Add, Attack: 2), EnchantmentDuration.CurrentTurn);
}
