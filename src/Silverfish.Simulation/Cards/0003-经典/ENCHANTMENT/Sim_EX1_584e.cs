using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 肯瑞托的教诲
// Teachings of the Kirin Tor
// 法术伤害+1
// Spell Damage +1.
internal static class Sim_EX1_584e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_584e", null)
        {
            GrantedTag = GAME_TAG.SPELLPOWER
        };
}
