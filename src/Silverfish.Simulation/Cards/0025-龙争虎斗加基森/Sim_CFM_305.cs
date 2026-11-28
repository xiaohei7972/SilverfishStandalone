using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 风驰电掣
// Smuggler's Run
// 使你手牌中的所有随从牌获得+1/+1。
// Give all minions in your hand +1/+1.
[CardSim("CFM_305")]
internal static class Sim_CFM_305
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EnchantStatsEffect(StatTarget.FriendlyHand, new(StatModifierMode.Add, 1, 1))
                { EnchantmentCardId = Sim_CFM_305e.CardId },
        ],
    };
}
