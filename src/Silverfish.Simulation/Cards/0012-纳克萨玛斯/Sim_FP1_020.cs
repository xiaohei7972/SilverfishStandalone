using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 复仇
// Avenge
// 奥秘：当你的随从死亡时，随机使一个友方随从获得+3/+2。
// Secret: When one of your minions dies, give a random friendly minion +3/+2.
[CardSim("FP1_020")]
internal static class Sim_FP1_020
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(
                TriggerTiming.MinionDied,
                TriggerRelation.Friendly,
                [Sim_FP1_020e.Rule.ApplyTo(StatTarget.RandomOtherFriendlyMinion)])
            {
                Condition = new ZoneCountCondition(
                    ConditionPlayer.Friendly,
                    GameZone.Play,
                    NumberComparison.AtLeast,
                    1,
                    EntityKind.Minion)
            }
        ]
    };
}
