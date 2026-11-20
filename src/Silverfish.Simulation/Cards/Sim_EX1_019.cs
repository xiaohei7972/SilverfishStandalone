using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_019
{
    internal const string CardId = "EX1_019";

    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.FriendlyMinion,
        OnPlay = [Sim_EX1_019e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
