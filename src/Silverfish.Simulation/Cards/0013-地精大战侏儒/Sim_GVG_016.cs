namespace Silverfish.Simulation.Cards;

// 魔能机甲
// Fel Reaver
// 每当你的对手使用一张卡牌时，便移除你的牌库顶的三张牌。
// Whenever your opponent plays a card, remove the top 3 cards of your deck.
[CardSim("GVG_016")]
internal static class Sim_GVG_016
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.WhenCardPlayed, TriggerRelation.Opponent, [new MillDeckEffect(3)])
        ]
    };
}
