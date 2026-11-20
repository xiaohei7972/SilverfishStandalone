using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

[CardSim("CS2_105")]
internal static class Sim_CS2_105
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_CS2_105e.Rule.ApplyTo(StatTarget.FriendlyHero)]
    };
}
