using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 爆裂护盾
// Crackling Shield
// 圣盾
// Divine Shield
internal static class Sim_UNG_999t8e
{
    internal static StatEnchantmentRule Rule { get; } = new("UNG_999t8e", null)
    {
        GrantedTag = GAME_TAG.DIVINE_SHIELD
    };
}
