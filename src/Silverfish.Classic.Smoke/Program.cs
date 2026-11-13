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
    Console.WriteLine($"Best move: {Ai.Instance.bestmove?.ToString() ?? "none"}");
    if (durations.Count == 0)
        return 1;
}
else if (args.Length != 0)
{
    Console.Error.WriteLine("Usage: Silverfish.Classic.Smoke [board-test.txt]");
    return 2;
}

return 0;
