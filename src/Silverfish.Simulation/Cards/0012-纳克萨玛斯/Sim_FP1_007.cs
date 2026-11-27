namespace Silverfish.Simulation.Cards;

// 蛛魔之卵
// Nerubian Egg
// 亡语：召唤一个4/4的蛛魔。
// Deathrattle: Summon a 4/4 Nerubian.
[CardSim("FP1_007")]
internal static class Sim_FP1_007
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonEffect("FP1_007t")
        ]
    };
}
