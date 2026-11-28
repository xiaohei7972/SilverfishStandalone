namespace Silverfish.Simulation.Cards;

// 黑铁潜藏者
// Dark Iron Skulker
// 战吼：对所有未受伤的敌方随从造成2点伤害。
// Battlecry: Deal 2 damage to all undamaged enemy minions.
[CardSim("BRM_008")]
internal static class Sim_BRM_008
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.EnemyMinions, 2)
            {
                UndamagedOnly = true
            }
        ]
    };
}
