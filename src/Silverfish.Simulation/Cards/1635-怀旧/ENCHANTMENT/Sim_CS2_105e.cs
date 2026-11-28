using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 英勇打击
// Heroic Strike
// 在本回合中，+4攻击力。
// +4 Attack this turn.
internal static class Sim_CS2_105e
{
    internal const string CardId = "CS2_105e";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 4), EnchantmentDuration.CurrentTurn);
}
