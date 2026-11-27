using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 任务达人
// Questing Adventurer
// 每当你使用一张牌时，便获得+1/+1。
// Whenever you play a card, gain +1/+1.
[CardSim("EX1_044")]
internal static class Sim_EX1_044
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.WhenCardPlayed, TriggerRelation.Friendly, [Sim_EX1_044e.Rule.ApplyTo(StatTarget.Source)])]
    };
}
