using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 人民的力量
// Power of the People
// 攻击力提高。
// Increased Attack.
internal static class Sim_OG_113e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_113e", new(StatModifierMode.Add, Attack: 1));
}
