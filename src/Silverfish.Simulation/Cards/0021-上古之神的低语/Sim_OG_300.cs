using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 波戈蒙斯塔
// The Boogeymonster
// 每当波戈蒙斯塔攻击并消灭一个随从，便获得+2/+2。
// Whenever this attacks and kills a minion, gain +2/+2.
[CardSim("OG_300")]
internal static class Sim_OG_300
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterAttack, TriggerRelation.Friendly, [Sim_OG_300e.Rule.ApplyTo(StatTarget.Source)]) { EntityMatch = TriggerEntityMatch.Self, KilledMinion = true },
        ],
    };
}
