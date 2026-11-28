using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 放马过来！
// Bring it on!
// +1攻击力并具有嘲讽。
// +1 Attack and Taunt.
internal static class Sim_AT_116e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_116e", new(StatModifierMode.Add, Attack: 1))
        {
            GrantedTag = GAME_TAG.TAUNT,
        };
}
