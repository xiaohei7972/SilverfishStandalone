using System.Collections.Immutable;

namespace Silverfish.Domain;

/// <summary>
/// A server-provided EntityChoices packet. This is distinct from OPTION_TARGET,
/// where an Options packet carries one target for a single action.
/// </summary>
public enum EntityChoiceType { Target, General, Mulligan }

public sealed record EntityChoicePacket(
    int ChoiceId,
    EntityChoiceType Type,
    PlayerId Player,
    EntityId? Source,
    int CountMin,
    int CountMax,
    ImmutableArray<EntityId> Entities,
    ImmutableArray<EntityId> UnchoosableEntities)
{
    // Matches the legacy client's EntityChoices.IsSingleChoice() UI branch.
    // CountMax == 0 does NOT mean the user can choose one target.
    public bool IsSingleChoice() => CountMax == 0 || CountMin == 1 && CountMax == 1;

    public void Validate()
    {
        if (ChoiceId < 0 || Player.Value <= 0 || !Enum.IsDefined(Type) ||
            Source is EntityId { Value: < 0 } ||
            CountMin < 0 || CountMax < CountMin || Entities.IsDefault || UnchoosableEntities.IsDefault)
            throw new ArgumentException("选择包的 ID、类型、玩家、数量或实体列表无效。");

        var available = new HashSet<EntityId>();
        foreach (var id in Entities)
            if (id.Value < 0 || !available.Add(id))
                throw new ArgumentException("候选实体 ID 必须非负且不能重复。", nameof(Entities));

        var excluded = new HashSet<EntityId>();
        foreach (var id in UnchoosableEntities)
            if (!available.Contains(id) || !excluded.Add(id))
                throw new ArgumentException("不可选列表必须是候选实体的不重复子集。", nameof(UnchoosableEntities));

        if (CountMin > available.Count - excluded.Count)
            throw new ArgumentException("最少选择数量超过了可选实体数。", nameof(CountMin));
    }
}

public sealed record EntityChoiceSubmission(int ChoiceId, ImmutableArray<EntityId> OrderedEntityIds);

/// <summary>
/// Immutable ordered selection for a CHOICE packet. A click appends a target,
/// or removes it if already selected. The order is the actual submission order.
/// This type validates the packet and selection, but does not apply card effects.
/// </summary>
public sealed class EntityChoiceSelection
{
    private EntityChoiceSelection(EntityChoicePacket packet, ImmutableArray<EntityId> chosen)
    {
        Packet = packet;
        OrderedEntityIds = chosen;
    }

    public EntityChoicePacket Packet { get; }
    public ImmutableArray<EntityId> OrderedEntityIds { get; }

    public static EntityChoiceSelection Begin(EntityChoicePacket packet)
    {
        ArgumentNullException.ThrowIfNull(packet);
        packet.Validate();
        return new(packet, []);
    }

    public bool CanConfirm => OrderedEntityIds.Length >= Packet.CountMin &&
                              OrderedEntityIds.Length <= Packet.CountMax;

    // A necessary condition for client auto-submit, not a dispatch decision:
    // the actual client has additional guards in DoNetworkChoice().
    // An empty packet must never be auto-submitted based only on CountMax == 0.
    public bool IsSingleChoiceSubmissionCandidate =>
        Packet.IsSingleChoice() && OrderedEntityIds.Length == 1 && CanConfirm;

    public EntityChoiceSelection Click(EntityId id)
    {
        if (!Packet.Entities.Contains(id) || Packet.UnchoosableEntities.Contains(id))
            throw new ArgumentException($"实体 {id.Value} 不属于本次允许选择的目标。", nameof(id));

        var index = OrderedEntityIds.IndexOf(id);
        if (index >= 0)
            return new(Packet, OrderedEntityIds.RemoveAt(index));

        if (OrderedEntityIds.Length >= Packet.CountMax)
            throw new InvalidOperationException("选择数量已达到 CountMax。");
        return new(Packet, OrderedEntityIds.Add(id));
    }

    public EntityChoiceSubmission Confirm(int currentChoiceId)
    {
        if (currentChoiceId != Packet.ChoiceId)
            throw new InvalidOperationException("选择包 ID 已变化，不能提交过期的目标序列。");
        if (!CanConfirm)
            throw new InvalidOperationException("所选目标数未满足 CountMin / CountMax。");
        return new(Packet.ChoiceId, OrderedEntityIds);
    }
}

/// <summary>
/// Server CHOICE state. A submitted choice is an external handshake, not a
/// locally resolved card effect. Normal gameplay awaits a new snapshot.
/// </summary>
public sealed record PendingEntityChoice(EntityChoicePacket Packet, EntityChoiceSubmission? Submitted = null)
{
    public void Validate()
    {
        if (Packet is null)
            throw new ArgumentException("待决选择必须包含完整选择包。", nameof(Packet));
        Packet.Validate();
        if (Submitted is { } submission)
            _ = ValidateSubmission(submission.ChoiceId, submission.OrderedEntityIds);
    }

    public PendingEntityChoice Submit(int choiceId, ImmutableArray<EntityId> ordered)
    {
        if (Submitted is not null)
            throw new InvalidOperationException("本次选择已经提交，必须等待新快照才能继续。");
        var confirmed = ValidateSubmission(choiceId, ordered);
        return this with { Submitted = confirmed };
    }

    private EntityChoiceSubmission ValidateSubmission(int choiceId, ImmutableArray<EntityId> ordered)
    {
        if (ordered.IsDefault)
            throw new ArgumentException("目标列表尚未初始化。", nameof(ordered));
        var selection = EntityChoiceSelection.Begin(Packet);
        var unique = new HashSet<EntityId>();
        foreach (var target in ordered)
        {
            if (!unique.Add(target))
                throw new ArgumentException("提交目标不得重复。", nameof(ordered));
            selection = selection.Click(target);
        }
        return selection.Confirm(choiceId);
    }
}
