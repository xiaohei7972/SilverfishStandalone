namespace Silverfish.Simulation.Cards;

// 奥能铁匠
// Arcanosmith
// 战吼：召唤一个0/5并具有嘲讽的随从。
// Battlecry: Summon a 0/5 minion with Taunt.
[CardSim("KAR_710")]
internal static class Sim_KAR_710
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SummonEffect("KAR_710m", SummonPlacement.RightOfSource)],
    };
}
