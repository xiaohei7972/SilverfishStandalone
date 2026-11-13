using Libmem.NET;

namespace Silverfish.Integrations.Libmem;

public sealed record ProcessProbe(
    uint Pid,
    string Name,
    string Architecture,
    ulong Bits,
    IReadOnlyList<string> Modules);

/// <summary>
/// Optional Windows-only process metadata probe. It does not read gameplay
/// state, inject into the target process, or execute commands.
/// </summary>
public static class LibmemProcessProbe
{
    public static ProcessProbe Inspect(uint pid)
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("Libmem.NET probe requires Windows.");

        using var session = ProcessSession.Open(pid)
            ?? throw new InvalidOperationException($"Cannot open process {pid}.");

        var modules = session.Modules.Enumerate()
            .Select(module => module.Name)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new ProcessProbe(
            session.Pid,
            session.Name,
            session.Architecture.ToString(),
            session.Bits,
            modules);
    }
