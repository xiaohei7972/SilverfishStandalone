namespace Silverfish.Simulation.Cards;

// 慈祥的外婆
// Kindly Grandmother
// 亡语：召唤一只3/2的大灰狼。
// Deathrattle: Summon a 3/2 Big Bad Wolf.
[CardSim("KAR_005")]
internal static class Sim_KAR_005
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new SummonEffect("KAR_005a")],
    };
}
