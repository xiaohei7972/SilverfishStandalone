using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 蛛魔孢子
// Nerubian Spores
// 死亡时获得一张野兽牌。
// Get a Beast when this dies.
internal static class Sim_OG_045a
{
    internal static StatEnchantmentRule Rule { get; } =
        new("OG_045a", null)
        {
            GrantedTag = GAME_TAG.DEATHRATTLE,
            OnRemoved = [new(EnchantmentRemovalReason.Died, [new CreateRandomMatchingCardEffect(new CardFilter { Kind = EntityKind.Minion, MinionType = MinionType.Beast })])],
        };
}
