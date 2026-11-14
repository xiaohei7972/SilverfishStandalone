using HREngine.Bots;
using Silverfish.ClassicBridge;
using Silverfish.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;

// Offline-only: load original Silverfish CardDB and optionally run the actual
// classic AI on a text or restricted JSON fixture. Never execute game actions.
Settings.Instance.setFilePath(AppContext.BaseDirectory);
Settings.Instance.setLoggPath(Path.Combine(Path.GetTempPath(), "silverfish-standalone") + Path.DirectorySeparatorChar);
Directory.CreateDirectory(Settings.Instance.logpath);
Settings.Instance.setLoggFile("classic-smoke.log");

var unknown = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.None);
Console.WriteLine($"Classic CardDB loaded: {unknown is not null}");

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    WriteIndented = false,
    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
};

if (args.Length == 1)
{
    GameSnapshot? snapshot = null;
    string board;
    if (string.Equals(Path.GetExtension(args[0]), ".json", StringComparison.OrdinalIgnoreCase))
    {
        snapshot = JsonSerializer.Deserialize<GameSnapshot>(
            await File.ReadAllTextAsync(args[0]), options)
            ?? throw new InvalidDataException("Snapshot JSON is empty.");
        board = ClassicSnapshotFormatter.Format(snapshot);
    }
    else
    {
        board = await File.ReadAllTextAsync(args[0]);
    }

    // Previously initialized by Hearthbuddy's Routine; independent host sets
    // its own compatible classic search strategy explicitly.
    Ai.Instance.botBase = new Behavior丨通用丨不设惩罚();
    var durations = Ai.Instance.autoTester(printstuff: false, data: board, mode: 2);
    Console.WriteLine($"Classic AI ran {durations.Count} calculation pass(es).");
    var best = Ai.Instance.bestmove;
    if (durations.Count == 0 || best is null)
    {
        Console.Error.WriteLine("Classic AI did not produce a concrete bestmove.");
        return 1;
    }

    // Use the original action's fields instead of Action.ToString()'s type name.
    var cardField = typeof(HREngine.Bots.Action).GetField("hc")
                    ?? typeof(HREngine.Bots.Action).GetField("card");
    var handCard = cardField?.GetValue(best) as Handmanager.Handcard;
    Console.WriteLine("CLASSIC_BESTMOVE_JSON=" + JsonSerializer.Serialize(new
    {
        actionType = best.actionType.ToString(),
        sourceEntityId = best.own?.entitiyID ?? handCard?.entity,
        targetEntityId = best.target?.entitiyID,
        placement = best.place,
        choice = best.druidchoice
    }, options));

    if (snapshot is not null)
    {
        var decision = ClassicDecisionAdapter.Match(snapshot, best, Ai.Instance.bestmoveValue);
        Console.WriteLine("CLASSIC_DECISION_JSON=" + JsonSerializer.Serialize(decision, options));
        if (decision.ChosenAction is null)
        {
            Console.Error.WriteLine("Classic bestmove could not match one trusted legal action.");
            return 1;
        }
    }
}
else if (args.Length != 0)
{
    Console.Error.WriteLine("Usage: Silverfish.Classic.Smoke [board-test.txt|snapshot.json]");
    return 2;
}

return 0;
