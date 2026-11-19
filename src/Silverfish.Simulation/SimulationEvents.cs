using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

public enum SimulationEventKind
{
    TurnEnded, TurnStarted, CardDrawn, CardBurned, Fatigue, CardPlayed, Summoned, ControlChanged,
    Attack, Damage, ShieldBroken, Died, Deathrattle, MatchEnded, Triggered, ArmorGained, DrawnEffect,
    EnchantmentApplied, EnchantmentExpired, Silenced
}

public sealed record SimulationEvent(SimulationEventKind Kind, PlayerId? Player = null,
    EntityId? Source = null, EntityId? Target = null, int? Amount = null, TriggerTiming? Timing = null);

public sealed record SimulationResult(GameState State, ImmutableArray<SimulationEvent> Events);
