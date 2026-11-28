using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

[Flags]
public enum CardKeywords { None = 0, Taunt = 1, Charge = 2, DivineShield = 4, Rush = 8, Windfury = 16, CastsWhenDrawn = 32, Poisonous = 64, Lifesteal = 128, Overheal = 256 }

public enum TargetRequirement { None, Character, EnemyCharacter, EnemyMinion, FriendlyMinion, Minion, FriendlyCharacter }

[Flags]
public enum PlayRequirements { None = 0, BoardSpace = 1, FriendlyMinionDeath = 2 }

public enum DamageTarget { Selected, EnemyHero, AllMinions, Source, TriggerSubject, FriendlyHero, EnemyMinions, AllEnemies, AllCharacters, OtherCharacters, FriendlyMinions, BothHeroes, OtherMinions }
public enum FreezeTarget { Selected, EnemyMinions, TriggerSubject, SelectedAndAdjacentMinions, RandomUnfrozenEnemy }
public enum HealTarget { Selected, Source, TriggerSubject, FriendlyHero, EnemyHero, FriendlyMinions, AllMinions, AllFriendlyCharacters, RandomDamagedFriendlyCharacter, AllCharacters }
public enum EffectTarget { Selected, TriggerSubject, Source }
public enum RandomCharacterPool { EnemyCharacters, EnemyMinions, OtherCharacters, AllMinions, NonMechMinions, AllCharacters }
public enum RandomDamageMode { Sequential, DistinctBatch }
public enum ManaOperation { GainTemporary, GainEmpty, GainFilled, Refresh, Destroy }
public enum ManaRecipient { Controller, Opponent, BothPlayers }
public enum SummonPlacement { Default, Rightmost, LeftOfSource, RightOfSource }
public enum StatTarget { Selected, Source, TriggerSubject, FriendlyMinions, OtherFriendlyMinions, AdjacentFriendlyMinions, RandomOtherFriendlyMinion, AllMinions, FriendlyHero, EnemyMinions, FriendlyHandAndDeck, FriendlyHand }
public enum StatScaleSource { CardsPlayedBeforeThis, FriendlyHandCount, EquippedWeaponAttack, EnemyDeathrattleMinions, FriendlyMinions, EnemyMinions }
public enum DynamicStatMode { AttackEqualsHealth, SwapAttackAndHealth, DoubleAttack }
public enum DynamicDrawMode { DamagedFriendlyCharacters, MatchOpponentHandSize }
public enum EnchantmentDuration { Permanent, CurrentTurn, UntilNextTurn }
public enum EnchantmentRemovalReason { Expired, Silenced, LeftPlay, Transformed, Died }
public enum AuraRecipients { OtherFriendlyMinions, AdjacentFriendlyMinions, Self, FriendlyHero }
public enum AuraCondition { None = 0, WhileDamaged, WhileWeaponEquipped, WhileControllingMech, WhileOpponentHasAtLeastSixHandCards, WhileHavingSpellDamage, ControllerTurnOnly }
public enum SelfCostReduction
{
    None = 0, OtherMinionsOnBattlefield, EquippedWeaponAttack, OtherFriendlyHandCards, FriendlyHeroMissingHealth,
    MinionsDiedThisTurn, OpponentHandCards, FourIfControllingDamagedMinion, HeroPowerUses, FriendlyPirates, FriendlyMurlocs,
    SpellsCast
}
public enum CostAuraTarget { Hand, Minion, Spell, Weapon, HeroPower, Location, Hero }
public enum CostAuraRecipient { Friendly, Opponent, BothPlayers }
public enum HandCostTarget { Source, DrawnCard, FriendlyHand }
public enum HandRecipient { Controller, Opponent }
public enum DiscardSelection { Random, HighestCost, LowestCost }
public enum CardRecipient { Controller, Opponent, CurrentTurnPlayer }
public enum CardPlacement { Hand, DeckTop, DeckBottom, RandomDeckPosition, ShuffleIntoDeck }

