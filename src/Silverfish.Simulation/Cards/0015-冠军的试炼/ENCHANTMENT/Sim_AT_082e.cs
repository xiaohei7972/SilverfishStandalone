using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 刻苦训练
// Training
// 攻击力提高。
// Increased Attack.
internal static class Sim_AT_082e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_082e", new(StatModifierMode.Add, Attack: 1));
}
