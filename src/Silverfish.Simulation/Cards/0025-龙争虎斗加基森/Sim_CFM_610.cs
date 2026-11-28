using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 魔瘾结晶者
// Crystalweaver
// 战吼：使你的所有恶魔获得+1/+1。
// Battlecry: Give your Demons +1/+1.
[CardSim("CFM_610")]
internal static class Sim_CFM_610
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EnchantStatsEffect(StatTarget.FriendlyMinions, new(StatModifierMode.Add, 1, 1))
            {
                CardFilter = new CardFilter { MinionType = MinionType.Demon },
                EnchantmentCardId = Sim_CFM_610e.CardId,
            },
        ],
    };
}
