using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 地卜术
// Geomancy
// 嘲讽。
// Taunt.
internal static class Sim_OG_284e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_284e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1))
        {
            GrantedTag = GAME_TAG.TAUNT,
        };
}
