using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 石化
// Fossilized
// 拥有嘲讽。
// Has Taunt.
internal static class Sim_LOE_073e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("LOE_073e", null)
        {
            GrantedTag = GAME_TAG.TAUNT,
        };
}
