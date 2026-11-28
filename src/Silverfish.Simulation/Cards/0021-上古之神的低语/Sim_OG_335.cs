namespace Silverfish.Simulation.Cards;

// 变幻之影
// Shifting Shade
// 亡语：复制你对手的牌库中的一张牌，并将其置入你的手牌。
// Deathrattle: Copy a card from your opponent's deck _and add it to your hand.
[CardSim("OG_335")]
internal static class Sim_OG_335
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new CopyDeckCardsEffect(),
        ],
    };
}
