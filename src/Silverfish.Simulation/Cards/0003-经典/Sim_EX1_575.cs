namespace Silverfish.Simulation.Cards;

// 法力之潮图腾
// Mana Tide Totem
// 在你的回合结束时，抽一张牌。
// At the end of your turn, draw a card.
[CardSim("EX1_575")]
internal static class Sim_EX1_575
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [new DrawEffect(1)])]
    };
}
