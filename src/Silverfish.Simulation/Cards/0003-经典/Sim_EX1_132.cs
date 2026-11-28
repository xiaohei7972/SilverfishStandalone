using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 以眼还眼
// Eye for an Eye
// 奥秘： 当你的英雄受到伤害时，对敌方英雄造成等量伤害。
// Secret: When your hero takes damage, deal_that much damage to the enemy hero.
[CardSim("EX1_132")]
internal static class Sim_EX1_132
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.Damage, TriggerRelation.Friendly, [new EventAmountDamageEffect(DamageTarget.EnemyHero)])
            {
                EntityKind = EntityKind.Hero
            }
        ]
    };
}
