using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 阿莱克丝塔萨之赐
// Alexstrasza's Boon
// +1攻击力并获得冲锋。
// +1 Attack and Charge.
internal static class Sim_AT_071e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_071e", new(StatModifierMode.Add, Attack: 1))
        {
            GrantedTag = GAME_TAG.CHARGE,
        };
}
