using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 脊刺
// Spines
// 在本回合中+4攻击力。
// +4 Attack this turn.
internal static class Sim_OG_047e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_047e", new(StatModifierMode.Add, Attack: 4), EnchantmentDuration.CurrentTurn);
}
