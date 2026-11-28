using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 伪装
// Disguised
// 获得潜行直到你的下个回合。
// Stealthed until your next turn.
internal static class Sim_NEW1_014e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("NEW1_014e", null, EnchantmentDuration.UntilNextTurn)
        {
            GrantedTag = GAME_TAG.STEALTH
        };
}
