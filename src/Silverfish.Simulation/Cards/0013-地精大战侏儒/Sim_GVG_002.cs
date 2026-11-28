namespace Silverfish.Simulation.Cards;

// 碎雪机器人
// Snowchugger
// 冻结任何受到本随从伤害的角色。
// Freeze any character damaged by this minion.
[CardSim("GVG_002")]
internal static class Sim_GVG_002
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.Damage,
                TriggerRelation.Any,
                [new FreezeEffect(FreezeTarget.TriggerSubject)])
            {
                Focus = TriggerFocus.DamageSource,
                EntityMatch = TriggerEntityMatch.Self
            }
        ]
    };
}
