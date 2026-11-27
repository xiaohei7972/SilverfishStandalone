using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 齿轮大师
// Cogmaster
// 如果你控制任何机械，便拥有 +2攻击力。
// Has +2 Attack while you have a Mech.
[CardSim("GVG_013")]
internal static class Sim_GVG_013
{
    internal static CardRules Rules { get; } = new()
    {
        Auras =
        [
            new StatAura(AuraRecipients.Self, new(StatModifierMode.Add, Attack: 2))
            {
                Condition = AuraCondition.WhileControllingMech
            }
        ]
    };
}
