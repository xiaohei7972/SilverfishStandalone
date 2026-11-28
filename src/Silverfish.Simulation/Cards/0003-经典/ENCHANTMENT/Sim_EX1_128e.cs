using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 被隐藏
// Concealed
// 获得潜行直到你的下个回合。
// Stealthed until your next turn.
internal static class Sim_EX1_128e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_128e", null, EnchantmentDuration.UntilNextTurn)
        {
            GrantedTag = GAME_TAG.STEALTH
        };
}
