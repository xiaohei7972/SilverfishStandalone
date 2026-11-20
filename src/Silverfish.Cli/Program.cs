using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using Silverfish.Contracts;
using Silverfish.Core;
using Silverfish.Domain;
using Silverfish.Simulation;

var replay = (args.Length == 2 || (args.Length == 4 && args[2] == "--card-data")) &&
    args[0] is "simulate" or "plan";
var plan = replay && args[0] == "plan";
var import = args.Length is 3 or 4 && args[0] == "import-card-data" &&
    (args.Length == 3 || args[3] == "--starter");
if ((!replay && !import && args.Length != 1) || args[0] is "--help" or "-h")
{
    Console.Error.WriteLine("Usage: Silverfish.Cli <game-snapshot.json>");
    Console.Error.WriteLine("       Silverfish.Cli simulate <scenario.json> [--card-data <catalog.json>]");
    Console.Error.WriteLine("       Silverfish.Cli plan <scenario.json> [--card-data <catalog.json>]");
    Console.Error.WriteLine("       Silverfish.Cli import-card-data <CardDefs.xml> <catalog.json> [--starter]");
    return args.Length == 1 ? 0 : 2;
}

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    WriteIndented = true,
    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

try
{
    if (import)
    {
        ImportCardData(args[1], args[2], args.Length == 4);
        return 0;
    }

    await using var stream = File.OpenRead(replay ? args[1] : args[0]);
    if (replay)
    {
        var scenario = await JsonSerializer.DeserializeAsync<SimulationScenario>(stream, options)
            ?? throw new InvalidDataException("Input JSON must contain a simulation scenario.");
        if (scenario.Cards is null || scenario.Players is null || scenario.Entities is null || scenario.Actions is null || scenario.MinionDeaths is null)
            throw new InvalidDataException("Cards, players, entities, actions and minion deaths must be arrays.");
        CardDataCatalog? cardData = null;
        if (args.Length == 4)
        {
            if (scenario.CardSet != "starter")
                throw new InvalidDataException("--card-data requires a scenario with the starter card set.");
            using var cardStream = File.OpenRead(args[3]);
            cardData = CardDataCatalog.LoadJson(cardStream);
        }

        IEnumerable<SimulationCard> cards = scenario.CardSet switch
        {
            null => scenario.Cards,
            "starter" when cardData is not null => StarterCardCatalog.FromCardData(cardData).Concat(scenario.Cards),
            "starter" => StarterCardCatalog.Cards.Concat(scenario.Cards),
            _ => throw new InvalidDataException($"Unknown card set '{scenario.CardSet}'.")
        };
        var simulator = new GameSimulator(cards);
        var state = simulator.CreateGame(scenario.Players, scenario.Entities, scenario.ActivePlayer, scenario.Turn,
            scenario.MinionDeaths, scenario.RandomState);
        var steps = new List<SimulationStep>();
        foreach (var action in scenario.Actions)
        {
            var result = simulator.Apply(state, action);
            state = result.State;
            steps.Add(new SimulationStep(action, result.Events));
        }

        Console.WriteLine(JsonSerializer.Serialize(new
        {
            state.Turn, state.ActivePlayer, state.Outcome, state.Winner,
            state.MinionDeaths, state.RandomState,
            CardDataBuild = cardData?.SourceBuild,
            Players = state.Players.Values.OrderBy(player => player.Id.Value).ToArray(),
            Entities = state.Entities.Values.OrderBy(entity => entity.Id.Value).ToArray(),
            Steps = steps,
            LegalActions = simulator.LegalActions(state),
            Plan = plan ? new SimulationTurnPlanner(simulator, SimulationMaterialEvaluator.Evaluate).Plan(state) : null
        }, options));
        return 0;
    }

    var snapshot = await JsonSerializer.DeserializeAsync<GameSnapshot>(stream, options)
        ?? throw new InvalidDataException("Input JSON must contain a game snapshot object.");

    IStrategyEngine engine = new BaselineStrategyEngine();
    var decision = await engine.DecideAsync(snapshot);
    Console.WriteLine(JsonSerializer.Serialize(decision, options));
    return 0;
}
catch (Exception ex) when (ex is IOException or JsonException or XmlException or ArgumentException or
    InvalidDataException or InvalidOperationException or FormatException or OverflowException)
{
    var operation = import ? "import card data" : replay ? "simulate scenario" : "analyze snapshot";
    Console.Error.WriteLine($"Cannot {operation}: {ex.Message}");
    return 1;
}

static void ImportCardData(string input, string output, bool starterOnly)
{
    var outputPath = Path.GetFullPath(output);
    var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
    if (string.Equals(Path.GetFullPath(input), outputPath, comparison))
        throw new ArgumentException("The output catalog must use a different path from the input XML.");

    CardDataCatalog data;
    using (var source = File.OpenRead(input))
        data = CardDataCatalog.ImportXml(source);
    if (starterOnly)
    {
        var cards = StarterCardCatalog.FromCardData(data);
        _ = new GameSimulator(cards);
        data = data.SelectCards(cards.Select(card => card.Definition.CardId));
    }

    var directory = Path.GetDirectoryName(outputPath)!;
    Directory.CreateDirectory(directory);
    var temporaryPath = Path.Combine(directory, $"{Guid.NewGuid():N}.tmp");
    try
    {
        using (var destination = File.Open(temporaryPath, FileMode.CreateNew, FileAccess.Write))
            data.WriteJson(destination, indented: starterOnly);
        File.Move(temporaryPath, outputPath, overwrite: true);
    }
    finally
    {
        File.Delete(temporaryPath);
    }
    Console.WriteLine($"Exported {data.Cards.Length} cards from build {data.SourceBuild} to {outputPath}");
}

internal sealed record SimulationScenario
{
    public string? CardSet { get; init; }
    public SimulationCard[] Cards { get; init; } = [];
    public required PlayerState[] Players { get; init; }
    public required GameEntity[] Entities { get; init; }
    public required PlayerId ActivePlayer { get; init; }
    public int Turn { get; init; } = 1;
    public MinionDeath[] MinionDeaths { get; init; } = [];
    public ulong RandomState { get; init; }
    public required SimulationAction[] Actions { get; init; }
}

internal sealed record SimulationStep(SimulationAction Action, IReadOnlyList<SimulationEvent> Events);
