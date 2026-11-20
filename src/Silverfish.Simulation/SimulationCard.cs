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

public enum DamageTarget { Selected, EnemyHero, AllMinions, Source, TriggerSubject, FriendlyHero, EnemyMinions, AllEnemies }
public enum FreezeTarget { Selected, EnemyMinions, TriggerSubject }
public enum HealTarget { Selected, Source, TriggerSubject, FriendlyHero, EnemyHero, FriendlyMinions, AllMinions, AllFriendlyCharacters }
public enum EffectTarget { Selected, TriggerSubject }
public enum RandomCharacterPool { EnemyCharacters, EnemyMinions, OtherCharacters }
public enum RandomDamageMode { Sequential, DistinctBatch }
public enum ManaOperation { GainTemporary, GainEmpty, GainFilled, Refresh, Destroy }
public enum ManaRecipient { Controller, BothPlayers }
public enum SummonPlacement { Default, Rightmost }
public enum StatTarget { Selected, Source, FriendlyMinions, AllMinions, FriendlyHero }
public enum EnchantmentDuration { Permanent, CurrentTurn }
public enum EnchantmentRemovalReason { Expired, Silenced, LeftPlay, Transformed }
public enum AuraRecipients { OtherFriendlyMinions, AdjacentFriendlyMinions }
public enum CostAuraTarget { Hand, Minion, Spell, Weapon, HeroPower, Location }
public enum CostAuraRecipient { Friendly, Opponent, BothPlayers }
public enum HandCostTarget { Source, DrawnCard, FriendlyHand }
public enum HandRecipient { Controller, Opponent }
public enum DiscardSelection { Random, HighestCost, LowestCost }
public enum CardRecipient { Controller, Opponent }
public enum CardPlacement { Hand, DeckTop, DeckBottom, RandomDeckPosition, ShuffleIntoDeck }

public sealed record StatAura(AuraRecipients Recipients, StatModifier Modifier)
{
    public string? EnchantmentCardId { get; init; }
    public CardFilter? CardFilter { get; init; }
}

/// <summary>对指定玩家的手牌或英雄技能持续增减费用。</summary>
public sealed record CostAura(
    [property: JsonRequired] CostAuraRecipient Recipient,
    [property: JsonRequired] CostAuraTarget Target,
    [property: JsonRequired] int Delta)
{
    public CardFilter? CardFilter { get; init; }
}

public enum TriggerTiming
{
    TurnStarted, TurnEnded, WhenMinionSummoned, AfterMinionSummoned,
    BeforeSpellResolved, AfterSpellResolved, MinionDied, Damage, CardDrawn, HeroAttacking, WeaponDestroyed, AfterHeroPowerUsed,
    SpellCounterWindow, AttackProposed, AfterMinionPlayed, CardDiscarded,
    Overkill, HonorableKill, Overheal
}

public enum TriggerRelation { Friendly, Opponent, Any }
public enum TriggerFocus { EventTarget, DamageSource, AttackDefender }
public enum TriggerEntityMatch { Any, Self, Other }

public sealed record CardTrigger(TriggerTiming Timing, TriggerRelation Relation, ImmutableArray<CardEffect> Effects)
{
    public TriggerFocus Focus { get; init; }
    public TriggerEntityMatch EntityMatch { get; init; }
    public EntityKind? EntityKind { get; init; }
    public CardFilter? CardFilter { get; init; }
    public bool RequireLivingEntity { get; init; }
    public bool RequiresBoardSpace { get; init; }
    public EffectCondition? Condition { get; init; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(DamageEffect), "damage")]
