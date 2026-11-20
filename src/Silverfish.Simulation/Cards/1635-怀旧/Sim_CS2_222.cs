using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 暴风城勇士
// Stormwind Champion
// 你的其他随从拥有+1/+1。
// Your other minions have +1/+1.
[CardSim("CS2_222")]
internal static class Sim_CS2_222
{
    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_CS2_222o.Rule.GrantTo(AuraRecipients.OtherFriendlyMinions)]
    };
}
