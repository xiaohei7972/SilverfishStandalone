using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 野蛮残食
// Cannibalize
// 攻击力提高。
// Increased Attack.
internal static class Sim_tt_004o
{
    internal static StatEnchantmentRule Rule { get; } =
        new("tt_004o", new(StatModifierMode.Add, Attack: 1));
}
