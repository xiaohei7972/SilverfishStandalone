namespace Silverfish.Simulation.Cards;

// 苦痛侍僧
// Acolyte of Pain
// 每当本随从受到伤害，&抽一张牌。
// Whenever this minion takes damage,& draw a_card.
[CardSim("EX1_007")]
internal static class Sim_EX1_007
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.Damage, TriggerRelation.Friendly, [new DrawEffect(1)])
        {
            EntityMatch = TriggerEntityMatch.Self
        }]
    };
}
