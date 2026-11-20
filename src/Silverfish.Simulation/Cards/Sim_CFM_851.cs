using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

internal static class Sim_CFM_851
{
    internal const string CardId = "CFM_851";

    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.CardDrawn, TriggerRelation.Opponent,
                [Sim_CFM_851e.Rule.ApplyTo(StatTarget.Source)])
        ]
    };
}
