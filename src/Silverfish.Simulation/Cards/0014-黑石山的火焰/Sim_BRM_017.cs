namespace Silverfish.Simulation.Cards;

// 复活术
// Resurrect
// 随机召唤一个在本局对战中死亡的友方随从。
// Summon a random friendly minion that died this game.
[CardSim("BRM_017")]
internal static class Sim_BRM_017
{
    internal static CardRules Rules { get; } = new()
    {
        Requirements = PlayRequirements.BoardSpace | PlayRequirements.FriendlyMinionDeath,
        OnPlay = [new ResurrectFriendlyMinionEffect()]
    };
}
