using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 精彩绝伦
// Incredible Impression
// 攻击力和生命值为1。
// Attack and Health set to 1.
internal static class Sim_KAR_114e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("KAR_114e", new(StatModifierMode.Set, Attack: 1, MaximumHealth: 1));
}
