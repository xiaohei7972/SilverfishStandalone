using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 撕咬
// Bite
// 在本回合中+4攻击力。
// +4 Attack this turn.
internal static class Sim_EX1_570e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_570e", new(StatModifierMode.Add, Attack: 4), EnchantmentDuration.CurrentTurn);
}
