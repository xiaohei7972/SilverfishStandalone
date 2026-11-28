namespace Silverfish.Simulation.Cards;

// 暮光烈焰召唤者
// Twilight Flamecaller
// 战吼：对所有敌方随从造成1点伤害。
// Battlecry: Deal 1 damage to all enemy minions.
[CardSim("OG_083")]
internal static class Sim_OG_083
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.EnemyMinions, 1),
        ],
    };
}
