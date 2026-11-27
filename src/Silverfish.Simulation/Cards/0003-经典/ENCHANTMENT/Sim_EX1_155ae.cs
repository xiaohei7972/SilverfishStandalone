using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 自然印记
// Mark of Nature
// 本随从拥有+4攻击力。
// This minion has +4 Attack.
internal static class Sim_EX1_155ae
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_155ae", new(StatModifierMode.Add, Attack: 4));
}
