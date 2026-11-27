using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 奥秘守护者
// Secretkeeper
// 每当有一张奥秘牌被使用时，便获得+1/+1。
// Whenever a Secret is played, gain +1/+1.
[CardSim("EX1_080")]
internal static class Sim_EX1_080
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.BeforeSpellResolved, TriggerRelation.Any, [Sim_EX1_080o.Rule.ApplyTo(StatTarget.Source)])
            {
                SecretOnly = true
            }
        ]
    };
}
