using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 恐怖利爪
// Dire Claws
// 在本回合中，+2攻击力。
// +2 Attack this turn.
internal static class Sim_AT_132_DRUIDe
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_132_DRUIDe", new(StatModifierMode.Add, Attack: 2), EnchantmentDuration.CurrentTurn);
}
