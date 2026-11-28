namespace Silverfish.Simulation.Cards;

// 山谷之王穆克拉
// Mukla, Tyrant of the Vale
// 战吼：将两根香蕉置入你的手牌。
// Battlecry: Add 2 Bananas to your hand.
[CardSim("OG_122")]
internal static class Sim_OG_122
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CreateCardEffect("EX1_014t", Count: 2),
        ],
    };
}
