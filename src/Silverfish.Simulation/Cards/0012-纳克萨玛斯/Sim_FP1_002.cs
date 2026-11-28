namespace Silverfish.Simulation.Cards;

// 鬼灵爬行者
// Haunted Creeper
// 亡语：召唤两只1/1的鬼灵蜘蛛。
// Deathrattle: Summon two 1/1 Spectral Spiders.
[CardSim("FP1_002")]
internal static class Sim_FP1_002
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonEffect("FP1_002t"),
            new SummonEffect("FP1_002t")
        ]
    };
}
