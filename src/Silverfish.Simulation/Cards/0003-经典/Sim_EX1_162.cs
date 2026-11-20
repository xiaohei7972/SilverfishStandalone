using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 恐狼前锋
// Dire Wolf Alpha
// 相邻的随从拥有+1攻击力。
// Adjacent minions have +1_Attack.
[CardSim("EX1_162")]
internal static class Sim_EX1_162
{
    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_EX1_162o.Rule.GrantTo(AuraRecipients.AdjacentFriendlyMinions)]
    };
}
