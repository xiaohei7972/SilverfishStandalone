using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 洛欧塞布
// Loatheb
// 战吼：下个回合敌方法术的法力值消耗增加（5）点。
// Battlecry: Enemy spells cost (5) more next turn.
[CardSim("FP1_030")]
internal static class Sim_FP1_030
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new AddTemporaryCostAuraEffect(
                EntityKind.Spell,
                CostModifierMode.Add,
                5,
                Recipient: CardRecipient.Opponent,
                NextTurn: true,
                EnchantmentCardId: Sim_FP1_030e.CardId)
        ]
    };
}
