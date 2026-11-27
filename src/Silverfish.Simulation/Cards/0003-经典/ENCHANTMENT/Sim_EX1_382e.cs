using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 老实点！
// Stand Down!
// 攻击力变为1。
// Attack changed to 1.
internal static class Sim_EX1_382e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_382e", new(StatModifierMode.Set, Attack: 1));
}
