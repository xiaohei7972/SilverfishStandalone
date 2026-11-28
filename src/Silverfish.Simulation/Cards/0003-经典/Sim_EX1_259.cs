namespace Silverfish.Simulation.Cards;

// 闪电风暴
// Lightning Storm
// 对所有敌方随从造成$3点伤害，过载：（1）
// Deal $3 damage to all_enemy minions. Overload: (1)
[CardSim("EX1_259")]
internal static class Sim_EX1_259
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 3)]
    };
}
