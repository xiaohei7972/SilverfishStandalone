using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Silverfish.Domain;

namespace Silverfish.Simulation;

[Flags]
public enum CardKeywords { None = 0, Taunt = 1, Charge = 2, DivineShield = 4, Rush = 8, Windfury = 16 }

public enum TargetRequirement { None, Character, EnemyCharacter, EnemyMinion, FriendlyMinion, Minion }

[Flags]
public enum PlayRequirements { None = 0, BoardSpace = 1, FriendlyMinionDeath = 2 }

public enum DamageTarget { Selected, EnemyHero, AllMinions, Source, TriggerSubject, FriendlyHero, EnemyMinions, AllEnemies }
public enum FreezeTarget { Selected, EnemyMinions, TriggerSubject }
public enum EffectTarget { Selected, TriggerSubject }
public enum RandomCharacterPool { EnemyCharacters, EnemyMinions, OtherCharacters }
public enum RandomDamageMode { Sequential, DistinctBatch }
public enum ManaOperation { GainTemporary, GainEmpty, GainFilled, Refresh, Destroy }
public enum ManaRecipient { Controller, BothPlayers }
public enum SummonPlacement { Default, Rightmost }
public enum StatTarget { Selected, Source, FriendlyMinions, AllMinions, FriendlyHero }
public enum EnchantmentDuration { Permanent, CurrentTurn }
public enum AuraRecipients { OtherFriendlyMinions, AdjacentFriendlyMinions }

public sealed record StatAura(AuraRecipients Recipients, StatModifier Modifier);

public enum TriggerTiming
{
    TurnStarted, TurnEnded, WhenMinionSummoned, AfterMinionSummoned,
    BeforeSpellResolved, AfterSpellResolved, MinionDied, Damage, CardDrawn, HeroAttacking, WeaponDestroyed, AfterHeroPowerUsed,
    SpellCounterWindow, AttackProposed, AfterMinionPlayed
}

public enum TriggerRelation { Friendly, Opponent, Any }
public enum TriggerFocus { EventTarget, DamageSource, AttackDefender }
public enum TriggerEntityMatch { Any, Self, Other }

public sealed record CardTrigger(TriggerTiming Timing, TriggerRelation Relation, ImmutableArray<CardEffect> Effects)
{
    public TriggerFocus Focus { get; init; }
    public TriggerEntityMatch EntityMatch { get; init; }
    public EntityKind? EntityKind { get; init; }
    public bool RequireLivingEntity { get; init; }
    public bool RequiresBoardSpace { get; init; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(DamageEffect), "damage")]
[JsonDerivedType(typeof(DrawEffect), "draw")]
[JsonDerivedType(typeof(GainArmorEffect), "gainArmor")]
[JsonDerivedType(typeof(HealFriendlyHeroEffect), "healFriendlyHero")]
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
public abstract record CardEffect;

public sealed record DamageEffect(DamageTarget Target, int Amount) : CardEffect;
public sealed record DrawEffect(int Count) : CardEffect;
public sealed record GainArmorEffect(int Amount) : CardEffect;
public sealed record HealFriendlyHeroEffect(int Amount) : CardEffect;
public sealed record EquipWeaponEffect(string CardId) : CardEffect;
public sealed record DestroyEnemyWeaponEffect : CardEffect;
public sealed record EnchantStatsEffect(StatTarget Target, StatModifier Modifier,
    EnchantmentDuration Duration = EnchantmentDuration.Permanent) : CardEffect;
public sealed record SilenceEffect : CardEffect;
/// <summary>亡语召唤沿用死亡位置，其他情况放到场上最右侧。</summary>
public sealed record SummonEffect(string CardId, SummonPlacement Placement = SummonPlacement.Default) : CardEffect;
public sealed record TakeControlEffect : CardEffect;
public sealed record ReturnMinionEffect(int CostDelta = 0, EffectTarget Target = EffectTarget.Selected) : CardEffect;
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

public sealed record CardPlayChoice(string Id, ImmutableArray<CardEffect> Effects);

public sealed record SimulationCard(CardDefinition Definition)
{
    public CardKeywords Keywords { get; init; }
    public TargetRequirement Target { get; init; }
    public PlayRequirements Requirements { get; init; }
    public int MinimumEnemyMinions { get; init; }
    public int MinimumTotalMinions { get; init; }
    public int Overload { get; init; }
    public bool IsSecret { get; init; }
    public ImmutableArray<CardEffect> OnPlay { get; init; } = [];
    public ImmutableArray<CardPlayChoice> Choices { get; init; } = [];
    public ImmutableArray<CardEffect> Deathrattle { get; init; } = [];
    public ImmutableArray<CardEffect> WhenDrawn { get; init; } = [];
    public ImmutableArray<CardTrigger> Triggers { get; init; } = [];
    public ImmutableArray<StatAura> Auras { get; init; } = [];

    internal CharacterState CreateCharacter() => new(Definition.PrintedAttack, Definition.PrintedHealth,
        Exhausted: Definition.Kind == EntityKind.Minion,
        DivineShield: Keywords.HasFlag(CardKeywords.DivineShield));

    internal WeaponStats CreateWeapon() => new(Definition.PrintedAttack, Definition.PrintedDurability, Definition.PrintedDurability);
}
