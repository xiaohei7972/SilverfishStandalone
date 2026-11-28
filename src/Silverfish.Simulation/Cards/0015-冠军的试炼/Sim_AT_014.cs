using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 暗影魔
// Shadowfiend
// 每当你抽一张牌时，使其法力值消耗减少（1）点。
// Whenever you draw a card, reduce its Cost by (1).
[CardSim("AT_014")]
internal static class Sim_AT_014
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.CardDrawn, TriggerRelation.Friendly, [new ChangeHandCostEffect(HandCostTarget.DrawnCard, -1) { EnchantmentCardId = Sim_AT_014e.CardId }]),
        ],
    };
}
