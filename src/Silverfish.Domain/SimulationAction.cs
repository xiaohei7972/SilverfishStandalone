namespace Silverfish.Domain;

public enum SimulationActionKind { EndTurn, PlayCard, Attack, UseHeroPower }

public sealed record SimulationAction(SimulationActionKind Kind, PlayerId Actor,
    EntityId? Source = null, EntityId? Target = null, int? Position = null);
