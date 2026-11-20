using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_162
{
    internal const string CardId = "EX1_162";

    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_EX1_162o.Rule.GrantTo(AuraRecipients.AdjacentFriendlyMinions)]
    };
}
