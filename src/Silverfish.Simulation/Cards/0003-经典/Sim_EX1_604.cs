using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 暴乱狂战士
// Frothing Berserker
// 每当一个随从 受到伤害，便获得+1攻击力。
// Whenever a minion takes damage, gain +1 Attack.
[CardSim("EX1_604")]
internal static class Sim_EX1_604
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.Damage, TriggerRelation.Any, [Sim_EX1_604o.Rule.ApplyTo(StatTarget.Source)])
            {
                EntityKind = EntityKind.Minion
            }
        ]
    };
}
