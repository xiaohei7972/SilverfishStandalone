namespace Silverfish.Simulation.Cards;

// 麦田傀儡
// Harvest Golem
// 亡语：召唤一个2/1的损坏的傀儡。
// Deathrattle: Summon a 2/1 Damaged Golem.
[CardSim("EX1_556")]
internal static class Sim_EX1_556
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new SummonEffect("skele21")]
    };
}
