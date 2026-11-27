namespace Silverfish.Simulation.Cards;

// 奥妮克希亚
// Onyxia
// 战吼：召唤数条1/1的雏龙，直到你的随从数量达到上限。
// Battlecry: Summon 1/1 Whelps until your side of the battlefield is full.
[CardSim("EX1_562")]
internal static class Sim_EX1_562
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new FillBoardSummonEffect("EX1_116t", AlternateSides: true)]
    };
}
