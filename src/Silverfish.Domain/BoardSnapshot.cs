using System.Collections.Immutable;

namespace Silverfish.Domain;

/// <summary>实体的不可变快照。各分支可安全共享相同的快照值。</summary>
public sealed class BoardSnapshot
{
    private readonly ImmutableDictionary<EntityId, EntityState> _entities;

    private BoardSnapshot(ImmutableDictionary<EntityId, EntityState> entities)
        => _entities = entities;

    public int Count => _entities.Count;

    public IEnumerable<EntityState> Entities => _entities.Values;

    public static BoardSnapshot Create(IEnumerable<EntityState> entities)
    {
        if (entities is null)
            throw new ArgumentNullException(nameof(entities), "必须提供快照实体集合。");
        var builder = ImmutableDictionary.CreateBuilder<EntityId, EntityState>();
        foreach (var entity in entities)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity), "快照实体不能为空。");
            entity.Validate();
            if (builder.ContainsKey(entity.Id))
                throw new ArgumentException($"实体 ID {entity.Id} 重复。");
            builder.Add(entity.Id, entity);
        }
        return new BoardSnapshot(builder.ToImmutable());
    }

    public EntityState Get(EntityId id) =>
        _entities.TryGetValue(id, out var entity)
            ? entity
            : throw new KeyNotFoundException($"此快照中找不到实体 {id}。");

    public bool TryGet(EntityId id, out EntityState? entity)
        => _entities.TryGetValue(id, out entity);

    public BoardSnapshot Replace(EntityState entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity), "必须提供要替换的实体。");
        entity.Validate();
        if (!_entities.ContainsKey(entity.Id))
            throw new KeyNotFoundException($"无法替换快照中缺失的实体 {entity.Id}。");
        return new BoardSnapshot(_entities.SetItem(entity.Id, entity));
    }

    public BoardSnapshot Move(EntityId id, EntityZone zone, int position)
    {
        var existing = Get(id);
        return Replace(existing with { Zone = zone, Position = position });
    }

    public BoardSnapshot Remove(EntityId id)
    {
        if (!_entities.ContainsKey(id))
            throw new KeyNotFoundException($"无法移除快照中缺失的实体 {id}。");
        return new BoardSnapshot(_entities.Remove(id));
    }
}
