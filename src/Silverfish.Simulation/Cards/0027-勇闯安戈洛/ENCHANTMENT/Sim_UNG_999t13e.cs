using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 毒液喷吐
// Poison Spit
// 剧毒
// Poisonous
internal static class Sim_UNG_999t13e
{
    internal static StatEnchantmentRule Rule { get; } = new("UNG_999t13e", null)
    {
        GrantedTag = GAME_TAG.POISONOUS
    };
}
