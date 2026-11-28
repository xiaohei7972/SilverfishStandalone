namespace Silverfish.Simulation.Cards;

// 先祖知识
// Ancestral Knowledge
// 抽两张牌。过载：（1）
// Draw 2 cards. Overload: (1)
[CardSim("AT_053")]
internal static class Sim_AT_053
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DrawEffect(2),
        ],
    };
}
