using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

[CardSim("CS2_222")]
internal static class Sim_CS2_222
{
    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_CS2_222o.Rule.GrantTo(AuraRecipients.OtherFriendlyMinions)]
    };
}
