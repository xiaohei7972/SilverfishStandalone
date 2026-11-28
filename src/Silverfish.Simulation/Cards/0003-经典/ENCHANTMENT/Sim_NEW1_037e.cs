using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 装备
// Equipped
// 攻击力提高。
// Increased Attack.
internal static class Sim_NEW1_037e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("NEW1_037e", new(StatModifierMode.Add, Attack: 1));
}
