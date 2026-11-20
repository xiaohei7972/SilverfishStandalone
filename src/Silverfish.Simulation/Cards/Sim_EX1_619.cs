using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_619
{
    internal const string CardId = "EX1_619";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_EX1_619e.Rule.ApplyTo(StatTarget.AllMinions)]
    };
}