[JsonDerivedType(typeof(DrawEffect), "draw")]
[JsonDerivedType(typeof(DrawMatchingEffect), "drawMatching")]
[JsonDerivedType(typeof(ChangeHandCostEffect), "changeHandCost")]
[JsonDerivedType(typeof(ChangeCostTypeEffect), "changeCostType")]
[JsonDerivedType(typeof(RecruitEffect), "recruit")]
[JsonDerivedType(typeof(CreateCardEffect), "createCard")]
[JsonDerivedType(typeof(DiscardEffect), "discard")]
[JsonDerivedType(typeof(DiscardHandEffect), "discardHand")]
[JsonDerivedType(typeof(SummonDiscardedMinionEffect), "summonDiscardedMinion")]
[JsonDerivedType(typeof(GainArmorEffect), "gainArmor")]
[JsonDerivedType(typeof(HealFriendlyHeroEffect), "healFriendlyHero")]
[JsonDerivedType(typeof(HealEffect), "heal")]
[JsonDerivedType(typeof(EquipWeaponEffect), "equipWeapon")]
[JsonDerivedType(typeof(DestroyEnemyWeaponEffect), "destroyEnemyWeapon")]
[JsonDerivedType(typeof(EnchantStatsEffect), "enchantStats")]
[JsonDerivedType(typeof(SilenceEffect), "silence")]
[JsonDerivedType(typeof(SummonEffect), "summon")]
[JsonDerivedType(typeof(TakeControlEffect), "takeControl")]
[JsonDerivedType(typeof(ReturnMinionEffect), "returnMinion")]
[JsonDerivedType(typeof(TransformMinionEffect), "transformMinion")]
[JsonDerivedType(typeof(DestroyMinionEffect), "destroyMinion")]
[JsonDerivedType(typeof(ResurrectFriendlyMinionEffect), "resurrectFriendlyMinion")]
[JsonDerivedType(typeof(FreezeEffect), "freeze")]
[JsonDerivedType(typeof(UnlockOverloadEffect), "unlockOverload")]
[JsonDerivedType(typeof(CounterSpellEffect), "counterSpell")]
[JsonDerivedType(typeof(SummonAttackDefenderEffect), "summonAttackDefender")]
[JsonDerivedType(typeof(RandomDamageEffect), "randomDamage")]
[JsonDerivedType(typeof(DestroyRandomEnemyMinionEffect), "destroyRandomEnemyMinion")]
[JsonDerivedType(typeof(DestroyAllButOneMinionEffect), "destroyAllButOneMinion")]
[JsonDerivedType(typeof(SummonRandomMinionEffect), "summonRandomMinion")]
[JsonDerivedType(typeof(ChangeManaEffect), "changeMana")]
[JsonDerivedType(typeof(ConditionalEffect), "conditional")]
public abstract record CardEffect;

public sealed record DamageEffect(DamageTarget Target, int Amount) : CardEffect;
public sealed record DrawEffect(int Count) : CardEffect;
public sealed record DrawMatchingEffect([property: JsonRequired] CardFilter CardFilter, int Count = 1) : CardEffect;
public sealed record ChangeHandCostEffect(
    [property: JsonRequired] HandCostTarget Target,
    [property: JsonRequired] int Delta,
    EnchantmentDuration Duration = EnchantmentDuration.Permanent) : CardEffect
{
    public CardFilter? CardFilter { get; init; }
    public string? EnchantmentCardId { get; init; }
}
public sealed record ChangeCostTypeEffect(
    [property: JsonRequired] HandCostTarget Target,
    [property: JsonRequired] TAG_CARD_ALTERNATE_COST Type,
    EnchantmentDuration Duration = EnchantmentDuration.Permanent) : CardEffect
{
    public CardFilter? CardFilter { get; init; }
    public string? EnchantmentCardId { get; init; }
}
public sealed record RecruitEffect(int Count = 1) : CardEffect
{
    public CardFilter? CardFilter { get; init; }
}
public sealed record CreateCardEffect(
    [property: JsonRequired] string CardId,
    CardPlacement Placement = CardPlacement.Hand,
    CardRecipient Recipient = CardRecipient.Controller,
    int Count = 1) : CardEffect;
public sealed record DiscardEffect(int Count = 1, DiscardSelection Selection = DiscardSelection.Random,
    HandRecipient Recipient = HandRecipient.Controller) : CardEffect;
