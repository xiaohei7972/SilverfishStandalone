using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_554
{
    internal const string CardId = "EX1_554";

    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AttackProposed, TriggerRelation.Friendly,
                [new SummonEffect("EX1_554t"), new SummonEffect("EX1_554t"), new SummonEffect("EX1_554t")])
            {
                Focus = TriggerFocus.AttackDefender, EntityKind = EntityKind.Minion, RequiresBoardSpace = true
            }
        ]
    };
}
