using System.Collections.Immutable;

namespace Silverfish.Domain;

public enum SimulationActionKind { EndTurn, PlayCard, Attack, UseHeroPower, ActivateLocation, ChooseOption }

public sealed record SimulationAction(SimulationActionKind Kind, PlayerId Actor,
    EntityId? Source = null, EntityId? Target = null, int? Position = null)
{
    public string? Choice { get; init; }
    /// <summary>Server packet ID. No relation to PlayCard.Choice.</summary>
    public int? ChoicePacketId { get; init; }
    public ImmutableArray<EntityId> OrderedChoiceTargets { get; init; } = ImmutableArray<EntityId>.Empty;
}
