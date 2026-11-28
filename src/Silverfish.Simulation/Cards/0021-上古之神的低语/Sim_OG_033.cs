namespace Silverfish.Simulation.Cards;

// 钢铁触须
// Tentacles for Arms
// 亡语：将这把武器移回你的手牌。
// Deathrattle: Return this to your hand.
[CardSim("OG_033")]
internal static class Sim_OG_033
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new ReturnSourceCardEffect(),
        ],
    };
}
