using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 心灵之火
// Inner Fire
// 本随从的攻击力等同于其生命值。
// This minion's Attack is equal to its Health.
internal static class Sim_CS1_129e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("CS1_129e", new(StatModifierMode.Set, Attack: 0));
}
