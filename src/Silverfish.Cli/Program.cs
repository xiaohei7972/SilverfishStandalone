using System.Text.Json;
using System.Text.Json.Serialization;
using Silverfish.Contracts;
using Silverfish.Core;

if (args.Length != 1 || args[0] is "--help" or "-h")
{
    Console.Error.WriteLine("Usage: Silverfish.Cli <game-snapshot.json>");
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
    await using var stream = File.OpenRead(args[0]);
    var snapshot = await JsonSerializer.DeserializeAsync<GameSnapshot>(stream, options)
        ?? throw new InvalidDataException("Input JSON must contain a game snapshot object.");

    IStrategyEngine engine = new BaselineStrategyEngine();
    var decision = await engine.DecideAsync(snapshot);
    Console.WriteLine(JsonSerializer.Serialize(decision, options));
    return 0;
}
catch (Exception ex) when (ex is IOException or JsonException or ArgumentException or InvalidDataException)
{
    Console.Error.WriteLine($"Cannot analyze snapshot: {ex.Message}");
    return 1;
}
