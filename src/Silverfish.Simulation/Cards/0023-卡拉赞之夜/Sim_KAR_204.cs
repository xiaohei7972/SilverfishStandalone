namespace Silverfish.Simulation.Cards;

// 玛瑙主教
// Onyx Bishop
// 战吼：随机召唤一个在本局对战中死亡的友方随从。
// Battlecry: Summon a random friendly minion that died this game.
[CardSim("KAR_204")]
internal static class Sim_KAR_204
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ResurrectFriendlyMinionEffect { Placement = SummonPlacement.RightOfSource }],
    };
}
