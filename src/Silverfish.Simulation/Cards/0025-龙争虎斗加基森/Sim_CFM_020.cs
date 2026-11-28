using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 缚链者拉兹
// Raza the Chained
// 战吼：如果你的牌库里没有相同的牌，则在本局对战中，你的英雄技能的法力值消耗为（0）点。
// Battlecry: If your deck has no duplicates, your Hero Power costs (0) this game.
[CardSim("CFM_020")]
internal static class Sim_CFM_020
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new NoDuplicatesInDeckCondition(),
                [new AddTemporaryCostAuraEffect(EntityKind.HeroPower, CostModifierMode.Set, 0,
                    EnchantmentCardId: Sim_CFM_020e.CardId, Persistent: true)]),
        ],
    };
}
