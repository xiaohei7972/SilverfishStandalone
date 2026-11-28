namespace Silverfish.Simulation.Cards;

// 蛛魔
// Nerubian
// 亡语：召唤阿努巴拉克。
// Deathrattle: Summon Anub'arak.
[CardSim("AT_036t")]
internal static class Sim_AT_036t
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonEffect("AT_036"),
        ],
    };
}
