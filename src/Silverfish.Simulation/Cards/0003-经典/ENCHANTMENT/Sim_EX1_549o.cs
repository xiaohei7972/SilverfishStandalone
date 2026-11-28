using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 狂野怒火
// Bestial Wrath
// 在本回合中，+2攻击力和免疫。
// +2 Attack and Immune this turn.
internal static class Sim_EX1_549o
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_549o", new(StatModifierMode.Add, Attack: 2), EnchantmentDuration.CurrentTurn)
        {
            GrantedTag = GAME_TAG.IMMUNE
        };
}
