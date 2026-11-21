using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 烈焰利爪
// Flaming Claws
// +3攻击力。
// +3 Attack.
internal static class Sim_UNG_999t3e
{
    internal static StatEnchantmentRule Rule { get; } = new("UNG_999t3e", new(StatModifierMode.Add, Attack: 3));
}
