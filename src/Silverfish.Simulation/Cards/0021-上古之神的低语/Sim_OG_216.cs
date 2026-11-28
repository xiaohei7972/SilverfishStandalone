namespace Silverfish.Simulation.Cards;

// 寄生恶狼
// Infested Wolf
// 亡语：召唤两只1/1的蜘蛛。
// Deathrattle: Summon two 1/1 Spiders.
[CardSim("OG_216")]
internal static class Sim_OG_216
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonEffect("OG_216a"),
            new SummonEffect("OG_216a"),
        ],
    };
}
