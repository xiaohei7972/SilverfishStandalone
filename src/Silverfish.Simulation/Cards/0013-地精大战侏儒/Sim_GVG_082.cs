namespace Silverfish.Simulation.Cards;

// 发条侏儒
// Clockwork Gnome
// 亡语：将一张零件牌置入你的手牌。
// Deathrattle: Add a Spare Part card to your hand.
[CardSim("GVG_082")]
internal static class Sim_GVG_082
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new CreateRandomCardEffect(SpareParts.All)]
    };
}
