using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

/// <summary>将独立编写的卡牌规则与对应构建的静态属性绑定。</summary>
public static class StarterCardCatalog
{
    private static readonly Lazy<ImmutableArray<SimulationCard>> BuiltInCards = new(() =>
    {
        using var stream = typeof(StarterCardCatalog).Assembly.GetManifestResourceStream("Silverfish.Simulation.StarterCardData.json")
            ?? throw new InvalidOperationException("The built-in starter card data is missing.");
        return FromCardData(CardDataCatalog.LoadJson(stream));
    });

    public static ImmutableArray<SimulationCard> Cards => BuiltInCards.Value;

    public static ImmutableArray<SimulationCard> FromCardData(CardDataCatalog catalog) =>
        CreateCards(cardId => CreateCard(catalog.GetCard(cardId)));

    private static SimulationCard CreateCard(CardMetadata metadata)
    {
        const int windfury = 189, taunt = 190, divineShield = 194, charge = 197, overload = 215, rush = 791;
        var keywords = (metadata.GetTag(windfury) > 0 ? CardKeywords.Windfury : CardKeywords.None) |
            (metadata.GetTag(taunt) > 0 ? CardKeywords.Taunt : CardKeywords.None) |
            (metadata.GetTag(divineShield) > 0 ? CardKeywords.DivineShield : CardKeywords.None) |
            (metadata.GetTag(charge) > 0 ? CardKeywords.Charge : CardKeywords.None) |
            (metadata.GetTag(rush) > 0 ? CardKeywords.Rush : CardKeywords.None);
        return new(metadata.ToDefinition()) { Keywords = keywords, Overload = metadata.GetTag(overload) };
    }

