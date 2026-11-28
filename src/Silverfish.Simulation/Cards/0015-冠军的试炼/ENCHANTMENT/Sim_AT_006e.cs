using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 达拉然之力
// Power of Dalaran
// 法术伤害提高。
// Increased Spell Damage.
internal static class Sim_AT_006e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_006e", null)
        {
            GrantedTag = GAME_TAG.SPELLPOWER,
        };
}
