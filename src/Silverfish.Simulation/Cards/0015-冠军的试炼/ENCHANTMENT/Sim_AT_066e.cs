using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 奥格瑞玛之锋
// Forges of Orgrimmar
// 攻击力提高。
// Increased Attack.
internal static class Sim_AT_066e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_066e", new(StatModifierMode.Add, Attack: 1));
}
