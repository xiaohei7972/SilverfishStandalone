namespace Silverfish.Simulation.Cards;

// 神秘挑战者
// Mysterious Challenger
// 战吼：将每种不同的奥秘从你的牌库中置入战场。
// Battlecry: Put one of each Secret from your deck into the battlefield.
[CardSim("AT_079")]
internal static class Sim_AT_079
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new PutSecretFromDeckEffect(5),
        ],
    };
}
