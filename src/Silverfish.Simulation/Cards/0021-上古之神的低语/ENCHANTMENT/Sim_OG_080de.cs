using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 枯叶草
// Fadeleaf
// 获得潜行直到你的下个回合。
// Stealthed until your next turn.
internal static class Sim_OG_080de
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_080de", null, EnchantmentDuration.UntilNextTurn)
        {
            GrantedTag = GAME_TAG.STEALTH,
        };
}
