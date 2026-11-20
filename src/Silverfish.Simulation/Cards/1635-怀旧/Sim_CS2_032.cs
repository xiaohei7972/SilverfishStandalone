namespace Silverfish.Simulation.Cards;

// 烈焰风暴
// Flamestrike
// 对所有敌方随从造成$5点伤害。
// Deal $5 damage to all enemy minions.
[CardSim("CS2_032")]
internal static class Sim_CS2_032
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 5)]
    };
}
