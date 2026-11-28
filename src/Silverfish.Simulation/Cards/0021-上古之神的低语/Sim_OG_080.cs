namespace Silverfish.Simulation.Cards;

// 毒心者夏克里尔
// Xaril, Poisoned Mind
// 战吼，亡语：随机将一张毒素牌置入你的手牌。
// Battlecry and Deathrattle: Add a random Toxin card to your hand.
[CardSim("OG_080")]
internal static class Sim_OG_080
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CreateRandomCardEffect(["OG_080b", "OG_080c", "OG_080d", "OG_080e", "OG_080f"]),
        ],
        Deathrattle =
        [
            new CreateRandomCardEffect(["OG_080b", "OG_080c", "OG_080d", "OG_080e", "OG_080f"]),
        ],
    };
}
