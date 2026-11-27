using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 无羁元素
// Unbound Elemental
// 在你使用一张具有过载的牌后，便获得+1/+1。
// After you play a card_with Overload, gain_+1/+1.
[CardSim("EX1_258")]
internal static class Sim_EX1_258
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterCardPlayed, TriggerRelation.Friendly, [Sim_EX1_258e.Rule.ApplyTo(StatTarget.Source)])
            {
                OverloadOnly = true
            }
        ]
    };
}
