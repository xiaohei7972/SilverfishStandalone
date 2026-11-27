using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 误导
// Misdirection
// 奥秘：当一个敌人攻击你的英雄时，改为该敌人随机攻击另一个角色。
// Secret: When an enemy attacks your hero, instead it attacks another random character.
[CardSim("EX1_533")]
internal static class Sim_EX1_533
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AttackProposed, TriggerRelation.Friendly, [new RedirectAttackToRandomOtherCharacterEffect()])
            {
                Focus = TriggerFocus.AttackDefender,
                EntityKind = EntityKind.Hero
            }
        ]
    };
}
