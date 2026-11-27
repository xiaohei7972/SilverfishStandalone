using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 齿轮光锤
// Coghammer
// 战吼：&随机使一个友方随从获得圣盾和嘲讽。
// Battlecry:& Give a random friendly minion Divine Shield and Taunt.
[CardSim("GVG_059")]
internal static class Sim_GVG_059
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EnchantStatsEffect(StatTarget.RandomOtherFriendlyMinion, null)
            {
                GrantedTag = GAME_TAG.DIVINE_SHIELD,
                SecondaryGrantedTag = GAME_TAG.TAUNT
            }
        ]
    };
}
