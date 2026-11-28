using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 神圣勇士
// Holy Champion
// 过量治疗：获得+2攻击力。
// Overheal: Gain +2 Attack.
[CardSim("AT_011")]
internal static class Sim_AT_011
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.Overheal, TriggerRelation.Friendly, [Sim_AT_011e.Rule.ApplyTo(StatTarget.Source)]) { EntityMatch = TriggerEntityMatch.Self },
        ],
    };
}
