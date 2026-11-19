using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

/// <summary>Independently authored implementations validated against the November 2026 card-data baseline.</summary>
public static class StarterCardCatalog
{
    public static ImmutableArray<SimulationCard> Cards { get; } =
    [
        new(new("CS2_231", EntityKind.Minion, 0, 1, 1)),
        new(new("CS2_172", EntityKind.Minion, 2, 3, 2)),
        new SimulationCard(new("CS2_023", EntityKind.Spell, 3))
        {
            OnPlay = [new DrawEffect(2)]
        },
        new SimulationCard(new("CS2_032", EntityKind.Spell, 7))
        {
            OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 5)]
        },
        new SimulationCard(new("EX1_007", EntityKind.Minion, 3, 1, 4))
        {
            Triggers = [new(TriggerTiming.Damage, TriggerRelation.Friendly, [new DrawEffect(1)])
            {
                EntityMatch = TriggerEntityMatch.Self
            }]
        },
        new SimulationCard(new("EX1_402", EntityKind.Minion, 2, 1, 4))
        {
            Triggers = [new(TriggerTiming.Damage, TriggerRelation.Friendly, [new GainArmorEffect(1)])
            {
                EntityKind = EntityKind.Minion
            }]
        }
    ];
}
