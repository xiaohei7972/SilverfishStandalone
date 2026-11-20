using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_289
{
    internal const string CardId = "EX1_289";

    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AttackProposed, TriggerRelation.Friendly, [new GainArmorEffect(8)])
            {
                Focus = TriggerFocus.AttackDefender, EntityKind = EntityKind.Hero
            }
        ]
    };
}
