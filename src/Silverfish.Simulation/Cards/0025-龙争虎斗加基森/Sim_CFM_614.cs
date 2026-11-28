using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 玉莲印记
// Mark of the Lotus
// 使你所有的随从获得+1/+1。
// Give your minions +1/+1.
[CardSim("CFM_614")]
internal static class Sim_CFM_614
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EnchantStatsEffect(StatTarget.FriendlyMinions, new(StatModifierMode.Add, 1, 1))
                { EnchantmentCardId = Sim_CFM_614e.CardId },
        ],
    };
}
