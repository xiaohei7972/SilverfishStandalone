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
        ArgumentNullException.ThrowIfNull(entities);
        var builder = ImmutableDictionary.CreateBuilder<EntityId, EntityState>();
        foreach (var entity in entities)
        {
            ArgumentNullException.ThrowIfNull(entity);
            entity.Validate();
            if (builder.ContainsKey(entity.Id))
                throw new ArgumentException($"Duplicate entity ID {entity.Id}.");
            builder.Add(entity.Id, entity);
        }
        return new BoardSnapshot(builder.ToImmutable());
    }

    public EntityState Get(EntityId id) =>
        _entities.TryGetValue(id, out var entity)
            ? entity
            : throw new KeyNotFoundException($"Entity {id} is not in this snapshot.");

    public bool TryGet(EntityId id, out EntityState? entity)
        => _entities.TryGetValue(id, out entity);

    public BoardSnapshot Replace(EntityState entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.Validate();
        if (!_entities.ContainsKey(entity.Id))
            throw new KeyNotFoundException($"Cannot replace absent entity {entity.Id}.");
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
            throw new KeyNotFoundException($"Cannot remove absent entity {id}.");
        return new BoardSnapshot(_entities.Remove(id));
    }
}
