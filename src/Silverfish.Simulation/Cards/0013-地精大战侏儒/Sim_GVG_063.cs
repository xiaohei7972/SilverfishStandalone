using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 伯瓦尔·弗塔根
// Bolvar Fordragon
// 如果这张牌在你的手牌中，每当一个友方随从死亡，便获得+1攻击力。
// Whenever a friendly minion dies while this is in your hand, gain +1 Attack.
[CardSim("GVG_063")]
internal static class Sim_GVG_063
{
    internal static CardRules Rules { get; } = new()
    {
        InHandTriggers =
        [
            new(
                TriggerTiming.MinionDied,
                TriggerRelation.Friendly,
                [
                    new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, 1))
                    {
                        EnchantmentCardId = Sim_GVG_063a.CardId
                    }
                ])
        ]
    };
}
