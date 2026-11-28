using System.Text.Json.Serialization;
using Silverfish.Domain.Client;

namespace Silverfish.Domain;

public enum StatModifierMode { Add, Set }

public sealed record StatModifier(StatModifierMode Mode, int? Attack = null, int? MaximumHealth = null);

public sealed record StatEnchantment(EntityId Source, StatModifier? Modifier, int? ExpiresAfterTurn = null)
{
    public string? CardId { get; init; }
    public GAME_TAG? GrantedTag { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CapturedMinion? CapturedMinion { get; init; }
}

public sealed record CapturedMinion(string CardId, PlayerId Controller);

public sealed record StatAuraGrant(EntityId Source, int Index, StatModifier? Modifier)
{
    public string? CardId { get; init; }
    public GAME_TAG? GrantedTag { get; init; }
}
