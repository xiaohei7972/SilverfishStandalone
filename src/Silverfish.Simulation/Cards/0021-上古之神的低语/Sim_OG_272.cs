namespace Silverfish.Simulation.Cards;

// 暮光召唤师
// Twilight Summoner
// 亡语：召唤一个5/5的无面破坏者。
// Deathrattle: Summon a 5/5 Faceless Destroyer.
[CardSim("OG_272")]
internal static class Sim_OG_272
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonEffect("OG_272t"),
        ],
    };
}
