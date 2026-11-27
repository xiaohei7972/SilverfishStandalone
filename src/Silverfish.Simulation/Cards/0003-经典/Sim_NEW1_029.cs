using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 米尔豪斯·法力风暴
// Millhouse Manastorm
// 战吼：下个回合敌方法术的法力值消耗为（0）点。
// Battlecry: Enemy spells cost (0) next turn.
[CardSim("NEW1_029")]
internal static class Sim_NEW1_029
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new AddTemporaryCostAuraEffect(EntityKind.Spell, CostModifierMode.Set, 0,
                Recipient: CardRecipient.Opponent, NextTurn: true, EnchantmentCardId: Sim_NEW1_029t.CardId)
        ]
    };
}
