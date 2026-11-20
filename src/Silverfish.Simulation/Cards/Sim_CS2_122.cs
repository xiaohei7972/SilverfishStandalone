using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

internal static class Sim_CS2_122
{
    internal const string CardId = "CS2_122";

    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_CS2_122e.Rule.GrantTo(AuraRecipients.OtherFriendlyMinions)]
    };
}
