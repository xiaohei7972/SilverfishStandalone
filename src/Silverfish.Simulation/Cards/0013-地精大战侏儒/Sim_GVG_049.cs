using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 加兹瑞拉
// Gahz'rilla
// 每当本随从受到伤害，其攻击力翻倍。
// Whenever this minion takes damage, double its Attack.
[CardSim("GVG_049")]
internal static class Sim_GVG_049
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.Damage,
                TriggerRelation.Friendly,
                [new DynamicStatChangeEffect(StatTarget.Source, DynamicStatMode.DoubleAttack, Sim_GVG_049e.CardId)])
            {
                EntityMatch = TriggerEntityMatch.Self
            }
        ]
    };
}
