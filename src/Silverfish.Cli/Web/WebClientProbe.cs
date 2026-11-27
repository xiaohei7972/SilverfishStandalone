using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Silverfish.Cli.Web;

internal sealed record ClientInstance(uint Pid,
    [property: JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)] ulong StartTime,
    string ExecutablePath, string Architecture, ulong Bits);
internal sealed record ClientInspection(ClientInstance Client, string MainModuleSignature, string[] Modules);
internal sealed record ClientList(string? Unavailable, ClientInstance[] Clients);
internal sealed record ClientBinding(ClientInspection Inspection, string Status, bool Injected = false);

internal interface IClientProbe
{
    string? Unavailable { get; }
    Task<ClientInstance[]> ListAsync(CancellationToken token);
    Task<ClientInspection> InspectAsync(uint pid, ulong startTime, CancellationToken token);
    Task InjectAsync(uint pid, ulong startTime, string consoleUrl, CancellationToken token);
}

internal sealed class ProcessClientProbe(string? explicitPath) : IClientProbe
{
    private const string Exe = "Silverfish.Integrations.Libmem.exe";
    private readonly string? _path = OperatingSystem.IsWindows() ? explicitPath ?? FindHelper() : null;
    public string? Unavailable => !OperatingSystem.IsWindows() ? "客户端探测仅支持 Windows；离线功能仍可使用。" :
        _path is null || !File.Exists(_path) ? "找不到探测程序。请构建 Windows x64 探测项目，或启动时用 --probe 指定程序。" : null;

    public Task<ClientInstance[]> ListAsync(CancellationToken token) => QueryAsync<ClientInstance[]>(token, "list");

    public Task<ClientInspection> InspectAsync(uint pid, ulong startTime, CancellationToken token) =>
        QueryAsync<ClientInspection>(token, "inspect", pid.ToString(CultureInfo.InvariantCulture), "--start-time",
            startTime.ToString(CultureInfo.InvariantCulture));

    public async Task InjectAsync(uint pid, ulong startTime, string consoleUrl, CancellationToken token)
    {
        var probeDll = FindProbeDll() ?? throw new InvalidOperationException("找不到 Silverfish.GameProbe.dll，请先编译解决方案。");
        await QueryAsync<JsonElement>(token, "inject", pid.ToString(CultureInfo.InvariantCulture), "--start-time",
            startTime.ToString(CultureInfo.InvariantCulture), "--dll", probeDll, "--console-url", consoleUrl, "--match-id", $"live-{pid}");
    }

    private async Task<TResult> QueryAsync<TResult>(CancellationToken token, params string[] args)
    {
        if (Unavailable is { } reason)
            throw new InvalidOperationException(reason);
        var info = new ProcessStartInfo(_path!)
        {
            WorkingDirectory = Path.GetDirectoryName(_path)!, UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardOutput = true, RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8, StandardErrorEncoding = Encoding.UTF8
        };
        foreach (var argument in args)
            info.ArgumentList.Add(argument);
        try
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(token);
            timeout.CancelAfter(TimeSpan.FromSeconds(5));
            using var process = Process.Start(info) ?? throw new InvalidOperationException("无法启动探测程序。");
            try
            {
                var output = process.StandardOutput.ReadToEndAsync(timeout.Token);
                var errors = process.StandardError.ReadToEndAsync(timeout.Token);
                await Task.WhenAll(output, errors, process.WaitForExitAsync(timeout.Token));
                if (process.ExitCode != 0)
                {
                    var message = (await errors).Trim();
                    throw new InvalidOperationException(message.Length is > 0 and <= 300 ? message : "客户端探测失败。");
                }
                return JsonSerializer.Deserialize<TResult>(await output, WebConsoleInput.Json)
                    ?? throw new InvalidOperationException("探测程序没有返回有效结果。");
            }
            catch (OperationCanceledException)
            {
                try { process.Kill(entireProcessTree: true); }
                catch (Exception error) when (error is InvalidOperationException or Win32Exception) { }
                token.ThrowIfCancellationRequested();
                throw new InvalidOperationException("客户端探测超时。");
            }
        }
        catch (Exception error) when (error is Win32Exception or JsonException)
        {
            throw new InvalidOperationException("无法运行探测程序或读取它返回的结果。", error);
        }
    }

    private static string? FindHelper()
    {
        var adjacent = Path.Combine(AppContext.BaseDirectory, Exe);
        if (File.Exists(adjacent))
            return adjacent;
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (!File.Exists(Path.Combine(directory.FullName, "SilverfishStandalone.sln")))
                continue;
            return new[] { "Release", "Debug" }.Select(config => Path.Combine(directory.FullName, "src",
                "Silverfish.Integrations.Libmem", "bin", config, "net8.0-windows", "win-x64", Exe)).FirstOrDefault(File.Exists);
        }
        return null;
    }

    private static string? FindProbeDll()
    {
        const string dll = "Silverfish.GameProbe.Live4.dll";
        var adjacent = Path.Combine(AppContext.BaseDirectory, dll);
        if (File.Exists(adjacent))
            return adjacent;
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (!File.Exists(Path.Combine(directory.FullName, "SilverfishStandalone.sln")))
                continue;
            return new[] { "Release", "Debug" }.Select(config => Path.Combine(directory.FullName, "src",
                "Silverfish.GameProbe", "bin", config, "net472", dll)).FirstOrDefault(File.Exists);
        }
        return null;
    }
}

