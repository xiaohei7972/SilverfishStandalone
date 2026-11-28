using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 蛛魔先知
// Nerubian Prophet
// 在你的回合开始时，本牌的法力值消耗减少（1）点。
// At the start of your turn, reduce this card's Cost by (1).
[CardSim("OG_138")]
internal static class Sim_OG_138
{
    internal static CardRules Rules { get; } = new()
    {
        InHandTriggers =
        [
            new(TriggerTiming.TurnStarted, TriggerRelation.Friendly, [new ChangeHandCostEffect(HandCostTarget.Source, -1) { EnchantmentCardId = Sim_OG_138e.CardId }]),
        ],
    };
}
