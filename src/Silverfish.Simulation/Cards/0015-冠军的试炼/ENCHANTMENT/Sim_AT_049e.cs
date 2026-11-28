using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 雷霆崖之力
// Power of the Bluff
// 攻击力提高。
// Increased Attack.
internal static class Sim_AT_049e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_049e", new(StatModifierMode.Add, Attack: 2));
}
