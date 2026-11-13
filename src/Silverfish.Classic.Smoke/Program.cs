using HREngine.Bots;

// A headless smoke entry point for the classic CardDB and optional offline
// AI board-test fixture. It does not execute any actions in a live game.
Settings.Instance.setFilePath(AppContext.BaseDirectory);
Settings.Instance.setLoggPath(Path.Combine(Path.GetTempPath(), "silverfish-standalone") + Path.DirectorySeparatorChar);
Directory.CreateDirectory(Settings.Instance.logpath);
Settings.Instance.setLoggFile("classic-smoke.log");

var unknown = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.None);
Console.WriteLine($"Classic CardDB loaded: {unknown is not null}");

if (args.Length == 1)
{
    var board = await File.ReadAllTextAsync(args[0]);
    // In old Hearthbuddy, the host supplied a strategy before invoking Ai.
    // An independent host must select one explicitly for its offline searches.
    Ai.Instance.botBase = new Behavior丨通用丨不设惩罚();
    var durations = Ai.Instance.autoTester(printstuff: false, data: board, mode: 2);
    Console.WriteLine($"Classic AI ran {durations.Count} calculation pass(es).");
    var best = Ai.Instance.bestmove;
    if (durations.Count == 0 || best is null)
    {
        Console.Error.WriteLine("Classic AI did not produce a concrete bestmove.");
        return 1;
    }

    // Log the original search result, not the default Action.ToString()
    // class name. This is diagnostic metadata, never an execution request.
    var cardField = typeof(HREngine.Bots.Action).GetField("hc")
                    ?? typeof(HREngine.Bots.Action).GetField("card");
    var handCard = cardField?.GetValue(best) as Handmanager.Handcard;
    Console.WriteLine("CLASSIC_BESTMOVE_JSON=" + System.Text.Json.JsonSerializer.Serialize(new
    {
        actionType = best.actionType.ToString(),
        sourceEntityId = best.own?.entitiyID ?? handCard?.entity,
        targetEntityId = best.target?.entitiyID,
        placement = best.place,
        choice = best.druidchoice
    }));
}
else if (args.Length != 0)
{
    Console.Error.WriteLine("Usage: Silverfish.Classic.Smoke [board-test.txt]");
    return 2;
}

return 0;
