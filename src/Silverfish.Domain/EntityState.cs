using System.Collections.Immutable;

namespace Silverfish.Domain;

/// <summary>实体的固定标识。模拟中的卡牌、控制者、区域与属性可以独立变化。</summary>
public readonly record struct EntityId(int Value)
{
    public override string ToString() => Value.ToString();
}

public enum PlayerSide { Unknown, Friendly, Opponent }

public enum EntityZone
{
    Unknown, Deck, Hand, Battlefield, Graveyard, Secret, Weapon,
    SetAside, RemovedFromGame
}

public enum EntityKind
{
    Unknown, Hero, Minion, Spell, Weapon, Location, HeroPower,
    Enchantment, Other
}

/// <summary>不可变的卡牌印刷属性。战斗标签与临时增益记录在实体状态中。</summary>
public sealed record CardDefinition(
    string CardId,
    EntityKind Kind,
    int PrintedCost,
    int PrintedAttack = 0,
    int PrintedHealth = 0,
    int PrintedDurability = 0);

/// <summary>场上角色的战斗属性。英雄和随从共用此结构，由 EntityKind 区分。</summary>
public sealed record CombatStats(
    int Attack,
    int Health,
    int MaximumHealth,
    int Armor = 0,
    int AttacksThisTurn = 0,
    bool Ready = false);

public sealed record ManaCost(int PrintedCost, int CurrentCost);
public sealed record ManaCostModifier(EntityId Source, int Delta);

public sealed record HandStatModifiers(int AttackDelta = 0, int HealthDelta = 0);

/// <summary>已装备武器的战斗属性。武器 ID 由所属实体提供。</summary>
public sealed record WeaponStats(
    int Attack,
    int Durability,
    int MaximumDurability,
    int AttacksThisTurn = 0);

/// <summary>独立的不可变实体。通用标签和计数器可记录新增机制。</summary>
public sealed record EntityState
{
    public required EntityId Id { get; init; }
    public required string CardId { get; init; }
    public required EntityKind Kind { get; init; }
    public required PlayerSide Side { get; init; }
    public required EntityZone Zone { get; init; }
    public int Position { get; init; }

    public CombatStats? Combat { get; init; }
    public ManaCost? Cost { get; init; }
    public HandStatModifiers? HandModifiers { get; init; }
    public WeaponStats? Equipment { get; init; }

    public ImmutableHashSet<string> Keywords { get; init; } =
        ImmutableHashSet.Create<string>(StringComparer.OrdinalIgnoreCase);
    public ImmutableArray<string> EnchantmentCardIds { get; init; } =
        ImmutableArray<string>.Empty;
    public ImmutableDictionary<string, int> Counters { get; init; } =
        ImmutableDictionary<string, int>.Empty;
    public ImmutableDictionary<int, int> RawTags { get; init; } =
        ImmutableDictionary<int, int>.Empty;

    public void Validate()
    {
        if (Id.Value < 0)
            throw new ArgumentException("An entity must have a known non-negative ID.");
        if (string.IsNullOrWhiteSpace(CardId))
            throw new ArgumentException("CardId is required.");
        if (Kind == EntityKind.Unknown || Side == PlayerSide.Unknown || Zone == EntityZone.Unknown)
            throw new ArgumentException("Kind, side and zone must be explicit.");
        if (Position < 0)
            throw new ArgumentException("Zone position cannot be negative.");
        if (Combat is { } c && (c.Attack < 0 || c.Health < 0 ||
            c.MaximumHealth < 1 || c.Health > c.MaximumHealth ||
            c.Armor < 0 || c.AttacksThisTurn < 0))
            throw new ArgumentException("Invalid combat stats.");
        if (Cost is { } cost && (cost.PrintedCost < 0 || cost.CurrentCost < 0))
            throw new ArgumentException("Invalid mana cost.");
        if (Equipment is { } w && (w.Attack < 0 || w.Durability < 0 ||
            w.MaximumDurability < 1 || w.Durability > w.MaximumDurability ||
            w.AttacksThisTurn < 0))
            throw new ArgumentException("Invalid weapon stats.");

        if (Zone == EntityZone.Battlefield &&
            Kind is EntityKind.Hero or EntityKind.Minion && Combat is null)
            throw new ArgumentException("A battlefield hero or minion needs combat stats.");
        if (Zone == EntityZone.Weapon && Equipment is null)
            throw new ArgumentException("An equipped weapon needs weapon stats.");
        if (EnchantmentCardIds.IsDefault)
            throw new ArgumentException("EnchantmentCardIds cannot be an uninitialized array.");
        if (Keywords is null || Counters is null || RawTags is null)
            throw new ArgumentException("Mechanics collections cannot be null.");
    }
}
