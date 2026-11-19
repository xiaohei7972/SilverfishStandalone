using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

public enum SimulationEventKind
{
    TurnEnded, TurnStarted, CardDrawn, CardBurned, Fatigue, CardPlayed, Summoned, ControlChanged,
    Attack, Damage, ShieldBroken, Died, Deathrattle, MatchEnded
}

public sealed record SimulationEvent(SimulationEventKind Kind, PlayerId? Player = null,
    EntityId? Source = null, EntityId? Target = null, int? Amount = null);

public sealed record SimulationResult(GameState State, ImmutableArray<SimulationEvent> Events);
