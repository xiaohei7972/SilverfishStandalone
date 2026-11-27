using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 拔根
// Uprooted
// +5攻击力。
// +5 Attack.
internal static class Sim_EX1_178be
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_178be", new(StatModifierMode.Add, MaximumHealth: 5))
        {
            GrantedTag = GAME_TAG.TAUNT
        };
}
