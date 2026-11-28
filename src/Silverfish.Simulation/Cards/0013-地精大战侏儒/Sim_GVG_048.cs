using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 金刚刃牙兽
// Metaltooth Leaper
// 战吼：使你的其他机械获得+2攻击力。
// Battlecry: Give your other Mechs +2 Attack.
[CardSim("GVG_048")]
internal static class Sim_GVG_048
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EnchantStatsEffect(StatTarget.OtherFriendlyMinions, new(StatModifierMode.Add, 2))
            {
                CardFilter = new CardFilter { MinionType = MinionType.Mech },
                EnchantmentCardId = Sim_GVG_048e.CardId
            }
        ]
    };
}
