namespace Silverfish.Simulation.Cards;

// 吹嘘海盗
// Swashburglar
// 战吼：&随机将一张另一职业的卡牌置入你的手牌。
// Battlecry:& Add a random card from another class to_your hand.
[CardSim("KAR_069")]
internal static class Sim_KAR_069
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new CreateRandomMatchingCardEffect(new CardFilter()) { OtherClassOnly = true }],
    };
}
