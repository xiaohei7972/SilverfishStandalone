using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// “深受鼓舞”
// 'Inspired'
// 在本回合中，本随从拥有+2攻击力。
// This minion has +2 Attack this turn.
internal static class Sim_CS2_188o
{
    internal static StatEnchantmentRule Rule { get; } =
        new("CS2_188o", new(StatModifierMode.Add, Attack: 2), EnchantmentDuration.CurrentTurn);
}
