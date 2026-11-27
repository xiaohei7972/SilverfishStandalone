using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Silverfish.Domain.Client;

namespace Silverfish.Domain;

/// <summary>实体的固定标识。模拟中的卡牌、控制者、区域与属性可以独立变化。</summary>
public readonly record struct EntityId(int Value)
{
    public override string ToString() => Value.ToString();
}

public sealed record CharacterState(int Attack, int MaximumHealth, int Damage = 0, int Armor = 0,
    int AttacksThisTurn = 0, bool Exhausted = false, bool DivineShield = false)
{
    public int? BaseAttack { get; init; }
    public int? BaseMaximumHealth { get; init; }
    public bool Frozen { get; init; }
    public int Health => MaximumHealth - Damage;
}

/// <summary>已装备武器的战斗属性。武器 ID 由所属实体提供。</summary>
public sealed record WeaponStats(
    int Attack,
    int Durability,
    int MaximumDurability,
    int AttacksThisTurn = 0);

/// <summary>地标剩余次数与距离再次可用还需经过的己方回合开始次数。</summary>
public sealed record LocationState(int Health, int TurnsUntilReady = 0)
{
    internal const int StartsUntilReadyAfterUse = 2;
}

public sealed record GameEntity(EntityId Id, string CardId, PlayerId Owner, PlayerId Controller)
{
    public string? CopiedSpellCardId { get; init; }
    public bool? Stealthed { get; init; }
    public CharacterState? Character { get; init; }
    public WeaponStats? Weapon { get; init; }
    public LocationState? Location { get; init; }
    public long? PlayOrder { get; init; }
    public bool Silenced { get; init; }
    public ImmutableArray<StatEnchantment> Enchantments { get; init; } = [];
    public ImmutableArray<StatAuraGrant> AuraGrants { get; init; } = [];
    public ImmutableArray<ManaCostModifier> CostModifiers { get; init; } = [];
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ImmutableArray<CostTypeModifier>? CostTypeModifiers { get; init; }
}

/// <summary>手牌费用数值的修正。未设置到期回合时持续到卡牌离开手牌区。</summary>
public sealed record ManaCostModifier(EntityId Source, int Delta)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CardId { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ExpiresAfterTurn { get; init; }
}

/// <summary>手牌支付方式的修正。未设置到期回合时持续到卡牌离开手牌区。</summary>
public sealed record CostTypeModifier(EntityId Source, TAG_CARD_ALTERNATE_COST Type, int? ExpiresAfterTurn = null)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CardId { get; init; }
}

public enum CostModifierMode { Add, Set }

/// <summary>挂在玩家身上的临时或单次生效费用光环（例如肯瑞托法师、伺机待发、米尔豪斯）。</summary>
public sealed record TemporaryCostAura(
    EntityId Source,
    EntityKind TargetKind,
    CostModifierMode Mode,
    int Amount,
    int ActiveOnTurn,
    bool SecretOnly = false,
    bool ConsumedOnPlay = false,
    string? CardId = null);
