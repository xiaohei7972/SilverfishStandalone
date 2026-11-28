using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 血蹄勇士
// Bloodhoof Brave
// 嘲讽 受伤时拥有+3攻 击力。
// Taunt Has +3 Attack while damaged.
[CardSim("OG_218")]
internal static class Sim_OG_218
{
    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_OG_218e.Rule.GrantTo(AuraRecipients.Self, AuraCondition.WhileDamaged)],
    };
}
