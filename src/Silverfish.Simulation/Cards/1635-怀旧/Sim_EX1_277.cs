namespace Silverfish.Simulation.Cards;

// 奥术飞弹
// Arcane Missiles
// 造成$3点伤害，随机分配到所有敌人身上。
// Deal $3 damage randomly split among all enemies.
[CardSim("EX1_277")]
internal static class Sim_EX1_277
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 1, Count: 3)]
    };
}
