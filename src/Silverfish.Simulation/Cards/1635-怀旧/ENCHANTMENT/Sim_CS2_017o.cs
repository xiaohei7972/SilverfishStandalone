using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 额外攻击力
// Extra Attack
// 在本回合中，你的英雄拥有额外攻击力。
// Your hero has extra Attack this turn.
internal static class Sim_CS2_017o
{
    internal const string CardId = "CS2_017o";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 1), EnchantmentDuration.CurrentTurn);
}
