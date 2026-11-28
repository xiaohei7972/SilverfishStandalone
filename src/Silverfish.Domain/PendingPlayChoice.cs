using System.Collections.Immutable;

namespace Silverfish.Domain;

public sealed record PlayChoiceListener(EntityId Entity, string CardId, PlayerId Controller, long PlayOrder, GameZone Zone);

/// <summary>出牌已支付并完成前置结算，等待玩家从独立候选实体中选一张。</summary>
public sealed record PendingPlayChoice(EntityId Source, ImmutableArray<EntityId> Options,
    ImmutableArray<PlayChoiceListener> AfterPlayListeners)
{
    public ImmutableArray<EntityId>? Recipients { get; init; }
    public int? RemainingSelections { get; init; }
    public string? PlayBranch { get; init; }
    public EntityId? SelectedTarget { get; init; }
    public int? PlayedCardCost { get; init; }
    public int? DiscoverRound { get; init; }
}
