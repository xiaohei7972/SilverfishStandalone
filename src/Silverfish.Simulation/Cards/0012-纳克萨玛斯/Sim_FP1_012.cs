namespace Silverfish.Simulation.Cards;

// 淤泥喷射者
// Sludge Belcher
// 嘲讽，亡语：召唤一个1/2并具有嘲讽的泥浆怪。
// Taunt. Deathrattle: Summon a 1/2 Slime with Taunt.
[CardSim("FP1_012")]
internal static class Sim_FP1_012
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonEffect("FP1_012t")
        ]
    };
}
