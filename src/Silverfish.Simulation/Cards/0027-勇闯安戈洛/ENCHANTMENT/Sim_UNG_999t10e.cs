using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 氤氲迷雾
// Shrouding Mist
// 获得潜行直到你的下个回合。
// Stealthed until your next turn.
internal static class Sim_UNG_999t10e
{
    internal static StatEnchantmentRule Rule { get; } = new("UNG_999t10e", null, EnchantmentDuration.UntilNextTurn)
    {
        GrantedTag = GAME_TAG.STEALTH
    };
}
