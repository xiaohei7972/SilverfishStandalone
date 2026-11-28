using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 暮光幼龙
// Twilight Drake
// 战吼： 你每有一张手牌，便获得+1生命值。
// Battlecry: Gain +1 Health for each card in your hand.
[CardSim("EX1_043")]
internal static class Sim_EX1_043
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ScaledEnchantStatsEffect(StatTarget.Source, StatScaleSource.FriendlyHandCount, HealthPerUnit: 1, EnchantmentCardId: Sim_EX1_043e.CardId)]
    };
}
