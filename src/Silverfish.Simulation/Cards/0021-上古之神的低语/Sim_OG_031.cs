namespace Silverfish.Simulation.Cards;

// 暮光神锤
// Hammer of Twilight
// 亡语：&召唤一个4/2的元素。
// Deathrattle:& Summon a 4/2 Elemental.
[CardSim("OG_031")]
internal static class Sim_OG_031
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonEffect("OG_031a"),
        ],
    };
}
