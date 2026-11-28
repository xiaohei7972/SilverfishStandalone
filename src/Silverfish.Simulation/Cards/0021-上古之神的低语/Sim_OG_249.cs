namespace Silverfish.Simulation.Cards;

// 被感染的牛头人
// Infested Tauren
// 嘲讽 亡语：召唤一个2/2的泥浆怪。
// Taunt Deathrattle: Summon a 2/2 Slime.
[CardSim("OG_249")]
internal static class Sim_OG_249
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonEffect("OG_249a"),
        ],
    };
}
