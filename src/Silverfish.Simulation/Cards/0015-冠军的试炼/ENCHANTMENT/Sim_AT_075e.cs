using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 骑士之威
// Might of the Hostler
// 战马训练师使其获得+2攻击力和嘲讽。
// Warhorse Trainer is granting +2 Attack and Taunt.
internal static class Sim_AT_075e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_075e", new(StatModifierMode.Add, Attack: 2))
        {
            GrantedTag = GAME_TAG.TAUNT,
        };
}
