using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 回收机器人
// Junkbot
// 每当一个友方机械死亡，便获得+2/+2。
// Whenever a friendly Mech dies, gain +2/+2.
[CardSim("GVG_106")]
internal static class Sim_GVG_106
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.MinionDied,
                TriggerRelation.Friendly,
                [
                    new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, 2, 2))
                    {
                        EnchantmentCardId = Sim_GVG_106e.CardId
                    }
                ])
            {
                EntityMatch = TriggerEntityMatch.Other,
                CardFilter = new CardFilter { MinionType = MinionType.Mech }
            }
        ]
    };
}
