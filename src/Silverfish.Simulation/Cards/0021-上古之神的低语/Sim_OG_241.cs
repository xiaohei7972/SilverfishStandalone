namespace Silverfish.Simulation.Cards;

// 着魔村民
// Possessed Villager
// 亡语：召唤一个1/1的暗影兽。
// Deathrattle: Summon a 1/1 Shadowbeast.
[CardSim("OG_241")]
internal static class Sim_OG_241
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonEffect("OG_241a"),
        ],
    };
}
