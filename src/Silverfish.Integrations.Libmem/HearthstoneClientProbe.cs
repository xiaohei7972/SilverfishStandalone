using Libmem.NET;
using NativeApi = global::Libmem.NET.Libmem;

namespace Silverfish.Integrations.Libmem;

public sealed record HearthstoneClient(
    uint Pid,
    ulong StartTime,
    string ExecutablePath,
    string Architecture,
    ulong Bits);

public sealed record HearthstoneClientInspection(
    HearthstoneClient Client,
    string MainModuleSignature,
    IReadOnlyList<string> Modules);

public static class HearthstoneClientProbe
{
    private const string ExecutableName = "Hearthstone.exe";

    public static IReadOnlyList<HearthstoneClient> List() =>
        NativeApi.EnumProcesses()
            .Where(IsHearthstone)
            .OrderBy(process => process.Pid)
            .Select(Describe)
            .ToArray();

    public static HearthstoneClientInspection Inspect(uint pid, ulong? startTime = null)
    {
        var process = NativeApi.GetProcess(pid);
        if (process is null || !IsHearthstone(process))
            throw new InvalidOperationException($"PID {pid} 不是正在运行的 Hearthstone.exe。");
        if (startTime is ulong expected && (expected == 0 || process.StartTime != expected))
            throw new InvalidOperationException($"PID {pid} 的启动时间与发现的实例不一致。");
        if (process.Architecture != Architecture.X64 || process.Bits != 64)
            throw new InvalidOperationException($"PID {pid} 不是 x64 炉石客户端。");

        using var session = ProcessSession.Open(process)
            ?? throw new InvalidOperationException($"无法打开炉石客户端 PID {pid}。");
        var modules = session.Modules.Enumerate();
        var executable = modules.FirstOrDefault(module =>
            string.Equals(module.Path, process.Path, StringComparison.OrdinalIgnoreCase));
        if (executable is null)
            throw new InvalidOperationException($"PID {pid} 的模块列表中缺少主程序映像。");
        if (executable.Size < 2 || !session.Memory.TryRead(executable.Base, 2, out var signature))
            throw new InvalidOperationException($"无法读取 PID {pid} 的主程序映像签名。");
        if (!signature.AsSpan().SequenceEqual("MZ"u8))
            throw new InvalidOperationException($"PID {pid} 的主程序映像签名不匹配。");
        if (!session.IsAlive())
            throw new InvalidOperationException($"炉石客户端 PID {pid} 已退出或发生变化。");

        return new(Describe(process), "MZ", modules.Select(module => module.Name)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToArray());
    }

    private static bool IsHearthstone(ProcessInfo process) =>
        string.Equals(Path.GetFileName(process.Path), ExecutableName, StringComparison.OrdinalIgnoreCase);

    private static HearthstoneClient Describe(ProcessInfo process) =>
        new(process.Pid, process.StartTime, process.Path, process.Architecture.ToString(), process.Bits);
}
