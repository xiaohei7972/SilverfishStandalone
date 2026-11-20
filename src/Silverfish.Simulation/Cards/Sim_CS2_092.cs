using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

internal static class Sim_CS2_092
{
    internal const string CardId = "CS2_092";

    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Minion,
        OnPlay = [Sim_CS2_092e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
