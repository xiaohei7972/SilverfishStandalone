using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 送葬者
// Undertaker
// 每当你召唤一个具有亡语的随从，便获得+1/+1。
// Whenever you summon a minion with Deathrattle, gain +1/+1.
[CardSim("FP1_028")]
internal static class Sim_FP1_028
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.WhenMinionSummoned,
                TriggerRelation.Friendly,
                [Sim_FP1_028e.Rule.ApplyTo(StatTarget.Source)])
            {
                EntityMatch = TriggerEntityMatch.Other,
                CardFilter = new()
                {
                    HasDeathrattle = true
                }
            }
        ]
    };
}
