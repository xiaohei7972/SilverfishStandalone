using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

internal static class Sim_NEW1_033
{
    internal const string CardId = "NEW1_033";

    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_NEW1_033o.Rule.GrantTo(AuraRecipients.OtherFriendlyMinions)]
    };
}