public sealed record StatAura(AuraRecipients Recipients, StatModifier? Modifier = null)
{
    public string? EnchantmentCardId { get; init; }
    public CardFilter? CardFilter { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public AuraCondition Condition { get; init; }
    public GAME_TAG? GrantedTag { get; init; }
}

public sealed record WeaponAttackAura(int Attack, AuraCondition Condition = AuraCondition.None)
{
    public string? EnchantmentCardId { get; init; }
}

/// <summary>对指定玩家的手牌或英雄技能持续增减费用。</summary>
public sealed record CostAura(
    [property: JsonRequired] CostAuraRecipient Recipient,
    [property: JsonRequired] CostAuraTarget Target,
    [property: JsonRequired] int Delta)
{
    public CardFilter? CardFilter { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool FirstMinionEachTurn { get; init; }
    public int? MinimumCost { get; init; }
    public int? SetCost { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool SecretOnly { get; init; }
}

public enum TriggerTiming
{
    TurnStarted, TurnEnded, WhenMinionSummoned, AfterMinionSummoned,
    BeforeSpellResolved, AfterSpellResolved, MinionDied, Damage, CardDrawn, HeroAttacking, WeaponDestroyed, AfterHeroPowerUsed,
    SpellCounterWindow, SpellTargetWindow, AttackProposed, FatalDamageWindow, AfterMinionPlayed, CardDiscarded,
    Overkill, HonorableKill, Overheal, WhenCardPlayed, AfterCardPlayed, Healed, SecretRevealed, TargetedBySpell, ArmorGained,
    WeaponEquipped, AfterAttack
}

public enum TriggerRelation { Friendly, Opponent, Any }
public enum TriggerFocus { EventTarget, DamageSource, AttackDefender }
public enum TriggerEntityMatch { Any, Self, Other }

public sealed record CardTrigger(TriggerTiming Timing, TriggerRelation Relation, ImmutableArray<CardEffect> Effects)
{
    public TriggerFocus Focus { get; init; }
    public TriggerEntityMatch EntityMatch { get; init; }
    public EntityKind? EntityKind { get; init; }
    public EntityKind? AttackingKind { get; init; }
    public CardFilter? CardFilter { get; init; }
    public bool RequireLivingEntity { get; init; }
    public bool RequiresBoardSpace { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool SecretOnly { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool OverloadOnly { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool ControllerTurnOnly { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool AllowOnControllerTurn { get; init; }
    public bool RequireSourceAttacker { get; init; }
    public bool KilledMinion { get; init; }
    public bool SpellTargetOnOtherFriendlyMinion { get; init; }
    public int? ExactPrintedAttack { get; init; }
    public string? ExcludeCardId { get; init; }
    public EffectCondition? Condition { get; init; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(DamageEffect), "damage")]
[JsonDerivedType(typeof(DealAttackAsDamageToAllMinionsEffect), "dealAttackAsDamageToAllMinions")]
[JsonDerivedType(typeof(EventAmountDamageEffect), "eventAmountDamage")]
[JsonDerivedType(typeof(SplashDamageEffect), "splashDamage")]
[JsonDerivedType(typeof(ScaledDamageEffect), "scaledDamage")]
[JsonDerivedType(typeof(AdjacentAttackDamageEffect), "adjacentAttackDamage")]
[JsonDerivedType(typeof(DrawEffect), "draw")]
[JsonDerivedType(typeof(DynamicDrawEffect), "dynamicDraw")]
[JsonDerivedType(typeof(DrawAndDamageByCostEffect), "drawAndDamageByCost")]
[JsonDerivedType(typeof(DrawMatchingEffect), "drawMatching")]
[JsonDerivedType(typeof(DrawAndTransformIfMinionEffect), "drawAndTransformIfMinion")]
[JsonDerivedType(typeof(DrawAndDiscountIfMatchesEffect), "drawAndDiscountIfMatches")]
[JsonDerivedType(typeof(DrawToHandSizeEffect), "drawToHandSize")]
[JsonDerivedType(typeof(MillDeckEffect), "millDeck")]
[JsonDerivedType(typeof(ChangeHandCostEffect), "changeHandCost")]
[JsonDerivedType(typeof(AddTemporaryCostAuraEffect), "addTemporaryCostAura")]
[JsonDerivedType(typeof(ChangeCostTypeEffect), "changeCostType")]
[JsonDerivedType(typeof(RecruitEffect), "recruit")]
[JsonDerivedType(typeof(PutSecretFromDeckEffect), "putSecretFromDeck")]
[JsonDerivedType(typeof(SummonFromHandEffect), "summonFromHand")]
[JsonDerivedType(typeof(CreateCardEffect), "createCard")]
[JsonDerivedType(typeof(CreateRandomCardEffect), "createRandomCard")]
[JsonDerivedType(typeof(CreateRandomMatchingCardEffect), "createRandomMatchingCard")]
[JsonDerivedType(typeof(CreateRandomOpponentClassSpellEffect), "createRandomOpponentClassSpell")]
[JsonDerivedType(typeof(CopyTriggerCardToHandEffect), "copyTriggerCardToHand")]
[JsonDerivedType(typeof(CopyTargetCardEffect), "copyTargetCard")]
[JsonDerivedType(typeof(CopyFriendlyBoardToHandEffect), "copyFriendlyBoardToHand")]
[JsonDerivedType(typeof(CreateCardAtTurnEndEffect), "createCardAtTurnEnd")]
[JsonDerivedType(typeof(CopyDeckCardsEffect), "copyDeckCards")]
[JsonDerivedType(typeof(SummonDeckCopyEffect), "summonDeckCopy")]
[JsonDerivedType(typeof(DiscardEffect), "discard")]
[JsonDerivedType(typeof(DiscardHandEffect), "discardHand")]
[JsonDerivedType(typeof(SummonDiscardedMinionEffect), "summonDiscardedMinion")]
[JsonDerivedType(typeof(GainArmorEffect), "gainArmor")]
[JsonDerivedType(typeof(HealFriendlyHeroEffect), "healFriendlyHero")]
[JsonDerivedType(typeof(HealEffect), "heal")]
[JsonDerivedType(typeof(SetHeroRemainingHealthEffect), "setHeroRemainingHealth")]
[JsonDerivedType(typeof(ReplaceHeroPowerEffect), "replaceHeroPower")]
[JsonDerivedType(typeof(ReplaceHeroEffect), "replaceHero")]
[JsonDerivedType(typeof(EquipWeaponEffect), "equipWeapon")]
[JsonDerivedType(typeof(EquipRandomWeaponEachPlayerEffect), "equipRandomWeaponEachPlayer")]
[JsonDerivedType(typeof(ModifyWeaponEffect), "modifyWeapon")]
[JsonDerivedType(typeof(DestroyEnemyWeaponEffect), "destroyEnemyWeapon")]
[JsonDerivedType(typeof(DestroyFriendlyWeaponDamageEffect), "destroyFriendlyWeaponDamage")]
[JsonDerivedType(typeof(ProtectFriendlyMinionsMinimumHealthEffect), "protectFriendlyMinionsMinimumHealth")]
[JsonDerivedType(typeof(CopyTriggerSpellToOtherPlayerEffect), "copyTriggerSpellToOtherPlayer")]
[JsonDerivedType(typeof(EnchantStatsEffect), "enchantStats")]
[JsonDerivedType(typeof(RandomEnchantStatsEffect), "randomEnchantStats")]
[JsonDerivedType(typeof(ScaledEnchantStatsEffect), "scaledEnchantStats")]
[JsonDerivedType(typeof(DynamicStatChangeEffect), "dynamicStatChange")]
[JsonDerivedType(typeof(DrainDivineShieldsBuffEffect), "drainDivineShieldsBuff")]
[JsonDerivedType(typeof(ConsumeAdjacentMinionsEffect), "consumeAdjacentMinions")]
[JsonDerivedType(typeof(SilenceEffect), "silence")]
[JsonDerivedType(typeof(RemoveAllStealthEffect), "removeAllStealth")]
[JsonDerivedType(typeof(DestroySecretsEffect), "destroySecrets")]
[JsonDerivedType(typeof(StealRandomEnemySecretEffect), "stealRandomEnemySecret")]
[JsonDerivedType(typeof(GrantRandomKeywordToOtherFriendlyMinionsEffect), "grantRandomKeywordToOtherFriendlyMinions")]
[JsonDerivedType(typeof(SwapHealthEffect), "swapHealth")]
[JsonDerivedType(typeof(TriggerFriendlyDeathrattlesEffect), "triggerFriendlyDeathrattles")]
[JsonDerivedType(typeof(ShuffleMinionIntoDeckEffect), "shuffleMinionIntoDeck")]
[JsonDerivedType(typeof(MimironCombineEffect), "mimironCombine")]
[JsonDerivedType(typeof(ScheduleDormantAwakenEffect), "scheduleDormantAwaken")]
[JsonDerivedType(typeof(SummonEffect), "summon")]
[JsonDerivedType(typeof(SummonPerEnemyMinionEffect), "summonPerEnemyMinion")]
[JsonDerivedType(typeof(SwapSelfWithRandomHandMinionEffect), "swapSelfWithRandomHandMinion")]
[JsonDerivedType(typeof(FillBoardSummonEffect), "fillBoardSummon")]
[JsonDerivedType(typeof(SummonCopyEffect), "summonCopy")]
[JsonDerivedType(typeof(CopyMinionToSelfEffect), "copyMinionToSelf")]
[JsonDerivedType(typeof(TakeControlEffect), "takeControl")]
[JsonDerivedType(typeof(TakeControlRandomEnemyMinionEffect), "takeControlRandomEnemyMinion")]
[JsonDerivedType(typeof(ReturnMinionEffect), "returnMinion")]
[JsonDerivedType(typeof(TransformMinionEffect), "transformMinion")]
[JsonDerivedType(typeof(RandomTransformOtherMinionEffect), "randomTransformOtherMinion")]
[JsonDerivedType(typeof(TransformMinionByCostEffect), "transformMinionByCost")]
[JsonDerivedType(typeof(DestroyMinionEffect), "destroyMinion")]
[JsonDerivedType(typeof(DestroyAllMinionsEffect), "destroyAllMinions")]
[JsonDerivedType(typeof(ResurrectFriendlyMinionEffect), "resurrectFriendlyMinion")]
[JsonDerivedType(typeof(ResurrectDiedThisTurnEffect), "resurrectDiedThisTurn")]
[JsonDerivedType(typeof(ResummonMinionEffect), "resummonMinion")]
[JsonDerivedType(typeof(ReincarnateMinionEffect), "reincarnateMinion")]
[JsonDerivedType(typeof(FreezeEffect), "freeze")]
[JsonDerivedType(typeof(UnlockOverloadEffect), "unlockOverload")]
[JsonDerivedType(typeof(CounterSpellEffect), "counterSpell")]
[JsonDerivedType(typeof(RedirectSpellTargetEffect), "redirectSpellTarget")]
[JsonDerivedType(typeof(PreventDamageEffect), "preventDamage")]
[JsonDerivedType(typeof(SummonAttackDefenderEffect), "summonAttackDefender")]
[JsonDerivedType(typeof(RedirectAttackToRandomOtherCharacterEffect), "redirectAttackToRandomOtherCharacter")]
[JsonDerivedType(typeof(RandomDamageEffect), "randomDamage")]
[JsonDerivedType(typeof(RandomDamageRangeEffect), "randomDamageRange")]
[JsonDerivedType(typeof(DestroyRandomEnemyMinionEffect), "destroyRandomEnemyMinion")]
[JsonDerivedType(typeof(DestroyAllButOneMinionEffect), "destroyAllButOneMinion")]
[JsonDerivedType(typeof(SummonRandomMinionEffect), "summonRandomMinion")]
[JsonDerivedType(typeof(SummonMatchingMinionEffect), "summonMatchingMinion")]
[JsonDerivedType(typeof(ChangeManaEffect), "changeMana")]
[JsonDerivedType(typeof(ConditionalEffect), "conditional")]
[JsonDerivedType(typeof(RandomChanceEffect), "randomChance")]
[JsonDerivedType(typeof(JoustEffect), "joust")]
[JsonDerivedType(typeof(ReturnSourceCardEffect), "returnSourceCard")]
[JsonDerivedType(typeof(SummonMatchingCostEffect), "summonMatchingCost")]
[JsonDerivedType(typeof(ShufflePlayedCardEffect), "shufflePlayedCard")]
[JsonDerivedType(typeof(TransformFriendlyMinionsByCostEffect), "transformFriendlyMinionsByCost")]
[JsonDerivedType(typeof(RecastDeathrattleEffect), "recastDeathrattle")]
[JsonDerivedType(typeof(DestroyMatchingMinionsEffect), "destroyMatchingMinions")]
[JsonDerivedType(typeof(ResurrectMatchingMinionsEffect), "resurrectMatchingMinions")]
[JsonDerivedType(typeof(EmpowerCardEffect), "empowerCard")]
[JsonDerivedType(typeof(DamageSplitByAttackEffect), "damageSplitByAttack")]
[JsonDerivedType(typeof(SummonAtTurnEndEffect), "summonAtTurnEnd")]
[JsonDerivedType(typeof(DestroyRandomFriendlyMinionEffect), "destroyRandomFriendlyMinion")]
[JsonDerivedType(typeof(CopyOpponentHeroPowerEffect), "copyOpponentHeroPower")]
[JsonDerivedType(typeof(EnchantByEventAmountEffect), "enchantByEventAmount")]
[JsonDerivedType(typeof(ReplaceHandAndDeckEffect), "replaceHandAndDeck")]
[JsonDerivedType(typeof(SpendRemainingManaEffect), "spendRemainingMana")]
[JsonDerivedType(typeof(CastRandomSpellEffect), "castRandomSpell")]
[JsonDerivedType(typeof(CopyOrSwapMinionStatsEffect), "copyOrSwapMinionStats")]
[JsonDerivedType(typeof(HealingDealsDamageThisTurnEffect), "healingDealsDamageThisTurn")]
[JsonDerivedType(typeof(MergeMatchingMinionsEffect), "mergeMatchingMinions")]
[JsonDerivedType(typeof(EquipRandomWeaponEffect), "equipRandomWeapon")]
[JsonDerivedType(typeof(DestroyMinionsAndDrawEffect), "destroyMinionsAndDraw")]
[JsonDerivedType(typeof(DestroySecretsAndBuffEffect), "destroySecretsAndBuff")]
[JsonDerivedType(typeof(HealPerEnemyMinionEffect), "healPerEnemyMinion")]
[JsonDerivedType(typeof(RefreshAttackEffect), "refreshAttack")]
[JsonDerivedType(typeof(RewardSpellCastsThisTurnEffect), "rewardSpellCastsThisTurn")]
[JsonDerivedType(typeof(UpgradeHeroPowerEffect), "upgradeHeroPower")]
[JsonDerivedType(typeof(ReturnDiscardedCardsEffect), "returnDiscardedCards")]
[JsonDerivedType(typeof(CopyDeathrattleEffect), "copyDeathrattle")]
[JsonDerivedType(typeof(CopySpellOntoSelfEffect), "copySpellOntoSelf")]
[JsonDerivedType(typeof(TransformHandCardEffect), "transformHandCard")]
[JsonDerivedType(typeof(ReplaceClassCardsEffect), "replaceClassCards")]
[JsonDerivedType(typeof(EnchantMinionsByTypeEffect), "enchantMinionsByType")]
[JsonDerivedType(typeof(DestroyAndCaptureMinionEffect), "destroyAndCaptureMinion")]
[JsonDerivedType(typeof(ResummonCapturedMinionEffect), "resummonCapturedMinion")]
[JsonDerivedType(typeof(AddUniqueClassMinionsToDeckEffect), "addUniqueClassMinionsToDeck")]
public abstract record CardEffect;

public sealed record DamageEffect(DamageTarget Target, int Amount) : CardEffect
{
    /// <summary>在同一目标受到伤害后、伤害触发器执行前施加。</summary>
    public EnchantStatsEffect? Enchantment { get; init; }
    public string? ExcludeCardId { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool UndamagedOnly { get; init; }
    public MinionType? ExcludeMinionType { get; init; }
    public CardFilter? TargetCardFilter { get; init; }
}
public sealed record DealAttackAsDamageToAllMinionsEffect : CardEffect;
public sealed record EventAmountDamageEffect(DamageTarget Target) : CardEffect;
public sealed record SplashDamageEffect(int CenterAmount, int AdjacentAmount) : CardEffect;
public sealed record ScaledDamageEffect(
    [property: JsonRequired] ConditionCharacter ValueSource,
    [property: JsonRequired] CharacterMetric Metric,
    [property: JsonRequired] DamageTarget Target,
    bool DestroyValueSource = false) : CardEffect;
public sealed record AdjacentAttackDamageEffect(EffectTarget Target = EffectTarget.Selected) : CardEffect;
public sealed record DrawEffect(int Count, CardRecipient Recipient = CardRecipient.Controller,
    int CostDelta = 0, string? EnchantmentCardId = null) : CardEffect
{
    public bool SummonMinions { get; init; }
    public int ExtraCopies { get; init; }
}
public sealed record DynamicDrawEffect([property: JsonRequired] DynamicDrawMode Mode) : CardEffect;
public sealed record DrawAndDamageByCostEffect(DamageTarget Target = DamageTarget.Selected) : CardEffect;
public sealed record DrawMatchingEffect([property: JsonRequired] CardFilter CardFilter, int Count = 1) : CardEffect;
public sealed record DrawAndTransformIfMinionEffect([property: JsonRequired] string TransformCardId) : CardEffect;
public sealed record DrawAndDiscountIfMatchesEffect(
    [property: JsonRequired] CardFilter CardFilter,
    int CostDelta,
    string? EnchantmentCardId = null) : CardEffect;
public sealed record DrawToHandSizeEffect(int TargetSize, CardRecipient Recipient = CardRecipient.CurrentTurnPlayer) : CardEffect;
public sealed record MillDeckEffect(int Count, CardRecipient Recipient = CardRecipient.Controller) : CardEffect;
public sealed record ChangeHandCostEffect(
    [property: JsonRequired] HandCostTarget Target,
    [property: JsonRequired] int Delta,
    EnchantmentDuration Duration = EnchantmentDuration.Permanent) : CardEffect
{
    public CardFilter? CardFilter { get; init; }
    public string? EnchantmentCardId { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool OtherClassOnly { get; init; }
}
public sealed record AddTemporaryCostAuraEffect(
    EntityKind TargetKind,
    CostModifierMode Mode,
    int Amount,
    CardRecipient Recipient = CardRecipient.Controller,
    bool NextTurn = false,
    bool SecretOnly = false,
    bool ConsumedOnPlay = false,
    string? EnchantmentCardId = null,
    MinionType? MinionType = null,
    bool Persistent = false) : CardEffect;
public sealed record ChangeCostTypeEffect(
    [property: JsonRequired] HandCostTarget Target,
    [property: JsonRequired] TAG_CARD_ALTERNATE_COST Type,
    EnchantmentDuration Duration = EnchantmentDuration.Permanent) : CardEffect
{
    public CardFilter? CardFilter { get; init; }
    public string? EnchantmentCardId { get; init; }
}
public sealed record RecruitEffect(int Count = 1, CardRecipient Recipient = CardRecipient.Controller) : CardEffect
{
    public CardFilter? CardFilter { get; init; }
}
public sealed record PutSecretFromDeckEffect(int Count = 1) : CardEffect;
public sealed record SummonFromHandEffect(
    CardFilter? CardFilter = null,
    int Count = 1,
    CardRecipient Recipient = CardRecipient.Controller) : CardEffect;
public sealed record CreateCardEffect(
    [property: JsonRequired] string CardId,
    CardPlacement Placement = CardPlacement.Hand,
    CardRecipient Recipient = CardRecipient.Controller,
    int Count = 1) : CardEffect;
public sealed record CreateRandomCardEffect(
    [property: JsonRequired] ImmutableArray<string> CardIds,
    int Count = 1,
    CardRecipient Recipient = CardRecipient.Controller) : CardEffect;
public sealed record CreateRandomMatchingCardEffect(
    [property: JsonRequired] CardFilter CardFilter,
    int Count = 1,
    CardRecipient Recipient = CardRecipient.Controller,
    int CostDelta = 0,
    string? EnchantmentCardId = null) : CardEffect
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool OtherClassOnly { get; init; }
}
public sealed record CreateRandomOpponentClassSpellEffect(int Count = 1, string? FallbackCardId = null) : CardEffect
{
    public bool IncludeAllKinds { get; init; }
}
public sealed record CopyTriggerCardToHandEffect(int Count = 1) : CardEffect;
public sealed record CopyTargetCardEffect(
    EffectTarget Target = EffectTarget.Selected,
    CardPlacement Placement = CardPlacement.ShuffleIntoDeck,
    CardRecipient Recipient = CardRecipient.Controller,
    int Count = 1) : CardEffect
{
    public int? SetCost { get; init; }
    public StatModifier? Modifier { get; init; }
    public string? EnchantmentCardId { get; init; }
}
public sealed record CopyFriendlyBoardToHandEffect(bool DamagedOnly = false) : CardEffect;
public sealed record CreateCardAtTurnEndEffect([property: JsonRequired] string CardId) : CardEffect;
public sealed record CopyDeckCardsEffect(
    ConditionPlayer SourceDeck = ConditionPlayer.Opponent,
    int Count = 1,
    CardFilter? CardFilter = null) : CardEffect;
public sealed record SummonDeckCopyEffect(
    ConditionPlayer SourceDeck = ConditionPlayer.Opponent,
    int Count = 1,
    CardFilter? CardFilter = null,
    string? FallbackCardId = null) : CardEffect
{
    public StatModifier? StatsOverride { get; init; }
    public string? EnchantmentCardId { get; init; }
    public SummonPlacement Placement { get; init; } = SummonPlacement.Rightmost;
}
public sealed record DiscardEffect(int Count = 1, DiscardSelection Selection = DiscardSelection.Random,
    HandRecipient Recipient = HandRecipient.Controller) : CardEffect;
public sealed record DiscardHandEffect(HandRecipient Recipient = HandRecipient.Controller) : CardEffect;
public sealed record SummonDiscardedMinionEffect : CardEffect;
public sealed record GainArmorEffect(int Amount) : CardEffect;
public sealed record HealFriendlyHeroEffect(int Amount) : CardEffect;
/// <summary>Restores character Health; healing past full Health can trigger Overheal.</summary>
public sealed record HealEffect(HealTarget Target, int Amount) : CardEffect;
public sealed record SetHeroRemainingHealthEffect(int Health = 15, EffectTarget Target = EffectTarget.Selected) : CardEffect;
public sealed record ReplaceHeroPowerEffect([property: JsonRequired] string CardId) : CardEffect;
public sealed record ReplaceHeroEffect(
    [property: JsonRequired] string HeroCardId,
    [property: JsonRequired] string HeroPowerCardId) : CardEffect;
public sealed record EquipWeaponEffect(string CardId) : CardEffect;
public sealed record EquipRandomWeaponEachPlayerEffect : CardEffect;
public sealed record ModifyWeaponEffect(
    int AttackDelta = 0,
    int DurabilityDelta = 0,
    ConditionPlayer TargetWeapon = ConditionPlayer.Friendly,
    string? EnchantmentCardId = null) : CardEffect
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool UseSource { get; init; }
}
public sealed record DestroyEnemyWeaponEffect(bool DrawDurability = false) : CardEffect;
public sealed record DestroyFriendlyWeaponDamageEffect(DamageTarget Target = DamageTarget.EnemyMinions) : CardEffect;
public sealed record ProtectFriendlyMinionsMinimumHealthEffect : CardEffect;
public sealed record CopyTriggerSpellToOtherPlayerEffect : CardEffect;
public sealed record EnchantmentRemovalRule(EnchantmentRemovalReason Reason, ImmutableArray<CardEffect> Effects);
public sealed record EnchantStatsEffect(StatTarget Target, StatModifier? Modifier = null,
    EnchantmentDuration Duration = EnchantmentDuration.Permanent) : CardEffect
{
    public string? EnchantmentCardId { get; init; }
    public GAME_TAG? GrantedTag { get; init; }
    public GAME_TAG? SecondaryGrantedTag { get; init; }
    public CardFilter? CardFilter { get; init; }
    public bool ShieldedOnly { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ImmutableArray<EnchantmentRemovalRule> OnRemoved { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ImmutableArray<CardEffect> OnAttack { get; init; }
}
public sealed record RandomEnchantStatsEffect(
    StatTarget Target,
    int MinAttack,
    int MaxAttack,
    int MinHealth = 0,
    int MaxHealth = 0,
    string? EnchantmentCardId = null) : CardEffect;
public sealed record ScaledEnchantStatsEffect(
    StatTarget Target,
    StatScaleSource Scale,
    int AttackPerUnit = 0,
    int HealthPerUnit = 0,
    string? EnchantmentCardId = null) : CardEffect
{
    public CardFilter? CardFilter { get; init; }
}
public sealed record DynamicStatChangeEffect(
    StatTarget Target,
    DynamicStatMode Mode,
    string? EnchantmentCardId = null) : CardEffect;
public sealed record DrainDivineShieldsBuffEffect(
    int AttackPerShield,
    int HealthPerShield,
    string? EnchantmentCardId = null) : CardEffect;
public sealed record ConsumeAdjacentMinionsEffect(string? EnchantmentCardId = null) : CardEffect;
public sealed record SilenceEffect(bool AllEnemyMinions = false, bool OtherFriendlyMinions = false) : CardEffect;
public sealed record RemoveAllStealthEffect : CardEffect;
public sealed record DestroySecretsEffect(ConditionPlayer TargetPlayer = ConditionPlayer.Opponent) : CardEffect;
public sealed record StealRandomEnemySecretEffect : CardEffect;
public sealed record GrantRandomKeywordToOtherFriendlyMinionsEffect(ImmutableArray<GAME_TAG> Tags) : CardEffect;
public sealed record SwapHealthEffect : CardEffect;
public sealed record TriggerFriendlyDeathrattlesEffect : CardEffect;
public sealed record ShuffleMinionIntoDeckEffect(EffectTarget Target = EffectTarget.Selected) : CardEffect;
public sealed record MimironCombineEffect([property: JsonRequired] string TokenCardId, int MinimumMechs = 3) : CardEffect;
public sealed record ScheduleDormantAwakenEffect(
    [property: JsonRequired] string CardId,
    int RequiredDeaths,
    MinionType? RequiredMinionType = null) : CardEffect;
/// <summary>亡语召唤沿用死亡位置，其他情况放到场上最右侧。</summary>
public sealed record SummonEffect(string CardId, SummonPlacement Placement = SummonPlacement.Default,
    CardRecipient Recipient = CardRecipient.Controller) : CardEffect;
public sealed record SummonPerEnemyMinionEffect([property: JsonRequired] string CardId) : CardEffect;
public sealed record SwapSelfWithRandomHandMinionEffect : CardEffect;
public sealed record FillBoardSummonEffect(string CardId, bool AlternateSides = false, bool PlaceLeftNext = false) : CardEffect;
public sealed record SummonCopyEffect(EffectTarget Target = EffectTarget.Selected) : CardEffect
{
    public StatModifier? StatsOverride { get; init; }
    public string? EnchantmentCardId { get; init; }
    public bool AllOtherFriendlyMinions { get; init; }
    public SummonPlacement Placement { get; init; } = SummonPlacement.Rightmost;
}
public sealed record CopyMinionToSelfEffect : CardEffect;
public sealed record TakeControlEffect(EffectTarget Target = EffectTarget.Selected,
    bool ReadyToAttack = false, bool ReturnToSourceOpponent = false) : CardEffect;
public sealed record TakeControlRandomEnemyMinionEffect : CardEffect;
public sealed record ReturnMinionEffect(int CostDelta = 0, EffectTarget Target = EffectTarget.Selected,
    bool RandomFriendly = false) : CardEffect
{
    public string? EnchantmentCardId { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public EnchantmentDuration CostDuration { get; init; }
}
public sealed record TransformMinionEffect(string CardId, bool Self = false) : CardEffect;
public sealed record RandomTransformOtherMinionEffect(ImmutableArray<string> CardIds) : CardEffect;
public sealed record TransformMinionByCostEffect(int Increase) : CardEffect;
public sealed record DestroyMinionEffect(EffectTarget Target = EffectTarget.Selected) : CardEffect;
public sealed record DestroyAllMinionsEffect(bool ExcludeSource = false, bool IncludeLocations = false,
    string? ReplaceWithCardId = null) : CardEffect;
public sealed record ResurrectFriendlyMinionEffect : CardEffect
{
    public SummonPlacement Placement { get; init; } = SummonPlacement.Rightmost;
}
public sealed record ResurrectDiedThisTurnEffect : CardEffect;
public sealed record ResummonMinionEffect(EffectTarget Target = EffectTarget.TriggerSubject, int? RemainingHealth = null) : CardEffect;
public sealed record ReincarnateMinionEffect(EffectTarget Target = EffectTarget.Selected) : CardEffect;
public sealed record FreezeEffect(FreezeTarget Target) : CardEffect;
public sealed record UnlockOverloadEffect : CardEffect;
public sealed record CounterSpellEffect : CardEffect;
public sealed record RedirectSpellTargetEffect(string CardId) : CardEffect;
public sealed record PreventDamageEffect : CardEffect;
public sealed record SummonAttackDefenderEffect(string CardId) : CardEffect;
public sealed record RedirectAttackToRandomOtherCharacterEffect : CardEffect;
public sealed record RandomDamageEffect(RandomCharacterPool Pool, int Amount, int Count = 1,
    RandomDamageMode Mode = RandomDamageMode.Sequential) : CardEffect;
public sealed record RandomDamageRangeEffect(
    RandomCharacterPool? Pool,
    DamageTarget? Target,
    int MinAmount,
    int MaxAmount,
    string? SummonPerDamageCardId = null,
    bool RepeatUntilDeath = false) : CardEffect
{
    public bool HitEachCandidate { get; init; }
}
public sealed record DestroyRandomEnemyMinionEffect(int? MaximumAttack = null) : CardEffect;
public sealed record DestroyAllButOneMinionEffect(bool PreserveHighestAttackPerPlayer = false) : CardEffect;
public sealed record SummonRandomMinionEffect(ImmutableArray<string> CardIds) : CardEffect
{
    public bool ExcludeOnBoard { get; init; }
    public SummonPlacement Placement { get; init; } = SummonPlacement.Rightmost;
}
public sealed record SummonMatchingMinionEffect(
    [property: JsonRequired] CardFilter CardFilter,
    int Count = 1,
    CardRecipient Recipient = CardRecipient.Controller) : CardEffect;
public sealed record ChangeManaEffect(ManaOperation Operation, int Amount, ManaRecipient Recipient = ManaRecipient.Controller,
    string? OverflowCardId = null) : CardEffect;

public sealed record ConditionalEffect(EffectCondition Condition, ImmutableArray<CardEffect> Then) : CardEffect
{
    public ImmutableArray<CardEffect> Otherwise { get; init; } = [];
}

public sealed record RandomChanceEffect(int Numerator, int Denominator, ImmutableArray<CardEffect> Then) : CardEffect;
public sealed record JoustEffect(ImmutableArray<CardEffect> OnWin, bool DrawRevealedMinion = false) : CardEffect
{
    public ImmutableArray<CardEffect> Otherwise { get; init; } = [];
}

public sealed record ReturnSourceCardEffect : CardEffect;
public sealed record SummonMatchingCostEffect(bool UseSource = false) : CardEffect
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool UsePlayedCost { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int DurabilityLossOnSummon { get; init; }
}
public sealed record ShufflePlayedCardEffect(CardRecipient Recipient) : CardEffect;
public sealed record TransformFriendlyMinionsByCostEffect(int Increase) : CardEffect;
public sealed record RecastDeathrattleEffect : CardEffect;
public sealed record DestroyMatchingMinionsEffect(int MaximumAttack) : CardEffect;
public sealed record ResurrectMatchingMinionsEffect(CardFilter CardFilter, int Count = 7,
    bool IncludeOpponent = false) : CardEffect;
public sealed record EmpowerCardEffect(string CardId, StatModifier? Modifier, string EnchantmentCardId) : CardEffect
{
    public GAME_TAG? GrantedTag { get; init; }
    public bool FromSelectedStats { get; init; }
    public bool ReshuffleIfDead { get; init; }
}
public sealed record DamageSplitByAttackEffect : CardEffect;
public sealed record SummonAtTurnEndEffect(string CardId, string EnchantmentCardId) : CardEffect;
public sealed record DestroyRandomFriendlyMinionEffect : CardEffect;
public sealed record CopyOpponentHeroPowerEffect : CardEffect;
public sealed record EnchantByEventAmountEffect(StatTarget Target, int AttackPerUnit, string EnchantmentCardId) : CardEffect;
public sealed record ReplaceHandAndDeckEffect(CardFilter CardFilter) : CardEffect;
public enum ManaSpendReward { BuffSource, DamageSelected, HealSelected, SummonMatchingCost, SummonTokens }
public sealed record SpendRemainingManaEffect(ManaSpendReward Reward, string? TokenCardId = null, string? EnchantmentCardId = null) : CardEffect;
public sealed record CastRandomSpellEffect(int MinimumPrintedCost = 0, bool ForEachSpellCast = false) : CardEffect;
public sealed record CopyOrSwapMinionStatsEffect(bool Swap = false, string? EnchantmentCardId = null) : CardEffect;
public sealed record HealingDealsDamageThisTurnEffect(string EnchantmentCardId) : CardEffect;
public sealed record MergeMatchingMinionsEffect(string ResultCardId, int Count) : CardEffect;
public sealed record EquipRandomWeaponEffect : CardEffect;
public sealed record DestroyMinionsAndDrawEffect : CardEffect;
public sealed record DestroySecretsAndBuffEffect(string EnchantmentCardId) : CardEffect;
public sealed record HealPerEnemyMinionEffect(int Amount) : CardEffect;
public sealed record RefreshAttackEffect : CardEffect;
public sealed record RewardSpellCastsThisTurnEffect(TAG_CLASS CardClass, string EnchantmentCardId) : CardEffect;
public sealed record HeroPowerUpgrade(string From, string To);
public sealed record UpgradeHeroPowerEffect(ImmutableArray<HeroPowerUpgrade> Upgrades) : CardEffect;
public sealed record ReturnDiscardedCardsEffect(string EnchantmentCardId) : CardEffect;
public sealed record CopyDeathrattleEffect(string EnchantmentCardId) : CardEffect;
public sealed record CopySpellOntoSelfEffect : CardEffect;
public sealed record TransformHandCardEffect(CardFilter CardFilter, string EnchantmentCardId) : CardEffect;
public sealed record ClassHeroPower(TAG_CLASS CardClass, string HeroPowerCardId);
public sealed record ReplaceClassCardsEffect(TAG_CLASS OriginalClass, ImmutableArray<ClassHeroPower> Options,
    int CostDelta, string EnchantmentCardId) : CardEffect;
public sealed record EnchantMinionsByTypeEffect(ImmutableArray<MinionType> Types, EnchantStatsEffect Enchantment) : CardEffect;
public sealed record DestroyAndCaptureMinionEffect(EnchantStatsEffect Enchantment) : CardEffect;
public sealed record ResummonCapturedMinionEffect : CardEffect;
public sealed record AddUniqueClassMinionsToDeckEffect(CardFilter CardFilter, int Count) : CardEffect;

public sealed record CardPlayChoice(string Id, ImmutableArray<CardEffect> Effects)
{
    public PlayChoiceRule? ChooseAfterPlay { get; init; }
    public ImmutableArray<PlayReq> PlayReqs { get; init; } = [];
    public TargetRequirement Target { get; init; }
    public TargetFilter? TargetFilter { get; init; }
    public PlayRequirements Requirements { get; init; }
}

/// <summary>地标激活时的目标与结算规则。</summary>
public sealed record LocationAbility(ImmutableArray<CardEffect> Effects)
{
    public TargetRequirement Target { get; init; }
    public TargetFilter? TargetFilter { get; init; }
    public bool RemoveBeforeFinalEffects { get; init; }
}

/// <summary>完成出牌效果后开启的待决选择。</summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(DiscoverMinionChoice), "discoverMinion")]
[JsonDerivedType(typeof(AdaptChoice), "adapt")]
[JsonDerivedType(typeof(DiscoverCardChoice), "discoverCard")]
public abstract record PlayChoiceRule;

public sealed record DiscoverMinionChoice(int SummonCopies, bool LegendaryOnly = false) : PlayChoiceRule;
public sealed record DiscoverCardChoice(CardFilter? Filter = null) : PlayChoiceRule
{
    public ImmutableArray<string> Options { get; init; } = [];
    public EffectCondition? Condition { get; init; }
    public bool ReplaceHeroPower { get; init; }
    public int CostDelta { get; init; }
    public string? EnchantmentCardId { get; init; }
    public StatModifier? Modifier { get; init; }
    public DiscoverCardChoice? Then { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool HealHeroByChosenCost { get; init; }
}
public enum AdaptRecipient { Source, Selected, FriendlyMinions }
public sealed record AdaptChoice(ImmutableArray<string> Options, AdaptRecipient Recipient = AdaptRecipient.Source,
    CardFilter? RecipientFilter = null, int Times = 1) : PlayChoiceRule
{
    internal static ImmutableArray<string> StandardOptions { get; } =
        ["UNG_999t2", "UNG_999t3", "UNG_999t4", "UNG_999t5", "UNG_999t6",
            "UNG_999t7", "UNG_999t8", "UNG_999t10", "UNG_999t13", "UNG_999t14"];
}

public sealed record WeaponMinionAttackRule(int AttackLoss = 1, string? EnchantmentCardId = null);

/// <summary>与卡牌印刷属性分开定义的结算规则。</summary>
public record CardRules
{
    public PlayReqSet? PlayReqs { get; init; }
    public TargetRequirement Target { get; init; }
    public TargetFilter? TargetFilter { get; init; }
    public PlayRequirements Requirements { get; init; }
    public int MinimumEnemyMinions { get; init; }
    public int MinimumTotalMinions { get; init; }
    public bool IsSecret { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool LeavesNoCorpse { get; init; }
    public ImmutableArray<CardEffect> OnPlay { get; init; } = [];
    public ImmutableArray<CardPlayChoice> Choices { get; init; } = [];
    public CardPlayChoice? CombinedChoice { get; init; }
    public bool DoNotCombineChoices { get; init; }
    public PlayChoiceRule? ChooseAfterPlay { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsPlayChoiceOption { get; init; }
    public ImmutableArray<CardEffect> Deathrattle { get; init; } = [];
    public ImmutableArray<CardEffect> WhenDrawn { get; init; } = [];
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ImmutableArray<CardEffect> StartOfGame { get; init; } = [];
    public ImmutableArray<CardEffect>? WhenDiscarded { get; init; }
    public ImmutableArray<CardTrigger> Triggers { get; init; } = [];
    public ImmutableArray<CardTrigger> InHandTriggers { get; init; } = [];
    public ImmutableArray<StatAura> Auras { get; init; } = [];
    public ImmutableArray<WeaponAttackAura> WeaponAuras { get; init; } = [];
    public ImmutableArray<CostAura> CostAuras { get; init; } = [];
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public SelfCostReduction SelfCostReduction { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool HealingDealsDamage { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool CannotAttack { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool AttackEqualsHealth { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool HeroImmuneWhileAttacking { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool FriendlyHeroImmune { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool AllowHeroPowerTargetMinions { get; init; }
    public int? HeroPowerUsesAllowed { get; init; }
    public int? SpellDamageBonusMultiplier { get; init; }
    public int? FriendlyHeroPowerDamage { get; init; }
    public int? BothPlayersSpellDamage { get; init; }
    public bool SpellsHaveLifesteal { get; init; }
    public bool CannotAttackHeroes { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool UnlimitedHeroAttacks { get; init; }
    public bool RequiresHeroAttacked { get; init; }
    public bool AttacksOnlyIfLoneMinion { get; init; }
    public string? AllowAttackEnchantmentCardId { get; init; }
    public int? FriendlyHeroDamageMultiplier { get; init; }
    public int? FriendlyHeroDamageCap { get; init; }
    public bool RedirectFriendlyHeroDamageToSelf { get; init; }
    public string? SetHeroPowerDrawCostsToZeroEnchantment { get; init; }
    public ModifyWeaponEffect? ReplaceHeroPowerEquipEffect { get; init; }
    public MinionType? SummonedMinionTypeDiscount { get; init; }
    public bool DoubleFriendlyBattlecries { get; init; }
    public bool CombineFriendlyChoices { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool CleaveAdjacentOnAttack { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool MegaWindfury { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool AttackWrongEnemyChance { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool AllMinionsAttackWrongEnemyChance { get; init; }
    public MinionType? AttackPerOtherFriendlyMinionType { get; init; }
    public WeaponMinionAttackRule? MinionAttackRule { get; init; }
    // CardDefs 的 SPELLPOWER 标记关键词，实际加成由单卡声明。
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int SpellDamage { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? SpellAndHeroPowerMultiplier { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool DoubleFriendlyDeathrattles { get; init; }
    public LocationAbility? Activation { get; init; }
}

public sealed record SimulationCard : CardRules
{
    private static readonly CardRules EmptyRules = new();

    public SimulationCard(CardDefinition Definition) => this.Definition = Definition;

    internal SimulationCard(CardDefinition definition, CardRules rules) : base(rules) => Definition = definition;

    public CardDefinition Definition { get; init; }
    public CardKeywords Keywords { get; init; }
    public bool EffectPending { get; init; }
    public int RequirementTagMask { get; init; }
    public int Overload { get; init; }

    public void Deconstruct(out CardDefinition definition) => definition = Definition;

    public static SimulationCard FromMetadata(CardMetadata metadata, CardRules? rules = null)
    {
        if (metadata is null)
            throw new ArgumentNullException(nameof(metadata), "必须提供卡牌资料。");

        var definition = metadata.ToDefinition();
        // 随从也可能带此标签，其非施放效果仍由单卡的 WhenDrawn 定义。
        var castsWhenDrawn = definition.Kind == EntityKind.Spell && metadata.GetTag(GAME_TAG.CASTS_WHEN_DRAWN) > 0;
        var keywords = (metadata.GetTag(GAME_TAG.WINDFURY) > 0 ? CardKeywords.Windfury : CardKeywords.None) |
            (metadata.GetTag(GAME_TAG.TAUNT) > 0 ? CardKeywords.Taunt : CardKeywords.None) |
            (metadata.GetTag(GAME_TAG.DIVINE_SHIELD) > 0 ? CardKeywords.DivineShield : CardKeywords.None) |
            (metadata.GetTag(GAME_TAG.CHARGE) > 0 ? CardKeywords.Charge : CardKeywords.None) |
            (metadata.GetTag(GAME_TAG.RUSH) > 0 ? CardKeywords.Rush : CardKeywords.None) |
            (metadata.GetTag(GAME_TAG.POISONOUS) > 0 ? CardKeywords.Poisonous : CardKeywords.None) |
            (metadata.GetTag(GAME_TAG.LIFESTEAL) > 0 ? CardKeywords.Lifesteal : CardKeywords.None) |
            (metadata.GetTag(GAME_TAG.OVERHEAL) > 0 ? CardKeywords.Overheal : CardKeywords.None) |
            (castsWhenDrawn ? CardKeywords.CastsWhenDrawn : CardKeywords.None);
        return new(definition, rules ?? EmptyRules) { Keywords = keywords, RequirementTagMask = PlayReqTags.FromMetadata(metadata), Overload = metadata.GetTag(GAME_TAG.OVERLOAD) };
    }

    internal CharacterState CreateCharacter() => new(Definition.PrintedAttack, Definition.PrintedHealth,
        Exhausted: Definition.Kind == EntityKind.Minion,
        DivineShield: Keywords.HasFlag(CardKeywords.DivineShield));

    internal WeaponStats CreateWeapon() => new(Definition.PrintedAttack, Definition.PrintedDurability, Definition.PrintedDurability);
}