    private static ImmutableArray<SimulationCard> CreateCards(Func<string, SimulationCard> define) =>
    [
        define("CS2_231"),
        define("CS2_172"),
        define("CS2_023") with
        {
            OnPlay = [new DrawEffect(2)]
        },
        define("CS2_032") with
        {
            OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 5)]
        },
        define("EX1_007") with
        {
            Triggers = [new(TriggerTiming.Damage, TriggerRelation.Friendly, [new DrawEffect(1)])
            {
                EntityMatch = TriggerEntityMatch.Self
            }]
        },
        define("EX1_402") with
        {
            Triggers = [new(TriggerTiming.Damage, TriggerRelation.Friendly, [new GainArmorEffect(1)])
            {
                EntityKind = EntityKind.Minion
            }]
        },
        define("CS2_122") with
        {
            Auras = [new(AuraRecipients.OtherFriendlyMinions, new(StatModifierMode.Add, Attack: 1))]
        },
        define("CS2_222") with
        {
            Auras = [new(AuraRecipients.OtherFriendlyMinions, new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1))]
        },
        define("EX1_162") with
        {
            Auras = [new(AuraRecipients.AdjacentFriendlyMinions, new(StatModifierMode.Add, Attack: 1))]
        },
        define("CS2_092") with
        {
            Target = TargetRequirement.Minion,
            OnPlay = [new EnchantStatsEffect(StatTarget.Selected, new(StatModifierMode.Add, Attack: 4, MaximumHealth: 4))]
        },
        define("CS2_046") with
        {
            OnPlay = [new EnchantStatsEffect(StatTarget.FriendlyMinions, new(StatModifierMode.Add, Attack: 3),
                EnchantmentDuration.CurrentTurn)]
        },
        define("EX1_360") with
        {
            Target = TargetRequirement.Minion,
            OnPlay = [new EnchantStatsEffect(StatTarget.Selected, new(StatModifierMode.Set, Attack: 1))]
        },
        define("EX1_619") with
        {
            OnPlay = [new EnchantStatsEffect(StatTarget.AllMinions, new(StatModifierMode.Set, MaximumHealth: 1))]
        },
        define("EX1_019") with
        {
            Target = TargetRequirement.FriendlyMinion,
            OnPlay = [new EnchantStatsEffect(StatTarget.Selected, new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1))]
        },
        define("EX1_332") with
        {
            Target = TargetRequirement.Minion,
            OnPlay = [new SilenceEffect()]
        },
        define("EX1_245") with
        {
            Target = TargetRequirement.Minion,
            OnPlay = [new SilenceEffect(), new DamageEffect(DamageTarget.Selected, 1)]
        },
        define("CFM_851") with
        {
            Triggers =
            [
                new(TriggerTiming.CardDrawn, TriggerRelation.Opponent,
                    [new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, Attack: 1, MaximumHealth: 1))])
            ]
        },
        define("CS2_106"),
        define("CS2_091"),
        define("CS2_112"),
        define("CS2_082"),
        define("CS2_097") with
        {
            Triggers = [new(TriggerTiming.HeroAttacking, TriggerRelation.Friendly, [new HealFriendlyHeroEffect(3)])]
        },
        define("FP1_021") with
        {
            Deathrattle = [new DamageEffect(DamageTarget.AllMinions, 1)]
        },
        define("EX1_066") with
        {
            OnPlay = [new DestroyEnemyWeaponEffect()]
        },
        define("HERO_01bp") with
        {
            OnPlay = [new GainArmorEffect(2)]
        },
        define("HERO_03bp") with
        {
            OnPlay = [new EquipWeaponEffect("CS2_082")]
        },
        define("HERO_08bp") with
        {
            Target = TargetRequirement.Character,
            OnPlay = [new DamageEffect(DamageTarget.Selected, 1)]
        },
        define("HERO_06bp") with
        {
            OnPlay = [new EnchantStatsEffect(StatTarget.FriendlyHero, new(StatModifierMode.Add, Attack: 1),
                EnchantmentDuration.CurrentTurn), new GainArmorEffect(1)]
        },
        define("CS2_105") with
        {
            OnPlay = [new EnchantStatsEffect(StatTarget.FriendlyHero, new(StatModifierMode.Add, Attack: 4),
                EnchantmentDuration.CurrentTurn)]
        },
        define("EX1_581") with
        {
            Target = TargetRequirement.EnemyMinion,
            OnPlay = [new ReturnMinionEffect()]
        },
        define("EX1_144") with
        {
            Target = TargetRequirement.FriendlyMinion,
            OnPlay = [new ReturnMinionEffect(CostDelta: -2)]
        },
        define("EX1_049") with
        {
            Target = TargetRequirement.FriendlyMinion,
            OnPlay = [new ReturnMinionEffect()]
        },
        define("CS2_022") with
        {
            Target = TargetRequirement.Minion,
            OnPlay = [new TransformMinionEffect("CS2_tk1")]
        },
        define("CS2_tk1"),
        define("EX1_246") with
        {
            Target = TargetRequirement.Minion,
            OnPlay = [new TransformMinionEffect("hexfrog")]
        },
        define("hexfrog"),
        define("CS2_076") with
        {
            Target = TargetRequirement.EnemyMinion,
            OnPlay = [new DestroyMinionEffect()]
        },
        define("CS1_113") with
        {
            Target = TargetRequirement.EnemyMinion,
            OnPlay = [new TakeControlEffect()]
        },
        define("BRM_017") with
        {
            Requirements = PlayRequirements.BoardSpace | PlayRequirements.FriendlyMinionDeath,
            OnPlay = [new ResurrectFriendlyMinionEffect()]
        },
        define("CS2_024") with
        {
            Target = TargetRequirement.Character,
            OnPlay = [new DamageEffect(DamageTarget.Selected, 3), new FreezeEffect(FreezeTarget.Selected)]
        },
        define("CS2_026") with
        {
            OnPlay = [new FreezeEffect(FreezeTarget.EnemyMinions)]
        },
        define("CS2_028") with
        {
            OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 2), new FreezeEffect(FreezeTarget.EnemyMinions)]
        },
        define("UNG_205") with
        {
            Target = TargetRequirement.EnemyCharacter,
            OnPlay = [new FreezeEffect(FreezeTarget.Selected)]
        },
        define("CS2_033") with
        {
            Triggers = [new(TriggerTiming.Damage, TriggerRelation.Friendly, [new FreezeEffect(FreezeTarget.TriggerSubject)])
            {
                Focus = TriggerFocus.DamageSource, EntityMatch = TriggerEntityMatch.Self
            }]
        },
        define("GIL_803") with
        {
            OnPlay = [new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, Attack: 3), EnchantmentDuration.CurrentTurn)]
        },
        define("CS2_124"),
        define("CS2_169"),
        define("EX1_033"),
        define("EX1_567"),
        define("EX1_238") with
        {
            Target = TargetRequirement.Character,
            OnPlay = [new DamageEffect(DamageTarget.Selected, 3)]
        },
        define("EX1_241") with
        {
            Target = TargetRequirement.Character,
            OnPlay = [new DamageEffect(DamageTarget.Selected, 5)]
        },
        define("EX1_259") with
        {
            OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 3)]
        },
        define("BRM_011") with
        {
            Target = TargetRequirement.Character,
            OnPlay = [new DamageEffect(DamageTarget.Selected, 2), new UnlockOverloadEffect()]
        },
        define("OG_026") with
        {
            OnPlay = [new UnlockOverloadEffect()]
        },
        Secret(define("EX1_287"), new(TriggerTiming.SpellCounterWindow, TriggerRelation.Opponent, [new CounterSpellEffect()])),
        Secret(define("EX1_610"), new(TriggerTiming.AttackProposed, TriggerRelation.Friendly, [new DamageEffect(DamageTarget.AllEnemies, 2)])
        {
            Focus = TriggerFocus.AttackDefender, EntityKind = EntityKind.Hero
        }),
        Secret(define("EX1_611"), new(TriggerTiming.AttackProposed, TriggerRelation.Opponent,
            [new ReturnMinionEffect(CostDelta: 2, Target: EffectTarget.TriggerSubject)])
        {
            EntityKind = EntityKind.Minion, RequireLivingEntity = true
        }),
        Secret(define("EX1_609"), new(TriggerTiming.AfterMinionPlayed, TriggerRelation.Opponent, [new DamageEffect(DamageTarget.TriggerSubject, 6)])
        {
            EntityKind = EntityKind.Minion, RequireLivingEntity = true
        }),
        Secret(define("EX1_130"), new(TriggerTiming.AttackProposed, TriggerRelation.Friendly, [new SummonAttackDefenderEffect("EX1_130a")])
        {
            Focus = TriggerFocus.AttackDefender, RequiresBoardSpace = true
        }),
        define("EX1_130a"),
        Secret(define("EX1_289"), new(TriggerTiming.AttackProposed, TriggerRelation.Friendly, [new GainArmorEffect(8)])
        {
            Focus = TriggerFocus.AttackDefender, EntityKind = EntityKind.Hero
        }),
        Secret(define("EX1_554"), new(TriggerTiming.AttackProposed, TriggerRelation.Friendly,
            [new SummonEffect("EX1_554t"), new SummonEffect("EX1_554t"), new SummonEffect("EX1_554t")])
        {
            Focus = TriggerFocus.AttackDefender, EntityKind = EntityKind.Minion, RequiresBoardSpace = true
        }),
        define("EX1_554t"),
        define("EX1_277") with
        {
            OnPlay = [new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 1, Count: 3)]
        },
        define("CS2_114") with
        {
            MinimumEnemyMinions = 1,
            OnPlay = [new RandomDamageEffect(RandomCharacterPool.EnemyMinions, 2, Count: 2, Mode: RandomDamageMode.DistinctBatch)]
        },
        define("EX1_251") with
        {
            MinimumEnemyMinions = 1,
            OnPlay = [new RandomDamageEffect(RandomCharacterPool.EnemyMinions, 2, Count: 2, Mode: RandomDamageMode.DistinctBatch)]
        },
        define("EX1_617") with
        {
            MinimumEnemyMinions = 1, OnPlay = [new DestroyRandomEnemyMinionEffect()]
        },
        define("EX1_407") with
        {
            MinimumTotalMinions = 2, OnPlay = [new DestroyAllButOneMinionEffect()]
        },
        define("NEW1_031") with
        {
            Requirements = PlayRequirements.BoardSpace,
            OnPlay = [new SummonRandomMinionEffect(["NEW1_032", "NEW1_033", "NEW1_034"])]
        },
        define("NEW1_032"),
        define("NEW1_033") with
        {
            Auras = [new(AuraRecipients.OtherFriendlyMinions, new(StatModifierMode.Add, Attack: 1))]
        },
        define("NEW1_034"),
        define("NEW1_019") with
        {
            Triggers = [new(TriggerTiming.AfterMinionSummoned, TriggerRelation.Friendly,
                [new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 1)])]
        },
        define("EX1_082") with
        {
            OnPlay = [new RandomDamageEffect(RandomCharacterPool.OtherCharacters, 1, Count: 3)]
        },
        define("GVG_090") with
        {
            OnPlay = [new RandomDamageEffect(RandomCharacterPool.OtherCharacters, 1, Count: 6)]
        },
        define("GAME_005") with
        {
            OnPlay = [new ChangeManaEffect(ManaOperation.GainTemporary, 1)]
        },
        define("EX1_169") with
        {
            OnPlay = [new ChangeManaEffect(ManaOperation.GainTemporary, 1)]
        },
        define("SCH_427") with
        {
            OnPlay = [new ChangeManaEffect(ManaOperation.Refresh, 2)]
        },
        define("CS2_013") with
        {
            OnPlay = [new ChangeManaEffect(ManaOperation.GainEmpty, 1, OverflowCardId: "CS2_013t")]
        },
        define("CS2_013t") with
        {
            OnPlay = [new DrawEffect(1)]
        },
        define("BT_130") with
        {
            OnPlay = [new ChangeManaEffect(ManaOperation.GainEmpty, 2, OverflowCardId: "CS2_013t")]
        },
        define("BOT_054") with
        {
            OnPlay = [new ChangeManaEffect(ManaOperation.GainFilled, 2, ManaRecipient.BothPlayers)]
        },
        define("AT_038") with
        {
            OnPlay = [new ChangeManaEffect(ManaOperation.GainEmpty, 1)],
            Deathrattle = [new ChangeManaEffect(ManaOperation.Destroy, 1)]
        },
        define("LOOT_351") with
        {
            Deathrattle = [new ChangeManaEffect(ManaOperation.GainEmpty, 1)]
        },
        define("EX1_301") with
        {
            OnPlay = [new ChangeManaEffect(ManaOperation.Destroy, 1)]
        },
        define("LOOT_392") with
        {
            Deathrattle = [new ChangeManaEffect(ManaOperation.Refresh, int.MaxValue)]
        },
        define("EX1_164") with
        {
            Choices = [new("gainCrystals", [new ChangeManaEffect(ManaOperation.GainFilled, 2)]), new("draw", [new DrawEffect(3)])]
        },
        define("WON_014") with
        {
            Choices = [new("gainCrystal", [new ChangeManaEffect(ManaOperation.GainEmpty, 1)]), new("draw", [new DrawEffect(1)])]
        },
        define("CFM_308") with
        {
            Choices = [new("armor", [new GainArmorEffect(10)]), new("refresh", [new ChangeManaEffect(ManaOperation.Refresh, int.MaxValue)])]
        }
    ];

    private static SimulationCard Secret(SimulationCard card, CardTrigger trigger) =>
        card with { IsSecret = true, Triggers = [trigger] };
}
