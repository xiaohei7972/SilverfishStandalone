using System.Text.Json;
using System.Text.Json.Serialization;
using Silverfish.Contracts;
using Silverfish.Core;
using Silverfish.Domain;
using Silverfish.Simulation;

var replay = args.Length == 2 && args[0] is "simulate" or "plan";
var plan = replay && args[0] == "plan";
if ((!replay && args.Length != 1) || args[0] is "--help" or "-h")
{
    Console.Error.WriteLine("Usage: Silverfish.Cli <game-snapshot.json>");
    Console.Error.WriteLine("       Silverfish.Cli simulate <scenario.json>");
    Console.Error.WriteLine("       Silverfish.Cli plan <scenario.json>");
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
    await using var stream = File.OpenRead(args[^1]);
    if (replay)
    {
        var scenario = await JsonSerializer.DeserializeAsync<SimulationScenario>(stream, options)
            ?? throw new InvalidDataException("Input JSON must contain a simulation scenario.");
        if (scenario.Cards is null || scenario.Players is null || scenario.Entities is null || scenario.Actions is null)
            throw new InvalidDataException("Cards, players, entities and actions must be arrays.");
        var simulator = new GameSimulator(scenario.Cards);
        var state = simulator.CreateGame(scenario.Players, scenario.Entities, scenario.ActivePlayer, scenario.Turn);
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
catch (Exception ex) when (ex is IOException or JsonException or ArgumentException or InvalidDataException or InvalidOperationException or OverflowException)
{
    Console.Error.WriteLine(replay ? $"Cannot simulate scenario: {ex.Message}" : $"Cannot analyze snapshot: {ex.Message}");
    return 1;
}

internal sealed record SimulationScenario
{
    public required SimulationCard[] Cards { get; init; }
    public required PlayerState[] Players { get; init; }
    public required GameEntity[] Entities { get; init; }
    public required PlayerId ActivePlayer { get; init; }
    public int Turn { get; init; } = 1;
    public required SimulationAction[] Actions { get; init; }
}

internal sealed record SimulationStep(SimulationAction Action, IReadOnlyList<SimulationEvent> Events);
