using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 紫罗兰庇护
// Violet Immunity
// 免疫
// Immune.
internal static class Sim_KAR_712e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("KAR_712e", null)
        {
            GrantedTag = GAME_TAG.IMMUNE,
        };
}
