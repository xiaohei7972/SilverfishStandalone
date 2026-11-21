using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 爆炸陷阱
// Explosive Trap
// 奥秘：当你的英雄受到攻击，对所有敌人造成$2点伤害。
// Secret: When your hero is attacked, deal $2 damage to all enemies.
[CardSim("EX1_610")]
internal static class Sim_EX1_610
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AttackProposed, TriggerRelation.Friendly, [new DamageEffect(DamageTarget.AllEnemies, 2)])
            {
                Focus = TriggerFocus.AttackDefender, EntityKind = EntityKind.Hero
            }
        ]
    };
}
