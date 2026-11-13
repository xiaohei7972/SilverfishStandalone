using System.Collections.Concurrent;
using System.Diagnostics;

namespace HREngine.Bots;

/// <summary>Headless logging boundary replacing the old Triton/log4net host logger.</summary>
public interface ILog
{
    void Info(object? message);
    void Debug(object? message);
    void InfoFormat(string format, params object?[] values);
}

public static class Logger
{
    private sealed class TraceLogger : ILog
    {
        public void Info(object? message) => Trace.WriteLine(message);
        public void Debug(object? message) => Trace.WriteLine(message);
        public void InfoFormat(string format, params object?[] values) =>
            Trace.WriteLine(string.Format(format, values));
    }

    private static readonly ILog Shared = new TraceLogger();
    public static ILog GetLoggerInstanceForType() => Shared;
}

/// <summary>Explicit strategy-file configuration, not a Hearthbuddy singleton.</summary>
public static class ClassicBehaviorPaths
{
    public static Dictionary<string, string> Paths { get; } = new(StringComparer.Ordinal);
}

/// <summary>
/// Detached card entity used for old enchantment metadata. An adapter must populate
/// this registry per snapshot; it never reads a game's process or runtime directly.
/// </summary>
public sealed class Entity
{
    public int Id { get; init; }
    public string CardId { get; init; } = string.Empty;
    public Dictionary<GAME_TAG, int> Tags { get; } = new();
    public List<Entity> Attachments { get; } = new();

    public int GetTag(GAME_TAG tag) => Tags.TryGetValue(tag, out var value) ? value : 0;
    public string GetCardId() => CardId;
    public EntityDefinition GetEntityDef() => new(CardId);
    public List<Entity> GetAttachments() => Attachments;
}

public sealed record EntityDefinition(string CardId)
{
    public string GetCardId() => CardId;
}

// The original classic Silverfish uses the client GAME_TAG enum at a few edges.
// The numeric values below are pinned to its own bundled TAGGS.cs snapshot.
public enum GAME_TAG
{
    TAG_SCRIPT_DATA_NUM_1 = (int)GAME_TAGs.TAG_SCRIPT_DATA_NUM_1,
    SCORE_VALUE_1 = (int)GAME_TAGs.SCORE_VALUE_1,
    SCORE_VALUE_2 = (int)GAME_TAGs.SCORE_VALUE_2,
    CREATOR = (int)GAME_TAGs.CREATOR,
    COPY_DEATHRATTLE = (int)GAME_TAGs.COPY_DEATHRATTLE,
    CONTROLLER = (int)GAME_TAGs.CONTROLLER,
}

public static class DetachedEntityRegistry
{
    private static readonly ConcurrentDictionary<int, Entity> Data = new();
    public static void SetSnapshot(IEnumerable<Entity> entities)
    {
        Data.Clear();
        foreach (var entity in entities)
            Data[entity.Id] = entity;
    }

    public static Entity GetEntity(int entityId) => Data.TryGetValue(entityId, out var entity)
        ? entity
        : throw new InvalidOperationException(
            $"Entity {entityId} is not present in the current detached snapshot.");
}

/// <summary>Game-independent opening-hand request used by classic Mulligan.</summary>
public sealed class MulliganData
{
    public List<MulliganCard> Cards { get; init; } = new();
    public bool[] Mulligans { get; set; } = Array.Empty<bool>();
    public string UserClass { get; init; } = string.Empty;
    public string OpponentClass { get; init; } = string.Empty;
}

public sealed class MulliganCard
{
    public MulliganCardEntity Entity { get; init; } = new();
}

public sealed class MulliganCardEntity
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}
