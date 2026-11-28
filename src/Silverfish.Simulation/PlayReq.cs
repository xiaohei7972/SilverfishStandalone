using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json.Serialization;
using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

[method: JsonConstructor]
public sealed record PlayReq(ErrorType? Type, int? Parameter = null)
{
    public PlayReq(ErrorType type, TAG_RACE race) : this(type, (int)race) { }
    public PlayReq(ErrorType type, GAME_TAG tag) : this(type, (int)tag) { }
    public static PlayReq Copied { get; } = new((ErrorType?)null);
    public string Name => Type?.ToString() ?? "CopiedPlayRequirements";
    public string? ParameterName => Type switch
    {
        ErrorType.REQ_TARGET_WITH_RACE when Parameter is int value => ((TAG_RACE)value).ToString(),
        ErrorType.REQ_TARGET_MUST_HAVE_TAG when Parameter is int value => value switch
        {
            2831 => "Treant", 3881 => "Wisp", _ => ((GAME_TAG)value).ToString()
        },
        _ => null
    };
    public int? Code => (int?)Type;
}

public sealed record PlayReqSet(ImmutableArray<PlayReq> Play, ImmutableArray<PlayReq> Ability);
public sealed record CardRequirementInfo(string CardId, PlayReqSet Requirements, bool EffectImplemented);

public static class PlayReqCatalog
{
    private const BindingFlags DeclaredMember = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static readonly Lazy<ImmutableDictionary<string, CardRequirementInfo>> Index = new(() =>
    {
        var entries = ImmutableDictionary.CreateBuilder<string, CardRequirementInfo>(StringComparer.Ordinal);
        foreach (var (id, rules) in StarterCardCatalog.RegisteredRules)
            entries.Add(id, new(id, rules.PlayReqs!, true));

        foreach (var type in typeof(PlayReqCatalog).Assembly.GetTypes())
        {
            if (!type.Name.StartsWith("Sim_", StringComparison.Ordinal))
                continue;
            var id = type.Name[4..];
            if (!entries.ContainsKey(id) && ReadDeclaredRequirements(type) is { } requirements)
                entries.Add(id, new(id, requirements, false));
        }
        return entries.ToImmutable();
    });

    internal static PlayReqSet? ReadDeclaredRequirements(Type provider) =>
        (PlayReqSet?)provider.GetProperty("PlayReqs", DeclaredMember)?.GetValue(null);

    public static IReadOnlyDictionary<string, CardRequirementInfo> Cards => Index.Value;
}

public sealed record PlayFailure(string Reason, ErrorType? Type = null, int? Parameter = null,
    int? Actual = null, EntityId? Source = null, EntityId? Target = null)
{
    public int? Code => (int?)Type;
    public string? Name => Type?.ToString();
}
