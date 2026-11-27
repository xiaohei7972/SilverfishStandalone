namespace Silverfish.Simulation.Cards;

// 疯狂的科学家
// Mad Scientist
// 亡语：将一个奥秘从你的牌库中置入战场。
// Deathrattle: Put a Secret from your deck into the battlefield.
[CardSim("FP1_004")]
internal static class Sim_FP1_004
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new PutSecretFromDeckEffect()
        ]
    };
}
