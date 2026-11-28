namespace Silverfish.Simulation.Cards;

// 大漩涡传送门
// Maelstrom Portal
// 对所有敌方随从造成$1点伤害。随机召唤一个法力值消耗为（1）的随从。
// Deal_$1_damage to_all_enemy_minions. Summon_a_random 1-Cost minion.
[CardSim("KAR_073")]
internal static class Sim_KAR_073
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 1), new SummonMatchingMinionEffect(new CardFilter { MinimumPrintedCost = 1, MaximumPrintedCost = 1 })],
    };
}
