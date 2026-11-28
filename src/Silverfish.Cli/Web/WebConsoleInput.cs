using System.Text.Json;
using System.Text.Json.Serialization;
using Silverfish.Contracts;
using Silverfish.Core;
using Silverfish.Simulation;

namespace Silverfish.Cli.Web;

internal sealed class WebConsoleInput(CardDataCatalog? external = null)
{
    internal static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
    };

    private static readonly Lazy<CardDataCatalog> StarterNames = new(() =>
    {
        using var stream = Resource("Web/StarterCardData.json");
        return CardDataCatalog.LoadJson(stream);
    });
    private static readonly Lazy<CardDataCatalog> Demo = new(() =>
    {
        using var stream = Resource("Samples/card-data-demo.xml");
        return CardDataCatalog.ImportXml(stream);
    });
    private static readonly Lazy<IReadOnlyDictionary<string, string>> SampleKinds = new(FindSamples);
    internal static IReadOnlyDictionary<string, string> Samples => SampleKinds.Value;

    internal CardNames Names { get; } = new(StarterNames.Value, external);

    internal static Stream Resource(string name) => typeof(WebConsoleInput).Assembly.GetManifestResourceStream(name)
        ?? throw new InvalidOperationException("找不到控制台的内置资源。");

    internal async Task<ConsolePosition> LoadAsync(Stream stream, string? matchId, string? sample = null)
    {
        using var document = await JsonDocument.ParseAsync(stream);
        var root = document.RootElement;
        switch (Kind(root))
        {
            case "client":
                var idForClient = sample is not null ? "offline-standalone-sample" :
                    string.IsNullOrWhiteSpace(matchId) ? "imported-match" : matchId.Trim();
                var frame = StandaloneSnapshotAdapter.Read(root, idForClient);
                return Observable(frame.Snapshot, frame.AllEntities, frame.PendingChoice, frame.CanAct, frame.BlockReason, frame.ResponseMode);
            case "snapshot":
                if (new[] { "schemaVersion", "matchId", "sequence", "phase", "isOurTurn", "availableMana", "ourHero", "enemyHero" }
                    .Any(field => !root.TryGetProperty(field, out _)))
                    throw new InvalidDataException("可观察快照缺少版本、对局标识、阶段、法力或英雄字段。");
                return Observable(root.Deserialize<GameSnapshot>(Json)!);
            case "simulation":
                var scenario = root.Deserialize<SimulationScenario>(Json)!;
                if (scenario.Actions is { Length: > 256 } || scenario.Actions?.Any(action => action is null) == true)
                    throw new InvalidDataException("回放动作必须有效且不超过 256 步。");
                var catalog = sample == "simulation-card-data.json" ? Demo.Value :
                    scenario.CardSet is not null || scenario.CardRules is { Length: > 0 } ? external : null;
                var (simulator, game) = scenario.CreateGame(catalog);
                long sequence = 0;
                ConsoleEvent[] events = [];
                foreach (var action in scenario.Actions ?? [])
                {
                    var result = simulator.Apply(game, action);
                    game = result.State;
                    events = ConsolePosition.AppendEvents(events, ++sequence, result.Events);
                }
                var id = string.IsNullOrWhiteSpace(matchId) ? $"offline-{Guid.NewGuid():N}" : matchId.Trim();
                return ConsolePosition.Simulation(id, sequence, simulator, game, game.ActivePlayer, events, Names);
            default:
                throw new InvalidDataException("请选择可观察快照、客户端 state.snapshot 的 payload、模拟场景或模拟存档。");
        }
    }

    private ConsolePosition Observable(GameSnapshot snapshot, ConsoleEntity[]? allEntities = null,
        ConsolePendingChoice? pendingChoice = null, bool canAct = true, string? blockReason = null, string? responseMode = null)
    {
        SnapshotValidator.Validate(snapshot);
        return ConsolePosition.Observable(snapshot, Names, allEntities, pendingChoice, canAct, blockReason, responseMode);
    }

    private static string? Kind(JsonElement root) => root.ValueKind != JsonValueKind.Object ? null :
        root.TryGetProperty("protocolVersion", out _) && root.TryGetProperty("payloadJson", out _) ? "client" :
        root.TryGetProperty("protocolVersion", out _) ? "client" :
        root.TryGetProperty("players", out _) && root.TryGetProperty("entities", out _) ? "simulation" :
        root.TryGetProperty("ourHero", out _) ? "snapshot" : null;

    private static IReadOnlyDictionary<string, string> FindSamples()
    {
        var samples = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var resource in typeof(WebConsoleInput).Assembly.GetManifestResourceNames())
        {
            if (!resource.StartsWith("Samples/", StringComparison.Ordinal) || !resource.EndsWith(".json", StringComparison.Ordinal))
                continue;
            using var stream = Resource(resource);
            using var doc = JsonDocument.Parse(stream);
            if (Kind(doc.RootElement) is { } kind)
                samples.Add(resource["Samples/".Length..], kind);
        }
        return samples;
    }
}
