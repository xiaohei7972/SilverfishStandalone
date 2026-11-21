namespace Silverfish.Simulation.Cards;

// 水元素
// Water Elemental
// 冻结任何受到本随从伤害的角色。
// Freeze any character damaged by this minion.
[CardSim("CS2_033")]
internal static class Sim_CS2_033
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.Damage, TriggerRelation.Friendly, [new FreezeEffect(FreezeTarget.TriggerSubject)])
        {
            Focus = TriggerFocus.DamageSource, EntityMatch = TriggerEntityMatch.Self
        }]
    };
}
