using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 探险帽
// Explorer's Hat
// +1/+1。亡语：将一张“探险帽”置入你的手牌。
// +1/+1. Deathrattle: Add an Explorer's Hat to your hand.
internal static class Sim_LOE_105e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("LOE_105e", new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1))
        {
            GrantedTag = GAME_TAG.DEATHRATTLE,
            OnRemoved = [new(EnchantmentRemovalReason.Died, [new CreateCardEffect("LOE_105", Count: 1)])],
        };
}
