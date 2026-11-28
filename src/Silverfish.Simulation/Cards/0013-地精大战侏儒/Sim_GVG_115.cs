namespace Silverfish.Simulation.Cards;

// 托什雷
// Toshley
// 战吼，亡语： 将一张零件牌置入你的手牌。
// Battlecry and Deathrattle: Add a Spare Part card to your hand.
[CardSim("GVG_115")]
internal static class Sim_GVG_115
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new CreateRandomCardEffect(SpareParts.All)],
        Deathrattle = [new CreateRandomCardEffect(SpareParts.All)]
    };
}
