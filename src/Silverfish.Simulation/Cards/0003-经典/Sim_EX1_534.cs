namespace Silverfish.Simulation.Cards;

// 长鬃草原狮
// Savannah Highmane
// 亡语：召唤两只2/2的土狼。
// Deathrattle: Summon two 2/2 Hyenas.
[CardSim("EX1_534")]
internal static class Sim_EX1_534
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new SummonEffect("EX1_534t"), new SummonEffect("EX1_534t")]
    };
}
