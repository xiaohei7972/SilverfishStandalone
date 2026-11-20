using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

internal static class Sim_GIL_803
{
    internal const string CardId = "GIL_803";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_GIL_803e.Rule.ApplyTo(StatTarget.Source)]
    };
}
