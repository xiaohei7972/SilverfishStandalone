using System.Runtime.InteropServices;
using System.Text;
using Libmem.NET;

namespace Silverfish.Integrations.Libmem;

public sealed record HearthstoneInjectionResult(
    HearthstoneClient Client,
    string ProbeDllPath,
    string ConsoleUrl,
    string MatchId,
    string Status);

public static class HearthstoneMonoInjector
{
    private const uint ProcessCreateThread = 0x0002;
    private const uint ProcessQueryInformation = 0x0400;
    private const uint ProcessVmOperation = 0x0008;
    private const uint ProcessVmWrite = 0x0020;
    private const uint ProcessVmRead = 0x0010;
    private const uint MemCommit = 0x1000;
    private const uint MemReserve = 0x2000;
    private const uint MemRelease = 0x8000;
    private const uint PageReadWrite = 0x04;
    private const uint PageExecuteRead = 0x20;
    private const uint WaitObject0 = 0x00000000;

    public static HearthstoneInjectionResult Inject(uint pid, ulong startTime, string probeDllPath, string consoleUrl, string? matchId = null)
    {
        if (pid == 0 || startTime == 0)
            throw new InvalidOperationException("必须提供有效的 PID 和启动时间。");
        if (string.IsNullOrWhiteSpace(probeDllPath) || !File.Exists(probeDllPath))
            throw new FileNotFoundException("找不到注入探测 DLL：Silverfish.GameProbe.dll。", probeDllPath);
        if (string.IsNullOrWhiteSpace(consoleUrl))
            throw new InvalidOperationException("必须提供控制台地址。");

        var resolvedMatchId = string.IsNullOrWhiteSpace(matchId) ? $"live-{pid}" : matchId!.Trim();
        var inspection = HearthstoneClientProbe.Inspect(pid, startTime);

        // Stage DLL into an ASCII temp folder with a unique timestamp so hot re-injection bypasses Mono assembly cache.
        var stageDir = Path.Combine(Path.GetTempPath(), "SilverfishProbe");
        Directory.CreateDirectory(stageDir);
        var stamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var stagedDllPath = Path.Combine(stageDir, $"Silverfish.GameProbe.{pid}.{stamp}.dll");
        File.Copy(Path.GetFullPath(probeDllPath), stagedDllPath, overwrite: true);

        var cfgPath = Path.Combine(stageDir, "Silverfish.GameProbe.cfg");
        File.WriteAllLines(cfgPath, [consoleUrl.TrimEnd('/'), resolvedMatchId], Encoding.UTF8);

        using var session = ProcessSession.Open(pid)
            ?? throw new InvalidOperationException($"无法打开炉石客户端 PID {pid}。");
        if (session.Info.StartTime != startTime)
            throw new InvalidOperationException($"PID {pid} 的启动时间已变化，拒绝注入。");

        var mono = session.Modules.Find("mono-2.0-bdwgc.dll")
            ?? session.Modules.Find("mono-2.0-sgen.dll")
            ?? session.Modules.Find("mono.dll")
            ?? throw new InvalidOperationException($"炉石客户端 PID {pid} 尚未加载 Mono 运行时模块。");

        var fnGetRootDomain = RequireSymbol(session, mono, "mono_get_root_domain");
        var fnThreadAttach = RequireSymbol(session, mono, "mono_thread_attach");
        var fnThreadDetach = RequireSymbol(session, mono, "mono_thread_detach");
        var fnDomainAssemblyOpen = RequireSymbol(session, mono, "mono_domain_assembly_open");
        var fnAssemblyGetImage = RequireSymbol(session, mono, "mono_assembly_get_image");
        var fnClassFromName = RequireSymbol(session, mono, "mono_class_from_name");
        var fnClassGetMethod = RequireSymbol(session, mono, "mono_class_get_method_from_name");
        var fnRuntimeInvoke = RequireSymbol(session, mono, "mono_runtime_invoke");

        var hProcess = OpenProcess(ProcessCreateThread | ProcessQueryInformation | ProcessVmOperation | ProcessVmWrite | ProcessVmRead, false, pid);
        if (hProcess == 0)
            throw new InvalidOperationException($"无法打开目标进程句柄（Win32 错误 {Marshal.GetLastWin32Error()}）。");

        nint remoteData = 0;
        nint remoteCode = 0;
        nint hThread = 0;
        try
        {
            remoteData = VirtualAllocEx(hProcess, 0, 4096, MemCommit | MemReserve, PageReadWrite);
            remoteCode = VirtualAllocEx(hProcess, 0, 4096, MemCommit | MemReserve, PageExecuteRead);
            if (remoteData == 0 || remoteCode == 0)
                throw new InvalidOperationException("无法在目标进程中分配注入引导内存。");

            var dataBlock = BuildDataBlock(stagedDllPath, "Silverfish.GameProbe", "ProbeEntry", "Initialize", (ulong)remoteData,
                out var pathAddr, out var nsAddr, out var clsAddr, out var mthAddr, out var excAddr, out var statusAddr);
            if (!WriteProcessMemory(hProcess, remoteData, dataBlock, (nuint)dataBlock.Length, out _))
                throw new InvalidOperationException("写入注入参数到目标进程失败。");

            var shellcode = BuildX64MonoBootstrapShellcode(
                fnGetRootDomain, fnThreadAttach, fnThreadDetach, fnDomainAssemblyOpen, fnAssemblyGetImage,
                fnClassFromName, fnClassGetMethod, fnRuntimeInvoke,
                pathAddr, nsAddr, clsAddr, mthAddr, excAddr, statusAddr);

            if (!VirtualProtectEx(hProcess, remoteCode, (nuint)shellcode.Length, PageReadWrite, out _))
                throw new InvalidOperationException("修改远程代码页属性失败。");
            if (!WriteProcessMemory(hProcess, remoteCode, shellcode, (nuint)shellcode.Length, out _))
                throw new InvalidOperationException("写入注入引导代码失败。");
            VirtualProtectEx(hProcess, remoteCode, (nuint)shellcode.Length, PageExecuteRead, out _);

            hThread = CreateRemoteThread(hProcess, 0, 0, remoteCode, 0, 0, out _);
            if (hThread == 0)
                throw new InvalidOperationException($"创建远程引导线程失败（Win32 错误 {Marshal.GetLastWin32Error()}）。");

            var wait = WaitForSingleObject(hThread, 8000);
            if (wait != WaitObject0)
                throw new InvalidOperationException("等待远程 Mono 注入完成超时。");

            GetExitCodeThread(hThread, out var exitCode);
            if (exitCode != 0)
                throw new InvalidOperationException($"远程 Mono 注入返回阶段错误码 {exitCode}。");

            return new HearthstoneInjectionResult(inspection.Client, stagedDllPath, consoleUrl, resolvedMatchId, "injected");
        }
        finally
        {
            if (hThread != 0) CloseHandle(hThread);
            if (remoteCode != 0) VirtualFreeEx(hProcess, remoteCode, 0, MemRelease);
            if (remoteData != 0) VirtualFreeEx(hProcess, remoteData, 0, MemRelease);
            CloseHandle(hProcess);
        }
    }

