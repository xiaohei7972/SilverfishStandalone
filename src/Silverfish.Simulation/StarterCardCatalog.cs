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
        },
        new(new("CS2_106", EntityKind.Weapon, 2, 3, PrintedDurability: 2)),
        new(new("CS2_091", EntityKind.Weapon, 1, 1, PrintedDurability: 4)),
        new(new("CS2_112", EntityKind.Weapon, 5, 5, PrintedDurability: 2)),
        new(new("CS2_082", EntityKind.Weapon, 1, 1, PrintedDurability: 2)),
        new SimulationCard(new("CS2_097", EntityKind.Weapon, 4, 4, PrintedDurability: 2))
        {
            Triggers = [new(TriggerTiming.HeroAttacking, TriggerRelation.Friendly, [new HealFriendlyHeroEffect(3)])]
        },
        new SimulationCard(new("FP1_021", EntityKind.Weapon, 4, 4, PrintedDurability: 2))
        {
            Deathrattle = [new DamageEffect(DamageTarget.AllMinions, 1)]
        },
        new SimulationCard(new("EX1_066", EntityKind.Minion, 2, 3, 2))
        {
            OnPlay = [new DestroyEnemyWeaponEffect()]
        },
        new SimulationCard(new("HERO_01bp", EntityKind.HeroPower, 2))
        {
            OnPlay = [new GainArmorEffect(2)]
        },
        new SimulationCard(new("HERO_03bp", EntityKind.HeroPower, 2))
        {
            OnPlay = [new EquipWeaponEffect("CS2_082")]
        },
        new SimulationCard(new("HERO_08bp", EntityKind.HeroPower, 2))
        {
            Target = TargetRequirement.Character,
            OnPlay = [new DamageEffect(DamageTarget.Selected, 1)]
        },
        new SimulationCard(new("HERO_06bp", EntityKind.HeroPower, 2))
        {
            OnPlay = [new EnchantStatsEffect(StatTarget.FriendlyHero, new(StatModifierMode.Add, Attack: 1),
                EnchantmentDuration.CurrentTurn), new GainArmorEffect(1)]
        },
        new SimulationCard(new("CS2_105", EntityKind.Spell, 2))
        {
            OnPlay = [new EnchantStatsEffect(StatTarget.FriendlyHero, new(StatModifierMode.Add, Attack: 4),
                EnchantmentDuration.CurrentTurn)]
        },
        new SimulationCard(new("EX1_581", EntityKind.Spell, 2))
        {
            Target = TargetRequirement.EnemyMinion,
            OnPlay = [new ReturnMinionEffect()]
        },
        new SimulationCard(new("EX1_144", EntityKind.Spell, 0))
        {
            Target = TargetRequirement.FriendlyMinion,
            OnPlay = [new ReturnMinionEffect(CostDelta: -2)]
        },
        new SimulationCard(new("EX1_049", EntityKind.Minion, 2, 3, 2))
        {
            Target = TargetRequirement.FriendlyMinion,
            OnPlay = [new ReturnMinionEffect()]
        },
        new SimulationCard(new("CS2_022", EntityKind.Spell, 4))
        {
            Target = TargetRequirement.Minion,
            OnPlay = [new TransformMinionEffect("CS2_tk1")]
        },
        new(new("CS2_tk1", EntityKind.Minion, 1, 1, 1)),
        new SimulationCard(new("EX1_246", EntityKind.Spell, 3))
        {
            Target = TargetRequirement.Minion,
            OnPlay = [new TransformMinionEffect("hexfrog")]
        },
        new SimulationCard(new("hexfrog", EntityKind.Minion, 0, 0, 1)) { Keywords = CardKeywords.Taunt },
        new SimulationCard(new("CS2_076", EntityKind.Spell, 4))
        {
            Target = TargetRequirement.EnemyMinion,
            OnPlay = [new DestroyMinionEffect()]
        },
        new SimulationCard(new("CS1_113", EntityKind.Spell, 9))
        {
            Target = TargetRequirement.EnemyMinion,
            OnPlay = [new TakeControlEffect()]
        },
        new SimulationCard(new("BRM_017", EntityKind.Spell, 2))
        {
            Requirements = PlayRequirements.BoardSpace | PlayRequirements.FriendlyMinionDeath,
            OnPlay = [new ResurrectFriendlyMinionEffect()]
        },
        new SimulationCard(new("CS2_024", EntityKind.Spell, 2))
        {
            Target = TargetRequirement.Character,
            OnPlay = [new DamageEffect(DamageTarget.Selected, 3), new FreezeEffect(FreezeTarget.Selected)]
        },
        new SimulationCard(new("CS2_026", EntityKind.Spell, 3))
        {
            OnPlay = [new FreezeEffect(FreezeTarget.EnemyMinions)]
        },
        new SimulationCard(new("CS2_028", EntityKind.Spell, 6))
        {
            OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 2), new FreezeEffect(FreezeTarget.EnemyMinions)]
        },
        new SimulationCard(new("UNG_205", EntityKind.Minion, 1, 2, 1))
        {
            Target = TargetRequirement.EnemyCharacter,
            OnPlay = [new FreezeEffect(FreezeTarget.Selected)]
        },
        new SimulationCard(new("CS2_033", EntityKind.Minion, 4, 3, 6))
        {
            Triggers = [new(TriggerTiming.Damage, TriggerRelation.Friendly, [new FreezeEffect(FreezeTarget.TriggerSubject)])
            {
                Focus = TriggerFocus.DamageSource, EntityMatch = TriggerEntityMatch.Self
            }]
        },
        new SimulationCard(new("GIL_803", EntityKind.Minion, 4, 2, 5))
        {
            Keywords = CardKeywords.Rush,
            OnPlay = [new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, Attack: 3), EnchantmentDuration.CurrentTurn)]
        },
        new SimulationCard(new("CS2_124", EntityKind.Minion, 3, 3, 1)) { Keywords = CardKeywords.Charge },
        new SimulationCard(new("CS2_169", EntityKind.Minion, 1, 1, 1)) { Keywords = CardKeywords.Windfury },
        new SimulationCard(new("EX1_033", EntityKind.Minion, 6, 4, 5)) { Keywords = CardKeywords.Windfury },
        new SimulationCard(new("EX1_567", EntityKind.Weapon, 5, 2, PrintedDurability: 8))
        {
            Keywords = CardKeywords.Windfury, Overload = 2
        },
        new SimulationCard(new("EX1_238", EntityKind.Spell, 1))
        {
            Target = TargetRequirement.Character, Overload = 1,
            OnPlay = [new DamageEffect(DamageTarget.Selected, 3)]
        },
        new SimulationCard(new("EX1_241", EntityKind.Spell, 3))
        {
            Target = TargetRequirement.Character, Overload = 2,
            OnPlay = [new DamageEffect(DamageTarget.Selected, 5)]
        },
        new SimulationCard(new("EX1_259", EntityKind.Spell, 3))
        {
            Overload = 1, OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 3)]
        },
        new SimulationCard(new("BRM_011", EntityKind.Spell, 2))
        {
            Target = TargetRequirement.Character,
            OnPlay = [new DamageEffect(DamageTarget.Selected, 2), new UnlockOverloadEffect()]
        },
        new SimulationCard(new("OG_026", EntityKind.Minion, 2, 3, 2))
        {
            OnPlay = [new UnlockOverloadEffect()]
        },
        Secret("EX1_287", 3, new(TriggerTiming.SpellCounterWindow, TriggerRelation.Opponent, [new CounterSpellEffect()])),
        Secret("EX1_610", 2, new(TriggerTiming.AttackProposed, TriggerRelation.Friendly, [new DamageEffect(DamageTarget.AllEnemies, 2)])
        {
            Focus = TriggerFocus.AttackDefender, EntityKind = EntityKind.Hero
        }),
        Secret("EX1_611", 2, new(TriggerTiming.AttackProposed, TriggerRelation.Opponent,
            [new ReturnMinionEffect(CostDelta: 2, Target: EffectTarget.TriggerSubject)])
        {
            EntityKind = EntityKind.Minion, RequireLivingEntity = true
        }),
        Secret("EX1_609", 2, new(TriggerTiming.AfterMinionPlayed, TriggerRelation.Opponent, [new DamageEffect(DamageTarget.TriggerSubject, 6)])
        {
            EntityKind = EntityKind.Minion, RequireLivingEntity = true
        }),
        Secret("EX1_130", 1, new(TriggerTiming.AttackProposed, TriggerRelation.Friendly, [new SummonAttackDefenderEffect("EX1_130a")])
        {
            Focus = TriggerFocus.AttackDefender, RequiresBoardSpace = true
        }),
        new(new("EX1_130a", EntityKind.Minion, 1, 2, 1)),
        Secret("EX1_289", 3, new(TriggerTiming.AttackProposed, TriggerRelation.Friendly, [new GainArmorEffect(8)])
        {
            Focus = TriggerFocus.AttackDefender, EntityKind = EntityKind.Hero
        }),
        Secret("EX1_554", 2, new(TriggerTiming.AttackProposed, TriggerRelation.Friendly,
            [new SummonEffect("EX1_554t"), new SummonEffect("EX1_554t"), new SummonEffect("EX1_554t")])
        {
            Focus = TriggerFocus.AttackDefender, EntityKind = EntityKind.Minion, RequiresBoardSpace = true
        }),
        new(new("EX1_554t", EntityKind.Minion, 1, 1, 1))
    ];

    private static SimulationCard Secret(string cardId, int cost, CardTrigger trigger) =>
        new(new(cardId, EntityKind.Spell, cost)) { IsSecret = true, Triggers = [trigger] };
}
