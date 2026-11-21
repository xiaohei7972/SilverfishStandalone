using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 活性孢子
// Living Spores
// 亡语：召唤两个1/1的植物。
// Deathrattle: Summon two 1/1 Plants.
internal static class Sim_UNG_999t2e
{
    internal static StatEnchantmentRule Rule { get; } = new("UNG_999t2e", null)
    {
        GrantedTag = GAME_TAG.DEATHRATTLE,
        OnRemoved = [new(EnchantmentRemovalReason.Died, [new SummonEffect("UNG_999t2t1"), new SummonEffect("UNG_999t2t1")])]
    };
}