    private static ulong RequireSymbol(ProcessSession session, ModuleInfo module, string name) =>
        session.Symbols.TryFindAddress(module, name, demangle: false, out var address) && address != 0
            ? address
            : throw new InvalidOperationException($"在 {module.Name} 中找不到导出符号 {name}。");

    public static byte[] BuildDataBlock(
        string dllPath, string nameSpace, string className, string methodName, ulong baseAddress,
        out ulong pathAddr, out ulong nsAddr, out ulong clsAddr, out ulong mthAddr, out ulong excAddr, out ulong statusAddr)
    {
        var buffer = new byte[2048];
        excAddr = baseAddress;
        statusAddr = baseAddress + 8;
        var offset = 32;

        pathAddr = baseAddress + (ulong)offset;
        offset += WriteNullTerminatedUtf8(buffer, offset, dllPath);

        nsAddr = baseAddress + (ulong)offset;
        offset += WriteNullTerminatedUtf8(buffer, offset, nameSpace);

        clsAddr = baseAddress + (ulong)offset;
        offset += WriteNullTerminatedUtf8(buffer, offset, className);

        mthAddr = baseAddress + (ulong)offset;
        WriteNullTerminatedUtf8(buffer, offset, methodName);
        return buffer;
    }

    private static int WriteNullTerminatedUtf8(byte[] dest, int offset, string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        Buffer.BlockCopy(bytes, 0, dest, offset, bytes.Length);
        dest[offset + bytes.Length] = 0;
        return ((bytes.Length + 8) / 8) * 8;
    }

