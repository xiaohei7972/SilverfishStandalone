using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

/// <summary>
/// Explicit, locally verified rules used for an OFFLINE counterfactual CHOICE preview.
/// No prediction is permitted when an offered option lacks a known effect.
/// Never treat these rules as the result of an authoritative client snapshot.
/// </summary>
public sealed record ChoicePreviewRules(int ChoiceId, EntityChoiceType Type)
{
    /// <summary>Effects applied to each selected on-board character, in submitted order (TARGET).</summary>
    public ImmutableArray<CardEffect> PerTargetEffects { get; init; } = [];

    /// <summary>Explicit effects per offered entity, in submitted order (GENERAL / Discover).</summary>
    public ImmutableDictionary<EntityId, ImmutableArray<CardEffect>> PerOptionEffects { get; init; } =
        ImmutableDictionary<EntityId, ImmutableArray<CardEffect>>.Empty;
}

/// <summary>The state is hypothetical and keeps the submitted pending CHOICE as a barrier.</summary>
public sealed record ChoicePreviewResult(GameState State, ImmutableArray<SimulationEvent> Events);
