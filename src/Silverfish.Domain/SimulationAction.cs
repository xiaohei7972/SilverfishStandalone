namespace Silverfish.Domain;

public enum SimulationActionKind { EndTurn, PlayCard, Attack }

public sealed record SimulationAction(SimulationActionKind Kind, PlayerId Actor,
    EntityId? Source = null, EntityId? Target = null, int? Position = null);
