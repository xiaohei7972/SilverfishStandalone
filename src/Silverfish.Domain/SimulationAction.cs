namespace Silverfish.Domain;

public enum SimulationActionKind { EndTurn, PlayCard, Attack, UseHeroPower, ActivateLocation, ChooseOption }

public sealed record SimulationAction(SimulationActionKind Kind, PlayerId Actor,
    EntityId? Source = null, EntityId? Target = null, int? Position = null)
{
    public string? Choice { get; init; }
}
