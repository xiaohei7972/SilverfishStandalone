using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 破坏者
// Saboteur
// 战吼：下个回合敌方英雄技能的法力值消耗增加（5）点。
// Battlecry: Your opponent's Hero Power costs (5) more next turn.
[CardSim("AT_086")]
internal static class Sim_AT_086
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new AddTemporaryCostAuraEffect(EntityKind.HeroPower, CostModifierMode.Add, 5, CardRecipient.Opponent, NextTurn: true, EnchantmentCardId: Sim_AT_086e.CardId),
        ],
    };
}
