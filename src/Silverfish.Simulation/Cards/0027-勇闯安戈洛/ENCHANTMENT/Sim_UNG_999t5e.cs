using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 液态外膜
// Liquid Membrane
// 扰魔
// Elusive.
internal static class Sim_UNG_999t5e
{
    internal static StatEnchantmentRule Rule { get; } = new("UNG_999t5e", null)
    {
        GrantedTag = GAME_TAG.ELUSIVE
    };
}
