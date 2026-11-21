using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

public enum SimulationEventKind
{
    TurnEnded, TurnStarted, CardDrawn, CardBurned, Fatigue, CardPlayed, Summoned, ControlChanged,
    Attack, Damage, ShieldBroken, Died, Deathrattle, MatchEnded, Triggered, ArmorGained, DrawnEffect,
    EnchantmentApplied, EnchantmentExpired, Silenced, WeaponEquipped, WeaponDurabilityLost,
    WeaponDestroyed, HeroPowerUsed, Healed, ReturnedToHand, Transformed, Resurrected,
    Frozen, Thawed, OverloadScheduled, OverloadUnlocked, SecretArmed, SecretRevealed, SpellCountered, AttackRedirected,
    AttackCancelled, TemporaryManaGained, ManaCrystalsGained, ManaCrystalsDestroyed, ManaRefreshed, CardGenerated,
    LocationActivated, LocationHealthLost, LocationDestroyed, CardDiscarded, DiscardedEffect, Recruited, SpellCastWhenDrawn,
    CardAddedToDeck, DeckShuffled, AlternateCostPaid, CostTypeChanged, PlayChoiceOffered, PlayChoiceSelected, ChoiceSubmitted, Overhealed
}

public sealed record SimulationEvent(SimulationEventKind Kind, PlayerId? Player = null,
    EntityId? Source = null, EntityId? Target = null, int? Amount = null, TriggerTiming? Timing = null)
{
    public TAG_CARD_ALTERNATE_COST? CostType { get; init; }
    public string? EnchantmentCardId { get; init; }
    public int? ChoiceId { get; init; }
    public ImmutableArray<EntityId>? OrderedChoiceTargets { get; init; }
    /// <summary>Equipped weapon responsible for this hero combat hit; Source remains the attacking hero.</summary>
    public EntityId? AttackingWeapon { get; init; }
    /// <summary>Damage actually assigned to armor, before health.</summary>
    public int? DamageToArmor { get; init; }
    /// <summary>Damage assigned to health (including lethal excess).</summary>
    public int? DamageToHealth { get; init; }
    /// <summary>Damage exceeded the character's previous Health on the source's turn.</summary>
    public bool IsOverkill { get; init; }
    /// <summary>Damage equaled the character's previous Health on the source's turn.</summary>
    public bool IsHonorableKill { get; init; }
}

public sealed record SimulationResult(GameState State, ImmutableArray<SimulationEvent> Events);

public sealed record SimulationOutcome(SimulationResult Result, double Probability);

public sealed record SimulationDistribution(ImmutableArray<SimulationOutcome> Outcomes, int Resolutions, bool IsExact);
