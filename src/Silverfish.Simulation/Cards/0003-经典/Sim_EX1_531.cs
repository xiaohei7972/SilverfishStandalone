using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 食腐土狼
// Scavenging Hyena
// 每当一个友方野兽死亡，便获得+2/+1。
// Whenever a friendly Beast dies, gain +2/+1.
[CardSim("EX1_531")]
internal static class Sim_EX1_531
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.MinionDied, TriggerRelation.Friendly, [Sim_EX1_531e.Rule.ApplyTo(StatTarget.Source)])
            {
                EntityMatch = TriggerEntityMatch.Other,
                CardFilter = new() { MinionType = MinionType.Beast }
            }
        ]
    };
}
