namespace Silverfish.Simulation.Cards;

// 掷斧者
// Axe Flinger
// 每当本随从受到伤害，对敌方英雄造成2点伤害。
// Whenever this minion takes damage, deal 2 damage to the enemy hero.
[CardSim("BRM_016")]
internal static class Sim_BRM_016
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.Damage,
                TriggerRelation.Friendly,
                [new DamageEffect(DamageTarget.EnemyHero, 2)])
            {
                EntityMatch = TriggerEntityMatch.Self
            }
        ]
    };
}
