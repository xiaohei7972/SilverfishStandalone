using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 鱼人招潮者
// Murloc Tidecaller
// 每当你召唤一个鱼人，便获得 +1攻击力。
// Whenever you summon a Murloc, gain +1 Attack.
[CardSim("EX1_509")]
internal static class Sim_EX1_509
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.WhenMinionSummoned, TriggerRelation.Friendly, [Sim_EX1_509e.Rule.ApplyTo(StatTarget.Source)])
            {
                EntityMatch = TriggerEntityMatch.Other,
                CardFilter = new() { MinionType = MinionType.Murloc }
            }
        ]
    };
}
