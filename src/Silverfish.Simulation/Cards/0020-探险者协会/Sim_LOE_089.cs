namespace Silverfish.Simulation.Cards;

// 摇摆的俾格米
// Wobbling Runts
// 亡语：召唤三个2/2的俾格米。
// Deathrattle: Summon three 2/2 Runts.
[CardSim("LOE_089")]
internal static class Sim_LOE_089
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonEffect("LOE_089t"),
            new SummonEffect("LOE_089t2"),
            new SummonEffect("LOE_089t3"),
        ],
    };
}
