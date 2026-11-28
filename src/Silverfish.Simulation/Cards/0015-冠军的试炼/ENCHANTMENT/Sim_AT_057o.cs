using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 吃饱喝足
// Groomed
// 在本回合中免疫。
// Immune this turn.
internal static class Sim_AT_057o
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_057o", null, EnchantmentDuration.CurrentTurn)
        {
            GrantedTag = GAME_TAG.IMMUNE,
        };
}
