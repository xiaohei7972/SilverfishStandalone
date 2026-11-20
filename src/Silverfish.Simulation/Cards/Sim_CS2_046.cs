using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

[CardSim("CS2_046")]
internal static class Sim_CS2_046
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_CS2_046e.Rule.ApplyTo(StatTarget.FriendlyMinions)]
    };
}
