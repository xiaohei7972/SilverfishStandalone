namespace Silverfish.Simulation.Cards;

// 艾德温·范克里夫
// Edwin VanCleef
// 连击：在本回合中，你每使用一张其他牌，便获得+2/+2。
// Combo: Gain +2/+2 for each other card you've played this turn.
[CardSim("EX1_613")]
internal static class Sim_EX1_613
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ScaledEnchantStatsEffect(StatTarget.Source, StatScaleSource.CardsPlayedBeforeThis, 2, 2, "EX1_613e")]
    };
}
