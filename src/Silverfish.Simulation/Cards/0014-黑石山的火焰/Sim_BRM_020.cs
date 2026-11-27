using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 龙人巫师
// Dragonkin Sorcerer
// 每当你以本随从为目标施放一个法术时，获得+1/+1。
// Whenever you target this minion with a spell, gain +1/+1.
[CardSim("BRM_020")]
internal static class Sim_BRM_020
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.TargetedBySpell,
                TriggerRelation.Friendly,
                [Sim_BRM_020e.Rule.ApplyTo(StatTarget.Source)])
            {
                EntityMatch = TriggerEntityMatch.Self
            }
        ]
    };
}
