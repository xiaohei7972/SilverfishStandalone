using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 被净化
// Purified
// 攻击力变为1。
// Attack changed to 1.
internal static class Sim_AT_081e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_081e", new(StatModifierMode.Set, Attack: 1));
}
