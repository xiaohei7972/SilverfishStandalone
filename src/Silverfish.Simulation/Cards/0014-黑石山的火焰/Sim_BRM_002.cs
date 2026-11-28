namespace Silverfish.Simulation.Cards;

// 火妖
// Flamewaker
// 在你施放一个法术后，造成2点伤害，随机分配到所有敌人身上。
// After you cast a spell, deal 2 damage randomly split among all enemies.
[CardSim("BRM_002")]
internal static class Sim_BRM_002
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.AfterSpellResolved,
                TriggerRelation.Friendly,
                [new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 1, 2)])
        ]
    };
}
