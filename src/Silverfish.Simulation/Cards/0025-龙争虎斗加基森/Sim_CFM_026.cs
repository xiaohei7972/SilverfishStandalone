using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 军备宝箱
// Hidden Cache
// 奥秘：在你的对手使用一张随从牌后，随机使你手牌中的一张随从牌获得+2/+2。
// Secret: After your opponent plays a minion, give a random minion in your hand +2/+2.
[CardSim("CFM_026")]
internal static class Sim_CFM_026
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AfterMinionPlayed, TriggerRelation.Opponent,
                [new EnchantStatsEffect(StatTarget.RandomFriendlyHandMinion, new(StatModifierMode.Add, 2, 2))
                    { EnchantmentCardId = Sim_CFM_026e.CardId }]),
        ],
    };
}
