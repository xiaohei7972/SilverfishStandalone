namespace Silverfish.Simulation.Cards;

// 瓦里安·乌瑞恩
// Varian Wrynn
// 战吼：抽三张牌。将抽到的随从牌直接置入战场。
// Battlecry: Draw 3 cards. Put any minions you drew directly into the battlefield.
[CardSim("AT_072")]
internal static class Sim_AT_072
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DrawEffect(3) { SummonMinions = true },
        ],
    };
}
