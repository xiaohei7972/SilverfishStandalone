using Silverfish.Domain.Client;
using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 击剑教头
// Fencing Coach
// 战吼：你的下一个英雄技能的法力值消耗减少（2）点。
// Battlecry: The next time you use your Hero Power, it costs (2) less.
[CardSim("AT_115")]
internal static class Sim_AT_115
{
    internal static PlayReqSet PlayReqs { get; } = new([], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new AddTemporaryCostAuraEffect(EntityKind.HeroPower, CostModifierMode.Add, -2, ConsumedOnPlay: true, EnchantmentCardId: Sim_AT_115e.CardId, Persistent: true),
        ],
    };
}
