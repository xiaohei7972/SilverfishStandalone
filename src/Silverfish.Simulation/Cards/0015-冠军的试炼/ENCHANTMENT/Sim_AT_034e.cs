using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 小心剧毒
// Laced
// 攻击力提高。
// Increased Attack.
internal static class Sim_AT_034e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_034e", new(StatModifierMode.Add, Attack: 1));
}
