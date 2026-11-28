using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 重型刃弩
// Glaivezooka
// 战吼：随机使一个友方随从获得+1攻击力。
// Battlecry: Give a random friendly minion +1 Attack.
[CardSim("GVG_043")]
internal static class Sim_GVG_043
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EnchantStatsEffect(StatTarget.RandomOtherFriendlyMinion, new(StatModifierMode.Add, 1))
            {
                EnchantmentCardId = Sim_GVG_043e.CardId
            }
        ]
    };
}
