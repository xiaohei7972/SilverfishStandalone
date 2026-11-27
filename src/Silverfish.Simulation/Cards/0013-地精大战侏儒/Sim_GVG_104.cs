using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 大胖
// Hobgoblin
// 每当你使用一张攻击力为1的随从牌，便使其获得+2/+2。
// Whenever you play a 1-Attack minion, give it +2/+2.
[CardSim("GVG_104")]
internal static class Sim_GVG_104
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.AfterMinionPlayed,
                TriggerRelation.Friendly,
                [
                    new EnchantStatsEffect(StatTarget.TriggerSubject, new(StatModifierMode.Add, 2, 2))
                    {
                        EnchantmentCardId = Sim_GVG_104a.CardId
                    }
                ])
            {
                ExactPrintedAttack = 1
            }
        ]
    };
}
