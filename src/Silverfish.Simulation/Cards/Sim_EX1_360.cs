using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_360
{
    internal const string CardId = "EX1_360";

    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Minion,
        OnPlay = [Sim_EX1_360e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
