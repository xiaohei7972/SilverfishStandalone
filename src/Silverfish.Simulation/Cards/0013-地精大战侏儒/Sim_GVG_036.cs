using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 动力战锤
// Powermace
// 亡语：随机使一个友方机械获得+2/+2。
// Deathrattle: Give a random friendly Mech +2/+2.
[CardSim("GVG_036")]
internal static class Sim_GVG_036
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new EnchantStatsEffect(StatTarget.RandomOtherFriendlyMinion, new(StatModifierMode.Add, 2, 2))
            {
                CardFilter = new CardFilter { MinionType = MinionType.Mech },
                EnchantmentCardId = Sim_GVG_036e.CardId
            }
        ]
    };
}
