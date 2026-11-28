namespace Silverfish.Simulation.Cards;

// 苦战傀儡
// Blood Warriors
// 将每个受伤的友方随从的各一张复制置入你的手牌。
// Add a copy of each damaged friendly minion to your hand.
[CardSim("OG_276")]
internal static class Sim_OG_276
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CopyFriendlyBoardToHandEffect(DamagedOnly: true),
        ],
    };
}
