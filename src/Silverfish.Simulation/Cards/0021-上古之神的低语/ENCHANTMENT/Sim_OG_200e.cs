using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 末日不再
// Doom Free
// 攻击力变为7。
// Attack set to 7.
internal static class Sim_OG_200e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_200e", new(StatModifierMode.Set, Attack: 7));
}
