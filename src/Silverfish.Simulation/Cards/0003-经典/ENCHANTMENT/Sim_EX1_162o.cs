using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 狼群之力
// Strength of the Pack
// {0}使其获得+1攻击力。
// +1 Attack from {0}.
internal static class Sim_EX1_162o
{
    internal const string CardId = "EX1_162o";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 1));
}
