namespace Silverfish.Simulation.Cards;

// 机械雪人
// Mechanical Yeti
// 亡语：使每个玩家获得一张零件牌。
// Deathrattle: Give each player a Spare Part.
[CardSim("GVG_078")]
internal static class Sim_GVG_078
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new CreateRandomCardEffect(SpareParts.All, 1, CardRecipient.Controller),
            new CreateRandomCardEffect(SpareParts.All, 1, CardRecipient.Opponent)
        ]
    };
}
