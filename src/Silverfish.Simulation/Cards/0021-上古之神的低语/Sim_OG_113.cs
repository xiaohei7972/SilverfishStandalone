using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 夜色镇议员
// Darkshire Councilman
// 在你召唤一个随从后，获得+1攻击力。
// After you summon a minion, gain +1 Attack.
[CardSim("OG_113")]
internal static class Sim_OG_113
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterMinionSummoned, TriggerRelation.Friendly, [Sim_OG_113e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
