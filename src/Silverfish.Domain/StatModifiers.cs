using Silverfish.Domain.Client;

namespace Silverfish.Domain;

public enum StatModifierMode { Add, Set }

public sealed record StatModifier(StatModifierMode Mode, int? Attack = null, int? MaximumHealth = null);

public sealed record StatEnchantment(EntityId Source, StatModifier? Modifier, int? ExpiresAfterTurn = null)
{
    public string? CardId { get; init; }
    public GAME_TAG? GrantedTag { get; init; }
}

public sealed record StatAuraGrant(EntityId Source, int Index, StatModifier? Modifier)
{
    public string? CardId { get; init; }
    public GAME_TAG? GrantedTag { get; init; }
}
