using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 军需官
// Quartermaster
// 战吼：使你的白银之手新兵获得+2/+2。
// Battlecry: Give your Silver Hand Recruits +2/+2.
[CardSim("GVG_060")]
internal static class Sim_GVG_060
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EnchantStatsEffect(StatTarget.FriendlyMinions, new(StatModifierMode.Add, 2, 2))
            {
                CardFilter = new CardFilter { CardId = "CS2_101t" },
                EnchantmentCardId = Sim_GVG_060e.CardId
            }
        ]
    };
}
