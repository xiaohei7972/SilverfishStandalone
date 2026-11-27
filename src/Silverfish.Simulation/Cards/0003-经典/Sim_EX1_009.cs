using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 愤怒的小鸡
// Angry Chicken
// 受伤时拥有+5攻 击力。
// Has +5 Attack while damaged.
[CardSim("EX1_009")]
internal static class Sim_EX1_009
{
    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_EX1_009e.Rule.GrantTo(AuraRecipients.Self, AuraCondition.WhileDamaged)]
    };
}
