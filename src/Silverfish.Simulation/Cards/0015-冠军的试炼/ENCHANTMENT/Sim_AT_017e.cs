using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 暮光的拥抱
// Twilight's Embrace
// +1攻击力并具有嘲讽。
// +1 Attack and Taunt.
internal static class Sim_AT_017e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_017e", new(StatModifierMode.Add, Attack: 1))
        {
            GrantedTag = GAME_TAG.TAUNT,
        };
}
