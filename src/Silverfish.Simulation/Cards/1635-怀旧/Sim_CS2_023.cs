namespace Silverfish.Simulation.Cards;

// 奥术智慧
// Arcane Intellect
// 抽两张牌。
// Draw 2 cards.
[CardSim("CS2_023")]
internal static class Sim_CS2_023
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DrawEffect(2)]
    };
}
