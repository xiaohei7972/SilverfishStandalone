using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 梦魇之龙
// Scaled Nightmare
// 在你的回合开始时，本随从的攻击力 翻倍。
// At the start of your turn, double this minion's Attack.
[CardSim("OG_271")]
internal static class Sim_OG_271
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnStarted, TriggerRelation.Friendly, [new DynamicStatChangeEffect(StatTarget.Source, DynamicStatMode.DoubleAttack, Sim_OG_271e.CardId)]),
        ],
    };
}
