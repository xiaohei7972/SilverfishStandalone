using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 南海船长
// Southsea Captain
// 你的其他海盗拥有+1/+1。
// Your other Pirates have +1/+1.
[CardSim("NEW1_027")]
internal static class Sim_NEW1_027
{
    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_NEW1_027e.Rule.GrantTo(AuraRecipients.OtherFriendlyMinions, cardFilter: new() { MinionType = MinionType.Pirate })]
    };
}
