using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 龙王配偶
// Dragon Consort
// 战吼：你的下一张龙牌的法力值消耗减少（2）点。
// Battlecry: The next Dragon you play costs (2) less.
[CardSim("BRM_018")]
internal static class Sim_BRM_018
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new AddTemporaryCostAuraEffect(
                EntityKind.Minion,
                CostModifierMode.Add,
                -2,
                ConsumedOnPlay: true,
                EnchantmentCardId: Sim_BRM_018e.CardId,
                MinionType: MinionType.Dragon,
                Persistent: true)
        ]
    };
}
