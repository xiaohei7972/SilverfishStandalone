using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 巨型体态
// Massive
// 嘲讽
// Taunt
internal static class Sim_UNG_999t6e
{
    internal static StatEnchantmentRule Rule { get; } = new("UNG_999t6e", null)
    {
        GrantedTag = GAME_TAG.TAUNT
    };
}
