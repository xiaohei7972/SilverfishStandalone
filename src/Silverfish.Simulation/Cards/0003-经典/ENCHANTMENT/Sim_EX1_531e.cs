using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 进食充分
// Well Fed
// 攻击力和生命值提高。
// Increased Attack and Health.
internal static class Sim_EX1_531e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_531e", new(StatModifierMode.Add, Attack: 2, MaximumHealth: 1));
}
