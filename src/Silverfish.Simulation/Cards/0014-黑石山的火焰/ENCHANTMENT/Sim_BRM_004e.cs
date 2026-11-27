using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 暮光坚韧
// Twilight Endurance
// 生命值提高。
// Increased Health.
internal static class Sim_BRM_004e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("BRM_004e", new(StatModifierMode.Add, Attack: 0, MaximumHealth: 2));
}
