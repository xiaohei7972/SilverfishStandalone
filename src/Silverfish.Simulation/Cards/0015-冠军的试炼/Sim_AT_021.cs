using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 小鬼骑士
// Tiny Knight of Evil
// 每当你弃掉一张牌时，便获得+2/+1。
// Whenever you discard a card, gain +2/+1.
[CardSim("AT_021")]
internal static class Sim_AT_021
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.CardDiscarded, TriggerRelation.Friendly, [Sim_AT_021e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
