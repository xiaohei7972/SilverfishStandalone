using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 激怒
// Enraged
// +1攻击力并获得风怒.
// +1 Attack and Windfury.
internal static class Sim_EX1_412e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_412e", new(StatModifierMode.Add, Attack: 1))
        {
            GrantedTag = GAME_TAG.WINDFURY
        };
}
