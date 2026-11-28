namespace Silverfish.Simulation.Cards;

// 杂耍吞法者
// Sideshow Spelleater
// 战吼：复制对手的英雄技能。
// Battlecry: Copy your opponent's Hero Power.
[CardSim("AT_098")]
internal static class Sim_AT_098
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CopyOpponentHeroPowerEffect(),
        ],
    };
}
