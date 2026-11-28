namespace Silverfish.Simulation.Cards;

// 阿努巴拉克
// Anub'arak
// 亡语：召唤一个4/4并具有“亡语：召唤阿努巴拉克”的蛛魔。
// Deathrattle: Summon a 4/4 Nerubian with "Deathrattle: Summon Anub'arak."
[CardSim("AT_036")]
internal static class Sim_AT_036
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonEffect("AT_036t"),
        ],
    };
}
