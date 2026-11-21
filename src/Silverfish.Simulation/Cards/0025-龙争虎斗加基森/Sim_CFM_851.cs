using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 勇敢的记者
// Daring Reporter
// 每当你的对手抽一张牌时，便获得+1/+1。
// Whenever your opponent draws a card, gain +1/+1.
[CardSim("CFM_851")]
internal static class Sim_CFM_851
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.CardDrawn, TriggerRelation.Opponent,
                [Sim_CFM_851e.Rule.ApplyTo(StatTarget.Source)])
        ]
    };
}
