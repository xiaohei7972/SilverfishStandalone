using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 虚灵奥术师
// Ethereal Arcanist
// 在你的回合结束时，如果你控制着奥秘，获得+2/+2。
// If you control a Secret at_the end of your turn, gain +2/+2.
[CardSim("EX1_274")]
internal static class Sim_EX1_274
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [Sim_EX1_274e.Rule.ApplyTo(StatTarget.Source)])
            {
                Condition = new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Secret, NumberComparison.AtLeast, 1)
            }
        ]
    };
}
