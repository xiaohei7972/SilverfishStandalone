using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 暴怒的狼人
// Raging Worgen
// 受伤时拥有+1攻击力和风怒。
// Has +1 Attack and Windfury while damaged.
[CardSim("EX1_412")]
internal static class Sim_EX1_412
{
    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_EX1_412e.Rule.GrantTo(AuraRecipients.Self, AuraCondition.WhileDamaged)]
    };
}