    public static byte[] BuildX64MonoBootstrapShellcode(
        ulong fnGetRootDomain, ulong fnThreadAttach, ulong fnThreadDetach,
        ulong fnDomainAssemblyOpen, ulong fnAssemblyGetImage,
        ulong fnClassFromName, ulong fnClassGetMethod, ulong fnRuntimeInvoke,
        ulong pathAddr, ulong nsAddr, ulong clsAddr, ulong mthAddr, ulong excAddr, ulong statusAddr)
    {
        var code = new List<byte>(384);
        var failPatches = new List<int>();

        // Prologue: push rbx; push rsi; push rdi; sub rsp, 0x30
        // Stack alignment: entry RSP = 16n+8; 3 pushes = 24B -> 16n; sub 0x30 (48B) -> 16n (32B shadow + 16B)
        // rbx = domain, rsi = monoThread (0 until attached), rdi = exitCode
        code.AddRange([0x53, 0x56, 0x57, 0x48, 0x83, 0xEC, 0x30]);
        code.AddRange([0x31, 0xF6]); // xor esi, esi (monoThread = null)
        code.AddRange([0x31, 0xFF]); // xor edi, edi (exitCode = 0)

        // 1. domain = mono_get_root_domain()
        EmitMovRax(code, fnGetRootDomain);
        EmitCallRax(code);
        EmitNullCheckAndFail(code, 1, failPatches);
        code.AddRange([0x48, 0x89, 0xC3]); // mov rbx, rax

        // 2. monoThread = mono_thread_attach(domain)
        code.AddRange([0x48, 0x89, 0xD9]); // mov rcx, rbx
        EmitMovRax(code, fnThreadAttach);
        EmitCallRax(code);
        EmitNullCheckAndFail(code, 2, failPatches);
        code.AddRange([0x48, 0x89, 0xC6]); // mov rsi, rax (save monoThread for detach)

        // 3. assembly = mono_domain_assembly_open(domain, pathAddr)
        code.AddRange([0x48, 0x89, 0xD9]); // mov rcx, rbx
        EmitMovRdx(code, pathAddr);
        EmitMovRax(code, fnDomainAssemblyOpen);
        EmitCallRax(code);
        EmitNullCheckAndFail(code, 3, failPatches);

        // 4. image = mono_assembly_get_image(assembly)
        code.AddRange([0x48, 0x89, 0xC1]); // mov rcx, rax
        EmitMovRax(code, fnAssemblyGetImage);
        EmitCallRax(code);
        EmitNullCheckAndFail(code, 4, failPatches);

        // 5. klass = mono_class_from_name(image, nsAddr, clsAddr)
        code.AddRange([0x48, 0x89, 0xC1]); // mov rcx, rax
        EmitMovRdx(code, nsAddr);
        EmitMovR8(code, clsAddr);
        EmitMovRax(code, fnClassFromName);
        EmitCallRax(code);
        EmitNullCheckAndFail(code, 5, failPatches);

        // 6. method = mono_class_get_method_from_name(klass, mthAddr, 0)
        code.AddRange([0x48, 0x89, 0xC1]); // mov rcx, rax
        EmitMovRdx(code, mthAddr);
        code.AddRange([0x45, 0x31, 0xC0]); // xor r8d, r8d
        EmitMovRax(code, fnClassGetMethod);
        EmitCallRax(code);
        EmitNullCheckAndFail(code, 6, failPatches);

        // 7. mono_runtime_invoke(method, null, null, excAddr)
        code.AddRange([0x48, 0x89, 0xC1]); // mov rcx, rax
        code.AddRange([0x31, 0xD2]);       // xor edx, edx
        code.AddRange([0x45, 0x31, 0xC0]); // xor r8d, r8d
        EmitMovR9(code, excAddr);
        EmitMovRax(code, fnRuntimeInvoke);
        EmitCallRax(code);

        // Check *excAddr == 0; if non-zero set edi = 7
        EmitMovRax(code, excAddr);
        code.AddRange([0x48, 0x8B, 0x00]); // mov rax, [rax]
        code.AddRange([0x48, 0x85, 0xC0]); // test rax, rax
        code.AddRange([0x74, 0x05]);       // jz +5 (skip mov edi, 7 which is 5 bytes)
        code.AddRange([0xBF, 0x07, 0x00, 0x00, 0x00]); // mov edi, 7

        // Exit label: detach Mono thread if attached (MUST detach before OS thread exits to avoid GC deadlock!)
        var exitOffset = code.Count;
        code.AddRange([0x48, 0x85, 0xF6]); // test rsi, rsi
        // jz skip_detach: skip mov rcx, rsi (3) + mov rax, imm64 (10) + call rax (2) = 15 bytes (0x0F)
        code.AddRange([0x74, 0x0F]);
        code.AddRange([0x48, 0x89, 0xF1]); // mov rcx, rsi
        EmitMovRax(code, fnThreadDetach);
        EmitCallRax(code);

        // Store status and return exitCode in eax
        code.AddRange([0x89, 0xF8]);       // mov eax, edi
        EmitMovRcx(code, statusAddr);
        code.AddRange([0x89, 0x01]);       // mov [rcx], eax
        // Epilogue: add rsp, 0x30; pop rdi; pop rsi; pop rbx; ret
        code.AddRange([0x48, 0x83, 0xC4, 0x30, 0x5F, 0x5E, 0x5B, 0xC3]);

        foreach (var patchPos in failPatches)
        {
            var rel = exitOffset - (patchPos + 4);
            var relBytes = BitConverter.GetBytes(rel);
            for (var i = 0; i < 4; i++)
                code[patchPos + i] = relBytes[i];
        }

        return [.. code];
    }

