using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 格罗玛什·地狱咆哮
// Grommash Hellscream
// 冲锋 受伤时拥有+6攻 击力。
// Charge Has +6 Attack while damaged.
[CardSim("EX1_414")]
internal static class Sim_EX1_414
{
    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_EX1_414e.Rule.GrantTo(AuraRecipients.Self, AuraCondition.WhileDamaged)]
    };
}
