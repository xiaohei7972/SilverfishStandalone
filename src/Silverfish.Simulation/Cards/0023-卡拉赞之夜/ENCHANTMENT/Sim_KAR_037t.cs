using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 卡拉赞之谜
// Secrets of Karazhan
// +1/+1并具有嘲讽。
// +1/+1 and Taunt.
internal static class Sim_KAR_037t
{
    internal static StatEnchantmentRule Rule { get; } =
        new("KAR_037t", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1))
        {
            GrantedTag = GAME_TAG.TAUNT,
        };
}
