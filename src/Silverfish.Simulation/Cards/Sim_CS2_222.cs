using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

internal static class Sim_CS2_222
{
    internal const string CardId = "CS2_222";

    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_CS2_222o.Rule.GrantTo(AuraRecipients.OtherFriendlyMinions)]
    };
}
