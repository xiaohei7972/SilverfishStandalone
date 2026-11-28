using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 畸变狂战士
// Aberrant Berserker
// 受伤时拥有+2攻 击力。
// Has +2 Attack while damaged.
[CardSim("OG_150")]
internal static class Sim_OG_150
{
    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_OG_150e.Rule.GrantTo(AuraRecipients.Self, AuraCondition.WhileDamaged)],
    };
}