public sealed record DiscardHandEffect(HandRecipient Recipient = HandRecipient.Controller) : CardEffect;
public sealed record SummonDiscardedMinionEffect : CardEffect;
public sealed record GainArmorEffect(int Amount) : CardEffect;
public sealed record HealFriendlyHeroEffect(int Amount) : CardEffect;
/// <summary>Restores character Health; healing past full Health can trigger Overheal.</summary>
public sealed record HealEffect(HealTarget Target, int Amount) : CardEffect;
public sealed record EquipWeaponEffect(string CardId) : CardEffect;
public sealed record DestroyEnemyWeaponEffect : CardEffect;
public sealed record EnchantmentRemovalRule(EnchantmentRemovalReason Reason, ImmutableArray<CardEffect> Effects);
public sealed record EnchantStatsEffect(StatTarget Target, StatModifier Modifier,
    EnchantmentDuration Duration = EnchantmentDuration.Permanent) : CardEffect
{
    public string? EnchantmentCardId { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ImmutableArray<EnchantmentRemovalRule> OnRemoved { get; init; }
}
public sealed record SilenceEffect : CardEffect;
/// <summary>亡语召唤沿用死亡位置，其他情况放到场上最右侧。</summary>
public sealed record SummonEffect(string CardId, SummonPlacement Placement = SummonPlacement.Default) : CardEffect;
public sealed record TakeControlEffect : CardEffect;
public sealed record ReturnMinionEffect(int CostDelta = 0, EffectTarget Target = EffectTarget.Selected) : CardEffect
{
    public string? EnchantmentCardId { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public EnchantmentDuration CostDuration { get; init; }
}
public sealed record TransformMinionEffect(string CardId) : CardEffect;
public sealed record DestroyMinionEffect : CardEffect;
public sealed record ResurrectFriendlyMinionEffect : CardEffect;
public sealed record FreezeEffect(FreezeTarget Target) : CardEffect;
public sealed record UnlockOverloadEffect : CardEffect;
public sealed record CounterSpellEffect : CardEffect;
public sealed record SummonAttackDefenderEffect(string CardId) : CardEffect;
public sealed record RandomDamageEffect(RandomCharacterPool Pool, int Amount, int Count = 1,
    RandomDamageMode Mode = RandomDamageMode.Sequential) : CardEffect;
public sealed record DestroyRandomEnemyMinionEffect : CardEffect;
public sealed record DestroyAllButOneMinionEffect : CardEffect;
public sealed record SummonRandomMinionEffect(ImmutableArray<string> CardIds) : CardEffect;
public sealed record ChangeManaEffect(ManaOperation Operation, int Amount, ManaRecipient Recipient = ManaRecipient.Controller,
    string? OverflowCardId = null) : CardEffect;

public sealed record ConditionalEffect(EffectCondition Condition, ImmutableArray<CardEffect> Then) : CardEffect
{
    public ImmutableArray<CardEffect> Otherwise { get; init; } = [];
}

public sealed record CardPlayChoice(string Id, ImmutableArray<CardEffect> Effects)
{
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

/// <summary>与卡牌印刷属性分开定义的结算规则。</summary>
public record CardRules
{
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
    public ImmutableArray<CardEffect> Deathrattle { get; init; } = [];
    public ImmutableArray<CardEffect> WhenDrawn { get; init; } = [];
    public ImmutableArray<CardEffect>? WhenDiscarded { get; init; }
    public ImmutableArray<CardTrigger> Triggers { get; init; } = [];
    public ImmutableArray<StatAura> Auras { get; init; } = [];
    public ImmutableArray<CostAura> CostAuras { get; init; } = [];
    public LocationAbility? Activation { get; init; }
}

public sealed record SimulationCard : CardRules
{
    private static readonly CardRules EmptyRules = new();

    public SimulationCard(CardDefinition Definition) => this.Definition = Definition;

    internal SimulationCard(CardDefinition definition, CardRules rules) : base(rules) => Definition = definition;

    public CardDefinition Definition { get; init; }
    public CardKeywords Keywords { get; init; }
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
        return new(definition, rules ?? EmptyRules) { Keywords = keywords, Overload = metadata.GetTag(GAME_TAG.OVERLOAD) };
    }

    internal CharacterState CreateCharacter() => new(Definition.PrintedAttack, Definition.PrintedHealth,
        Exhausted: Definition.Kind == EntityKind.Minion,
        DivineShield: Keywords.HasFlag(CardKeywords.DivineShield));

    internal WeaponStats CreateWeapon() => new(Definition.PrintedAttack, Definition.PrintedDurability, Definition.PrintedDurability);
}
