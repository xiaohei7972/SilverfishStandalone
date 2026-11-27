namespace Silverfish.Simulation.Cards;

// 凯恩·血蹄
// Cairne Bloodhoof
// 嘲讽。亡语：&召唤一个5/5的贝恩·血蹄。
// Taunt Deathrattle:& Summon a 5/5 Baine Bloodhoof.
[CardSim("EX1_110")]
internal static class Sim_EX1_110
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new SummonEffect("EX1_110t")]
    };
}
