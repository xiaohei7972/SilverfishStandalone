using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 自然印记
// Mark of Nature
// 本随从拥有+4生命值和嘲讽。
// This minion has +4 Health and Taunt.
internal static class Sim_EX1_155be
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_155be", new(StatModifierMode.Add, MaximumHealth: 4)) { GrantedTag = GAME_TAG.TAUNT };
}
