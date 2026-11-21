using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 嗜血
// Bloodlust
// 在本回合中+3攻击力。
// +3 Attack this turn.
internal static class Sim_CS2_046e
{
    internal const string CardId = "CS2_046e";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 3), EnchantmentDuration.CurrentTurn);
}
