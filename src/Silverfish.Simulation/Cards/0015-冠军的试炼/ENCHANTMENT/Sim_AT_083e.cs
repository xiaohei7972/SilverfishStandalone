using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 龙鹰骑士
// Dragonhawkery
// 在本回合中风怒。
// Windfury this turn.
internal static class Sim_AT_083e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_083e", null, EnchantmentDuration.CurrentTurn)
        {
            GrantedTag = GAME_TAG.WINDFURY,
        };
}
