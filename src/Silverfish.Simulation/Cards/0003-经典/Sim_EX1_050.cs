namespace Silverfish.Simulation.Cards;

// 寒光智者
// Coldlight Oracle
// 战吼：每个玩家抽两张牌。
// Battlecry: Each player draws 2 cards.
[CardSim("EX1_050")]
internal static class Sim_EX1_050
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DrawEffect(2), new DrawEffect(2, CardRecipient.Opponent)]
    };
}
