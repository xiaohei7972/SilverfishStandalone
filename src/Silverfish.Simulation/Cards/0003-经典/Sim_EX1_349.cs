namespace Silverfish.Simulation.Cards;

// 神恩术
// Divine Favor
// 抽若干数量的牌，直到你的手牌数量等同于你对手的手牌数量。
// Draw cards until you have as many in hand as your opponent.
[CardSim("EX1_349")]
internal static class Sim_EX1_349
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DynamicDrawEffect(DynamicDrawMode.MatchOpponentHandSize)]
    };
}
