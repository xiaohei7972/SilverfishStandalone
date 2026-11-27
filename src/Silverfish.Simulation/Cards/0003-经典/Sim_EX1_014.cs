namespace Silverfish.Simulation.Cards;

// 穆克拉
// King Mukla
// 战吼：使你的对手获得两根香蕉。
// Battlecry: Give your opponent 2 Bananas.
[CardSim("EX1_014")]
internal static class Sim_EX1_014
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new CreateCardEffect("EX1_014t", Recipient: CardRecipient.Opponent, Count: 2)]
    };
}
