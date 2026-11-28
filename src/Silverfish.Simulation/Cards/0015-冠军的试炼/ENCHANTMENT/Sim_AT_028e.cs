using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 真气突进
// Chi Lance
// +4攻击力。
// +4 Attack.
internal static class Sim_AT_028e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_028e", new(StatModifierMode.Add, Attack: 4));
}
