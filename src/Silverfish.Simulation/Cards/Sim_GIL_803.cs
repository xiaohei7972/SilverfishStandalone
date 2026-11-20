using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

[CardSim("GIL_803")]
internal static class Sim_GIL_803
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_GIL_803e.Rule.ApplyTo(StatTarget.Source)]
    };
}