internal sealed class WebClientConnection(IClientProbe probe)
{
    private readonly object _gate = new();
    private readonly SemaphoreSlim _queries = new(1);
    private ClientBinding? _binding;
    private long _revision;

    internal ClientBinding? Read() { lock (_gate) return _binding; }

    internal async Task<ClientList> ListAsync(CancellationToken token)
    {
        if (probe.Unavailable is { } unavailable)
            return new(unavailable, []);
        await _queries.WaitAsync(token);
        try { return new(null, await probe.ListAsync(token)); }
        finally { _queries.Release(); }
    }

    internal async Task<ClientBinding> BindAsync(uint pid, ulong startTime, CancellationToken token)
    {
        if (pid == 0 || startTime == 0)
            throw new ArgumentException("必须提供从实例列表中取得的 PID 和启动标识。");
        long revision;
        lock (_gate) revision = ++_revision;
        await _queries.WaitAsync(token);
        ClientInspection inspection;
        try { inspection = await probe.InspectAsync(pid, startTime, token); }
        finally { _queries.Release(); }
        if (inspection.Client.Pid != pid || inspection.Client.StartTime != startTime ||
            inspection.MainModuleSignature != "MZ" || inspection.Modules is not { Length: > 0 })
            throw new InvalidOperationException("探测结果的实例标识、映像签名或模块列表不匹配。");
        lock (_gate)
        {
            if (revision != _revision)
                throw new ConsoleConflictException("绑定已取消或被另一次操作替换。");
            return _binding = new(inspection, "online");
        }
    }

    internal void Disconnect()
    {
        lock (_gate)
        {
            _revision++;
            _binding = null;
        }
    }

    internal async Task<ClientBinding> InjectAsync(string consoleUrl, CancellationToken token)
    {
        ClientBinding current;
        lock (_gate)
            current = _binding ?? throw new InvalidOperationException("请先在客户端列表中选择并绑定一个炉石实例。");
        await _queries.WaitAsync(token);
        try
        {
            await probe.InjectAsync(current.Inspection.Client.Pid, current.Inspection.Client.StartTime, consoleUrl, token);
        }
        finally { _queries.Release(); }
        lock (_gate)
        {
            if (ReferenceEquals(_binding, current))
                _binding = current with { Injected = true, Status = "online" };
            return _binding ?? current with { Injected = true };
        }
    }

    internal async Task RefreshAsync(CancellationToken token)
    {
        ClientBinding? current;
        lock (_gate) current = _binding;
        if (current is null || current.Status == "offline")
            return;
        string status;
        try
        {
            var list = await ListAsync(token);
            status = list.Unavailable is not null ? "unknown" : list.Clients.Any(client =>
                client.Pid == current.Inspection.Client.Pid && client.StartTime == current.Inspection.Client.StartTime)
                ? "online" : "offline";
        }
        catch (Exception error) when (error is InvalidOperationException or IOException) { status = "unknown"; }
        lock (_gate)
            if (ReferenceEquals(_binding, current) && current.Status != status)
                _binding = current with { Status = status };
    }
}
