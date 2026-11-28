using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 上古之神先驱
// Ancient Harbinger
// 在你的回合开始时，将一个法力值消耗为（10）的随从从你的牌库置入你的手牌。
// At the start of your turn, put a 10-Cost minion from your deck into your hand.
[CardSim("OG_290")]
internal static class Sim_OG_290
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnStarted, TriggerRelation.Friendly, [new DrawMatchingEffect(new CardFilter { Kind = EntityKind.Minion, MinimumPrintedCost = 10, MaximumPrintedCost = 10 })]),
        ],
    };
}
