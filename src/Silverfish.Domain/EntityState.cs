using System.Collections.Immutable;

namespace Silverfish.Domain;

/// <summary>
/// A stable entity identity. An entity's card ID, controller, zone and stats
/// can change independently of this identity during a simulation.
/// </summary>
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

/// <summary>
/// Printed CardDB-like data is immutable and never stores current battle tags.
/// Card effects must not mutate these definitions to implement a one-off buff.
/// </summary>
public sealed record CardDefinition(
    string CardId,
    EntityKind Kind,
    int PrintedCost,
    int PrintedAttack = 0,
    int PrintedHealth = 0,
    int PrintedDurability = 0);

/// <summary>State of an in-play character. A hero and a minion share this component,
/// but are distinguished by EntityKind rather than being the same Minion object.</summary>
public sealed record CombatStats(
    int Attack,
    int Health,
    int MaximumHealth,
    int Armor = 0,
    int AttacksThisTurn = 0,
    bool Ready = false);

public sealed record ManaCost(int PrintedCost, int CurrentCost);

public sealed record HandStatModifiers(int AttackDelta = 0, int HealthDelta = 0);

/// <summary>Equipped weapon state. An entity ID comes from the entity, not from this component.</summary>
public sealed record WeaponStats(
    int Attack,
    int Durability,
    int MaximumDurability,
    int AttacksThisTurn = 0);

/// <summary>
/// Independent, immutable simulator entity. Never holds a reference to an old
/// Minion, Handcard, Weapon or mutable CardDB.Card object.
/// RawTags and Counters retain new mechanics without adding dozens of booleans.
/// </summary>
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
