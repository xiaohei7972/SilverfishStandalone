namespace Silverfish.Simulation.Cards;

// 诅咒教派领袖
// Cult Master
// 在一个友方随从死亡后，抽一张牌。
// After a friendly minion dies, draw a card.
[CardSim("EX1_595")]
internal static class Sim_EX1_595
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.MinionDied, TriggerRelation.Friendly, [new DrawEffect(1)])
            {
                EntityMatch = TriggerEntityMatch.Other
            }
        ]
    };
}
