using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 午夜幼龙
// Midnight Drake
// 战吼：你每有一张其他手牌，便获得+1攻击力。
// Battlecry: Gain +1 Attack for each other card in your hand.
[CardSim("OG_320")]
internal static class Sim_OG_320
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ScaledEnchantStatsEffect(StatTarget.Source, StatScaleSource.FriendlyHandCount, 1, EnchantmentCardId: Sim_OG_320e.CardId),
        ],
    };
}
