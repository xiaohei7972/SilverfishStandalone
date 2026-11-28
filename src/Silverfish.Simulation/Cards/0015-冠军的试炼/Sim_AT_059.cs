using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 神勇弓箭手
// Brave Archer
// 激励：如果你没有其他手牌，则对敌方英雄造成2点伤害。
// Inspire: If your hand is empty, deal 2 damage to the enemy hero.
[CardSim("AT_059")]
internal static class Sim_AT_059
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [new DamageEffect(DamageTarget.EnemyHero, 2)]) { Condition = new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Hand, NumberComparison.Equal, 0) },
        ],
    };
}
