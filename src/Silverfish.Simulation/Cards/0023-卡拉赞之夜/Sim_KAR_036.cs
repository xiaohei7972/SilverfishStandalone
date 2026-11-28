using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 奥术畸体
// Arcane Anomaly
// 在你施放一个法术后，使本随从获得+1生命值。
// After you cast a spell, give this minion +1 Health.
[CardSim("KAR_036")]
internal static class Sim_KAR_036
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterSpellResolved, TriggerRelation.Friendly, [Sim_KAR_036e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
