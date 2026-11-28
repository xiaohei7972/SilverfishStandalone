using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 团队领袖
// Raid Leader
// 你的其他随从拥有+1攻击力。
// Your other minions have +1 Attack.
[CardSim("CS2_122")]
internal static class Sim_CS2_122
{
    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_CS2_122e.Rule.GrantTo(AuraRecipients.OtherFriendlyMinions)]
    };
}
