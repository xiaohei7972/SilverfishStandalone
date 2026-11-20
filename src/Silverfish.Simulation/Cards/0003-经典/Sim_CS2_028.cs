namespace Silverfish.Simulation.Cards;

// 暴风雪
// Blizzard
// 对所有敌方随从造成$2点伤害，并使其冻结。
// Deal $2 damage to all enemy minions and Freeze them.
[CardSim("CS2_028")]
internal static class Sim_CS2_028
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 2), new FreezeEffect(FreezeTarget.EnemyMinions)]
    };
}