    private static void EmitNullCheckAndFail(List<byte> code, uint errorCode, List<int> failPatches)
    {
        // test rax, rax (3 bytes)
        code.AddRange([0x48, 0x85, 0xC0]);
        // jnz +10 (2 bytes): skips mov edi, imm32 (5 bytes) + jmp rel32 (5 bytes) = 10 bytes (0x0A)
        code.AddRange([0x75, 0x0A]);
        // mov edi, errorCode (5 bytes)
        code.Add(0xBF);
        code.AddRange(BitConverter.GetBytes(errorCode));
        // jmp rel32 exitOffset (5 bytes)
        code.Add(0xE9);
        failPatches.Add(code.Count);
        code.AddRange([0, 0, 0, 0]);
    }

    private static void EmitMovRax(List<byte> code, ulong value) { code.AddRange([0x48, 0xB8]); code.AddRange(BitConverter.GetBytes(value)); }
    private static void EmitMovRcx(List<byte> code, ulong value) { code.AddRange([0x48, 0xB9]); code.AddRange(BitConverter.GetBytes(value)); }
    private static void EmitMovRdx(List<byte> code, ulong value) { code.AddRange([0x48, 0xBA]); code.AddRange(BitConverter.GetBytes(value)); }
    private static void EmitMovR8(List<byte> code, ulong value) { code.AddRange([0x49, 0xB8]); code.AddRange(BitConverter.GetBytes(value)); }
    private static void EmitMovR9(List<byte> code, ulong value) { code.AddRange([0x49, 0xB9]); code.AddRange(BitConverter.GetBytes(value)); }
    private static void EmitCallRax(List<byte> code) => code.AddRange([0xFF, 0xD0]);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint VirtualAllocEx(nint hProcess, nint lpAddress, nuint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool VirtualFreeEx(nint hProcess, nint lpAddress, nuint dwSize, uint dwFreeType);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool VirtualProtectEx(nint hProcess, nint lpAddress, nuint dwSize, uint flNewProtect, out uint lpflOldProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, nuint nSize, out nuint lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint CreateRemoteThread(nint hProcess, nint lpThreadAttributes, nuint dwStackSize, nint lpStartAddress, nint lpParameter, uint dwCreationFlags, out uint lpThreadId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint WaitForSingleObject(nint hHandle, uint dwMilliseconds);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetExitCodeThread(nint hThread, out uint lpExitCode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(nint hObject);
}
