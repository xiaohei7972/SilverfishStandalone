namespace Silverfish.Simulation.Cards;

// 奈法利安
// Nefarian
// 战吼：随机将两张（你对手职业的）法术牌置入你的手牌。
// Battlecry: Add 2 random spells to your hand (from your opponent's class).
[CardSim("BRM_030")]
internal static class Sim_BRM_030
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CreateRandomOpponentClassSpellEffect(
                2,
                FallbackCardId: "BRM_030t")
        ]
    };
}
