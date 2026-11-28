using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 灵魂之力
// Soul Power
// 攻击力提高。
// Increased Attack.
internal static class Sim_OG_302e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_302e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
