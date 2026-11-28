namespace Silverfish.Simulation.Cards;

// 橱柜蜘蛛
// Pantry Spider
// 战吼：召唤一只1/3的蜘蛛。
// Battlecry: Summon a 1/3 Spider.
[CardSim("KAR_030a")]
internal static class Sim_KAR_030a
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SummonEffect("KAR_030", SummonPlacement.RightOfSource)],
    };
}
