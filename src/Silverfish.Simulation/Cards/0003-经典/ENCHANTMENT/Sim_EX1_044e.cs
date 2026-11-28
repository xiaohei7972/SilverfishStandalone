using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 升级
// Level Up!
// 攻击力和生命值提高。
// Increased Attack and Health.
internal static class Sim_EX1_044e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_044e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1));
}
