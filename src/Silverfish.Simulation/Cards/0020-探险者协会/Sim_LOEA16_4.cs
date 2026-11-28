namespace Silverfish.Simulation.Cards;

// 恐怖丧钟
// Timepiece of Horror
// 造成$10点伤害，随机分配到所有敌人身上。
// Deal $10 damage randomly split among all enemies.
[CardSim("LOEA16_4")]
internal static class Sim_LOEA16_4
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 1, 10),
        ],
    };
}
