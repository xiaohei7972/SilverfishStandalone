using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 无私的英雄
// Selfless Hero
// 亡语：随机使一个友方随从获得圣盾。
// Deathrattle: Give a random friendly minion Divine Shield.
[CardSim("OG_221")]
internal static class Sim_OG_221
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new EnchantStatsEffect(StatTarget.RandomOtherFriendlyMinion) { GrantedTag = GAME_TAG.DIVINE_SHIELD },
        ],
    };
}
