using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 士气振奋
// Inspired
// 在本回合中可进行攻击。
// Can attack this turn.
internal static class Sim_AT_109e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_109e", new(StatModifierMode.Add, Attack: 0), EnchantmentDuration.CurrentTurn);
}
