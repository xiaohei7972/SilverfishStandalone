namespace Silverfish.Simulation.Cards;

// 复仇之怒
// Avenging Wrath
// 造成$8点伤害，随机分配到所有敌人身上。
// Deal $8 damage randomly split among all enemies.
[CardSim("EX1_384")]
internal static class Sim_EX1_384
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 1, Count: 8)]
    };
}
