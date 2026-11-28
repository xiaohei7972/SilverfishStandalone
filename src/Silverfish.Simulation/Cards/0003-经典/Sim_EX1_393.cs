using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 阿曼尼狂战士
// Amani Berserker
// 受伤时拥有+3攻 击力。
// Has +3 Attack while damaged.
[CardSim("EX1_393")]
internal static class Sim_EX1_393
{
    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_EX1_393e.Rule.GrantTo(AuraRecipients.Self, AuraCondition.WhileDamaged)]
    };
}
