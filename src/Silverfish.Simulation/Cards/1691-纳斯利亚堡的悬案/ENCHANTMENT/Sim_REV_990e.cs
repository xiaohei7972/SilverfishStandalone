using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 堕罪恩典
// Sinfall Boon
// +2攻击力。
// +2 Attack.
internal static class Sim_REV_990e
{
    internal const string CardId = "REV_990e";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 2));
}
