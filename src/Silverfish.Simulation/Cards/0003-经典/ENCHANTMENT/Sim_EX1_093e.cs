using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 阿古斯之手
// Hand of Argus
// +1/+1并具有嘲讽。
// +1/+1 and Taunt.
internal static class Sim_EX1_093e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_093e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1))
        {
            GrantedTag = GAME_TAG.TAUNT
        };
}
