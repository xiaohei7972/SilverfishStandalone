using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 漂浮观察者
// Floating Watcher
// 每当你的英雄在你的回合受到伤害，便获得+2/+2。
// Whenever your hero takes damage on your turn, gain +2/+2.
[CardSim("GVG_100")]
internal static class Sim_GVG_100
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.Damage,
                TriggerRelation.Friendly,
                [
                    new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, 2, 2))
                    {
                        EnchantmentCardId = Sim_GVG_100e.CardId
                    }
                ])
            {
                EntityKind = EntityKind.Hero,
                ControllerTurnOnly = true
            }
        ]
    };
}
