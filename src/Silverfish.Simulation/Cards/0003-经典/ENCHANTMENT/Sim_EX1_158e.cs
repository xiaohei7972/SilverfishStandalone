using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 丛林之魂
// Soul of the Forest
// 亡语：召唤一个2/2的树人。
// Deathrattle: Summon a 2/2 Treant.
internal static class Sim_EX1_158e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_158e", null)
        {
            GrantedTag = GAME_TAG.DEATHRATTLE,
            OnRemoved = [new(EnchantmentRemovalReason.Died, [new SummonEffect("EX1_158t")])]
        };
}
