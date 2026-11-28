namespace Silverfish.Simulation.Cards;

// 阴暗渔夫纳特
// Nat, the Darkfisher
// 你的对手在回合开始时，有50%的几率额外抽一张牌。
// At the start of your opponent's turn, they have a 50% chance to draw an extra card.
[CardSim("OG_338")]
internal static class Sim_OG_338
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnStarted, TriggerRelation.Opponent, [new RandomChanceEffect(1, 2, [new DrawEffect(1, CardRecipient.Opponent)])]),
        ],
    };
}
