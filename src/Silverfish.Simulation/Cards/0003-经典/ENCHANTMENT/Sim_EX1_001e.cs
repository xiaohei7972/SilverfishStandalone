using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 护佑之力
// Warded
// 攻击力提高。
// Increased Attack.
internal static class Sim_EX1_001e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_001e", new(StatModifierMode.Add, Attack: 2));
}
