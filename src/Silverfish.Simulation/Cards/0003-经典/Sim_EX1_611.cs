using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 冰冻陷阱
// Freezing Trap
// 奥秘：当一个敌方随从攻击时，将其移回拥有者的手牌，并且法力值消耗增加（2）点。
// Secret: When an enemy minion attacks, return it to its owner's hand. It costs (2) more.
[CardSim("EX1_611")]
internal static class Sim_EX1_611
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AttackProposed, TriggerRelation.Opponent,
                [Sim_GBL_005e.Rule.ApplyWhenReturned(EffectTarget.TriggerSubject)])
            {
                EntityKind = EntityKind.Minion, RequireLivingEntity = true
            }
        ]
    };
}
