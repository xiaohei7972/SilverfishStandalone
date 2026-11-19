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
        },
        new SimulationCard(new("CS2_122", EntityKind.Minion, 3, 2, 3))
        {
            Auras = [new(AuraRecipients.OtherFriendlyMinions, new(StatModifierMode.Add, Attack: 1))]
        },
        new SimulationCard(new("CS2_222", EntityKind.Minion, 7, 7, 7))
        {
            Auras = [new(AuraRecipients.OtherFriendlyMinions, new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1))]
        },
        new SimulationCard(new("EX1_162", EntityKind.Minion, 2, 2, 2))
        {
            Auras = [new(AuraRecipients.AdjacentFriendlyMinions, new(StatModifierMode.Add, Attack: 1))]
        },
        new SimulationCard(new("CS2_092", EntityKind.Spell, 4))
        {
            Target = TargetRequirement.Minion,
            OnPlay = [new EnchantStatsEffect(StatTarget.Selected, new(StatModifierMode.Add, Attack: 4, MaximumHealth: 4))]
        },
        new SimulationCard(new("CS2_046", EntityKind.Spell, 5))
        {
            OnPlay = [new EnchantStatsEffect(StatTarget.FriendlyMinions, new(StatModifierMode.Add, Attack: 3),
                EnchantmentDuration.CurrentTurn)]
        },
        new SimulationCard(new("EX1_360", EntityKind.Spell, 1))
        {
            Target = TargetRequirement.Minion,
            OnPlay = [new EnchantStatsEffect(StatTarget.Selected, new(StatModifierMode.Set, Attack: 1))]
        },
        new SimulationCard(new("EX1_619", EntityKind.Spell, 2))
        {
            OnPlay = [new EnchantStatsEffect(StatTarget.AllMinions, new(StatModifierMode.Set, MaximumHealth: 1))]
        },
        new SimulationCard(new("EX1_019", EntityKind.Minion, 3, 3, 2))
        {
            Target = TargetRequirement.FriendlyMinion,
            OnPlay = [new EnchantStatsEffect(StatTarget.Selected, new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1))]
        },
        new SimulationCard(new("EX1_332", EntityKind.Spell, 0))
        {
            Target = TargetRequirement.Minion,
            OnPlay = [new SilenceEffect()]
        },
        new SimulationCard(new("EX1_245", EntityKind.Spell, 1))
        {
            Target = TargetRequirement.Minion,
            OnPlay = [new SilenceEffect(), new DamageEffect(DamageTarget.Selected, 1)]
        },
        new SimulationCard(new("CFM_851", EntityKind.Minion, 4, 3, 3))
        {
            Triggers =
            [
                new(TriggerTiming.CardDrawn, TriggerRelation.Opponent,
                    [new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1))])
            ]
        }
    ];
}
