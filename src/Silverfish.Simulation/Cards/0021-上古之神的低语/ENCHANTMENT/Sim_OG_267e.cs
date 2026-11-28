using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 深海鱼油
// Squid Oil Sheen
// +2攻击力
// +2 Attack
internal static class Sim_OG_267e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_267e", new(StatModifierMode.Add, Attack: 2));
}
