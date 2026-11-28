using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 人气选手
// Crowd Favorite
// 每当你使用一张具有战吼的牌，便获得+1/+1。
// Whenever you play a card with Battlecry, gain +1/+1.
[CardSim("AT_121")]
internal static class Sim_AT_121
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterCardPlayed, TriggerRelation.Friendly, [Sim_AT_121e.Rule.ApplyTo(StatTarget.Source)]) { CardFilter = new CardFilter { HasBattlecry = true } },
        ],
    };
}
