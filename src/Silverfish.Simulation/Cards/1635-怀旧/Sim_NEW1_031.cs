namespace Silverfish.Simulation.Cards;

// 动物伙伴
// Animal Companion
// 随机召唤一个野兽伙伴。
// Summon a random Beast Companion.
[CardSim("NEW1_031")]
internal static class Sim_NEW1_031
{
    internal static CardRules Rules { get; } = new()
    {
        Requirements = PlayRequirements.BoardSpace,
        OnPlay = [new SummonRandomMinionEffect(["NEW1_032", "NEW1_033", "NEW1_034"])]
    };
}
