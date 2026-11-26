namespace Silverfish.Simulation.Cards;

// 思维窃取
// Thoughtsteal
// 复制你对手的牌库中的两张牌，并将其置入你的手牌。
// Copy 2 cards in your opponent's deck and add them to your hand.
[CardSim("EX1_339")]
internal static class Sim_EX1_339
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new CopyDeckCardsEffect(ConditionPlayer.Opponent, Count: 2)]
    };
}
