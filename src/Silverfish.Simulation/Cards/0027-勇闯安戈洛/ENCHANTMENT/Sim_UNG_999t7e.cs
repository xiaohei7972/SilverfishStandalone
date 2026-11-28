using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 闪电之速
// Lightning Speed
// 风怒
// Windfury
internal static class Sim_UNG_999t7e
{
    internal static StatEnchantmentRule Rule { get; } = new("UNG_999t7e", null)
    {
        GrantedTag = GAME_TAG.WINDFURY
    };
}
