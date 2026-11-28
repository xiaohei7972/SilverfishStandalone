using System.Globalization;
using System.Text;
using System.Text.Json;
using Silverfish.Integrations.Libmem;

const string usage = "用法  Silverfish.Integrations.Libmem list | inspect <PID> [--start-time <启动时间>] | inject <PID> --start-time <启动时间> --dll <Silverfish.GameProbe.dll> [--console-url <URL>] [--match-id <ID>]";
Console.OutputEncoding = Encoding.UTF8;
if (args is ["--help"] or ["-h"])
{
    Console.WriteLine(usage);
    return 0;
}

var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true };

try
{
    if (args is ["list"])
    {
        Console.WriteLine(JsonSerializer.Serialize(HearthstoneClientProbe.List(), jsonOptions));
        return 0;
    }

    if ((args is ["inspect", _] or ["inspect", _, "--start-time", _]) &&
        uint.TryParse(args[1], NumberStyles.None, CultureInfo.InvariantCulture, out var pid) && pid != 0)
    {
        ulong? startTime = null;
        if (args.Length == 4)
        {
            if (!ulong.TryParse(args[3], NumberStyles.None, CultureInfo.InvariantCulture, out var parsedStartTime) || parsedStartTime == 0)
            {
                Console.Error.WriteLine("启动时间必须是 list 返回的正整数。");
                return 2;
            }
            startTime = parsedStartTime;
        }

        Console.WriteLine(JsonSerializer.Serialize(HearthstoneClientProbe.Inspect(pid, startTime), jsonOptions));
        return 0;
    }

    if (args.Length >= 6 && args[0] == "inject" &&
        uint.TryParse(args[1], NumberStyles.None, CultureInfo.InvariantCulture, out var injectPid) && injectPid != 0 &&
        args[2] == "--start-time" &&
        ulong.TryParse(args[3], NumberStyles.None, CultureInfo.InvariantCulture, out var injectStartTime) && injectStartTime != 0 &&
        args[4] == "--dll" && !string.IsNullOrWhiteSpace(args[5]))
    {
        var dllPath = args[5];
        var consoleUrl = "http://127.0.0.1:5088";
        string? matchId = null;
        for (var i = 6; i < args.Length; i += 2)
        {
            if (i + 1 >= args.Length)
            {
                Console.Error.WriteLine(usage);
                return 2;
            }
            if (args[i] == "--console-url") consoleUrl = args[i + 1];
            else if (args[i] == "--match-id") matchId = args[i + 1];
            else
            {
                Console.Error.WriteLine(usage);
                return 2;
            }
        }
        Console.WriteLine(JsonSerializer.Serialize(
            HearthstoneMonoInjector.Inject(injectPid, injectStartTime, dllPath, consoleUrl, matchId), jsonOptions));
        return 0;
    }

    Console.Error.WriteLine(usage);
    return 2;
}
catch (Exception error) when (error is InvalidOperationException or DllNotFoundException or BadImageFormatException or FileNotFoundException or FileLoadException)
{
    Console.Error.WriteLine(error.Message);
    return 1;
}
