using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 蒸发
// Vaporize
// 奥秘：当一个随从攻击你的英雄，将其消灭。
// Secret: When a minion attacks your hero, destroy it.
[CardSim("EX1_594")]
internal static class Sim_EX1_594
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AttackProposed, TriggerRelation.Friendly, [new DestroyMinionEffect(EffectTarget.TriggerSubject)])
            {
                Focus = TriggerFocus.AttackDefender,
                EntityKind = EntityKind.Hero,
                AttackingKind = EntityKind.Minion
            }
        ]
    };
}
