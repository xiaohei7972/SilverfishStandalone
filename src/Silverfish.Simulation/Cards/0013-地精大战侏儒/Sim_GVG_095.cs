using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 地精工兵
// Goblin Sapper
// 如果你对手的手牌数量大于或等于6张，便拥有+4攻击力。
// Has +4 Attack while your opponent has 6 or more cards in hand.
[CardSim("GVG_095")]
internal static class Sim_GVG_095
{
    internal static CardRules Rules { get; } = new()
    {
        Auras =
        [
            new StatAura(AuraRecipients.Self, new(StatModifierMode.Add, 4))
            {
                Condition = AuraCondition.WhileOpponentHasAtLeastSixHandCards
            }
        ]
    };
}
