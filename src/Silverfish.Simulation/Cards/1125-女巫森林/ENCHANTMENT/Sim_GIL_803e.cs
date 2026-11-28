using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 草莽
// Reckless
// 在本回合中+3攻击力。
// +3 Attack this turn.
internal static class Sim_GIL_803e
{
    internal const string CardId = "GIL_803e";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 3), EnchantmentDuration.CurrentTurn);
}
