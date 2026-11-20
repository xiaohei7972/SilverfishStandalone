using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_610
{
    internal const string CardId = "EX1_610";

    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AttackProposed, TriggerRelation.Friendly, [new DamageEffect(DamageTarget.AllEnemies, 2)])
            {
                Focus = TriggerFocus.AttackDefender, EntityKind = EntityKind.Hero
            }
        ]
    };
}
