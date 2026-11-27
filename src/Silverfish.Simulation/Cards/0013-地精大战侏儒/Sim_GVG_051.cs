using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 战斗机器人
// Warbot
// 受伤时拥有+1攻 击力。
// Has +1 Attack while damaged.
[CardSim("GVG_051")]
internal static class Sim_GVG_051
{
    internal static CardRules Rules { get; } = new()
    {
        Auras =
        [
            new StatAura(AuraRecipients.Self, new(StatModifierMode.Add, 1))
            {
                Condition = AuraCondition.WhileDamaged,
                EnchantmentCardId = Sim_GVG_051e.CardId
            }
        ]
    };
}
