using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 腐化灰熊
// Addled Grizzly
// 在你召唤一只野兽后，使其获得+1/+1。
// After you summon a Beast, give it +1/+1.
[CardSim("OG_313")]
internal static class Sim_OG_313
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterMinionSummoned, TriggerRelation.Friendly, [Sim_OG_313e.Rule.ApplyTo(StatTarget.TriggerSubject)]) { CardFilter = new CardFilter { MinionType = MinionType.Beast } },
        ],
    };
}
