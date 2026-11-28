using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Silverfish.GameProbe;

public static class ProbeEntry
{
    private const string GenerationKey = "Silverfish.GameProbe.Generation";
    private static readonly object Sync = new object();
    private static readonly object LogSync = new object();
    private static Timer? _pollTimer;
    private static volatile bool _running;
    private static volatile bool _dirty = true;
    private static int _inTick;
    private static int _lastFingerprint;
    private static bool _sentIdleFrame;
    private static long _revision;
    private static string _generation = string.Empty;
    private static object? _boundGameState;
    private static string _consoleEndpoint = "http://127.0.0.1:5088";
    private static string _matchId = "live-" + Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture);
    private static string _logPath = Path.Combine(Path.GetTempPath(), "SilverfishProbe", "probe.log");

    public static void Initialize()
    {
        var endpoint = Environment.GetEnvironmentVariable("SILVERFISH_CONSOLE_URL");
        var matchId = Environment.GetEnvironmentVariable("SILVERFISH_MATCH_ID");
        Start(endpoint, matchId);
    }

    public static void Start(string? consoleEndpoint = null, string? matchId = null)
    {
        try
        {
            var baseDir = Path.GetDirectoryName(typeof(ProbeEntry).Assembly.Location);
            if (!string.IsNullOrWhiteSpace(baseDir))
                _logPath = Path.Combine(baseDir!, "probe.log");

            var cfgPath = Path.Combine(baseDir ?? Path.Combine(Path.GetTempPath(), "SilverfishProbe"), "Silverfish.GameProbe.cfg");
            if (File.Exists(cfgPath))
            {
                var lines = File.ReadAllLines(cfgPath, Encoding.UTF8);
                if (string.IsNullOrWhiteSpace(consoleEndpoint) && lines.Length >= 1 && !string.IsNullOrWhiteSpace(lines[0]))
                    consoleEndpoint = lines[0].Trim();
                if (string.IsNullOrWhiteSpace(matchId) && lines.Length >= 2 && !string.IsNullOrWhiteSpace(lines[1]))
                    matchId = lines[1].Trim();
            }
        }
        catch (Exception ex)
        {
            WriteLog("Config read warning: " + ex.Message);
        }

        lock (Sync)
        {
            if (!string.IsNullOrWhiteSpace(consoleEndpoint))
                _consoleEndpoint = consoleEndpoint!.TrimEnd('/');
            if (!string.IsNullOrWhiteSpace(matchId))
                _matchId = matchId!;

            _generation = Guid.NewGuid().ToString("N");
            AppDomain.CurrentDomain.SetData(GenerationKey, _generation);

            _pollTimer?.Dispose();
            _running = true;
            _dirty = true;
            _sentIdleFrame = false;
            _lastFingerprint = 0;
            _revision = 0;
            _boundGameState = null;
            VirtualInputExecutor.Initialize(SendCommandAck, WriteLog);
            WriteLog($"Probe started gen={_generation} endpoint={_consoleEndpoint} matchId={_matchId}");
            _pollTimer = new Timer(OnTick, null, 50, 200);
        }
    }

    public static void Stop()
    {
        lock (Sync)
        {
            _running = false;
            _pollTimer?.Dispose();
            _pollTimer = null;
            _boundGameState = null;
            WriteLog($"Probe stopped gen={_generation}");
        }
    }

    public static void MarkDirty() => _dirty = true;

    private static void OnTick(object? state)
    {
        if (!_running)
            return;
        if (Interlocked.CompareExchange(ref _inTick, 1, 0) != 0)
            return;

        try
        {
            if (!string.Equals(AppDomain.CurrentDomain.GetData(GenerationKey) as string, _generation, StringComparison.Ordinal))
            {
                Stop();
                return;
            }

            PollNextCommand();
            EnsureGameStateListeners();
            var snapshot = GameStateProbeCollector.CaptureFromCurrentDomain(_revision + 1, out var fingerprint);
            if (snapshot == null)
            {
                if (!_sentIdleFrame)
                {
                    var idleRev = Interlocked.Increment(ref _revision);
                    var idleJson = GameStateProbeCollector.SerializeJson(GameStateProbeCollector.CreateIdleFrame(idleRev));
                    if (PushSnapshot(idleJson))
                    {
                        _sentIdleFrame = true;
                        WriteLog($"Pushed idle heartbeat rev={idleRev} (waiting for match)");
                    }
                }
                return;
            }

            _sentIdleFrame = false;
            if (!_dirty && fingerprint == _lastFingerprint)
                return;

            var nextRevision = Interlocked.Increment(ref _revision);
            snapshot.Revision = nextRevision;

            var json = GameStateProbeCollector.SerializeJson(snapshot);
            if (PushSnapshot(json))
            {
                _dirty = false;
                _lastFingerprint = fingerprint;
                WriteLog($"Pushed live frame rev={nextRevision} entities={snapshot.Entities.Count} canAct={snapshot.CanAct} reason={snapshot.BlockReason}");
            }
        }
        catch (Exception ex)
        {
            WriteLog("OnTick error: " + ex.GetType().Name + ": " + ex.Message);
        }
        finally
        {
            Interlocked.Exchange(ref _inTick, 0);
        }
    }

    private static void EnsureGameStateListeners()
    {
        var gameStateType = GameStateProbeCollector.FindGameStateType();
        if (gameStateType == null)
            return;

        var getMethod = gameStateType.GetMethod("Get", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        var instance = getMethod?.Invoke(null, null);
        if (instance == null || ReferenceEquals(instance, _boundGameState))
            return;

        _boundGameState = instance;
        _dirty = true;
        WriteLog("Bound to active GameState instance: " + gameStateType.AssemblyQualifiedName);

        TryRegisterListener(instance, "RegisterCreateGameListener", nameof(OnFourArgs));
        TryRegisterListener(instance, "RegisterOptionsReceivedListener", nameof(OnOneArg));
        TryRegisterListener(instance, "RegisterEntityChoicesReceivedListener", nameof(OnThreeArgsBool));
        TryRegisterListener(instance, "RegisterEntitiesChosenReceivedListener", nameof(OnTwoArgsBool));
        TryRegisterListener(instance, "RegisterCurrentPlayerChangedListener", nameof(OnTwoArgs));
        TryRegisterListener(instance, "RegisterTurnChangedListener", nameof(OnThreeArgsInt));

        var powerProcessor = GameStateProbeCollector.Invoke(instance, "GetPowerProcessor");
        if (powerProcessor != null)
            TryRegisterListener(powerProcessor, "AddTaskEventListener", nameof(OnZeroArgs));
    }

    private static void OnZeroArgs() => _dirty = true;
    private static void OnOneArg(object? a) => _dirty = true;
    private static void OnTwoArgs(object? a, object? b) => _dirty = true;
    private static bool OnTwoArgsBool(object? a, object? b) { _dirty = true; return false; }
    private static bool OnThreeArgsBool(object? a, object? b, object? c) { _dirty = true; return false; }
    private static void OnThreeArgsInt(int a, int b, object? c) => _dirty = true;
    private static void OnFourArgs(int a, object? b, object? c, object? d) => _dirty = true;

    private static void TryRegisterListener(object target, string registerMethodName, string handlerName)
    {
        try
        {
            var handler = typeof(ProbeEntry).GetMethod(handlerName, BindingFlags.Static | BindingFlags.NonPublic);
            if (handler == null)
                return;

            foreach (var method in target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!string.Equals(method.Name, registerMethodName, StringComparison.Ordinal))
                    continue;
                var parameters = method.GetParameters();
                if (parameters.Length != 1)
                    continue;
                var delegateType = parameters[0].ParameterType;
                if (!typeof(Delegate).IsAssignableFrom(delegateType))
                    continue;

                var callback = Delegate.CreateDelegate(delegateType, handler, throwOnBindFailure: false);
                if (callback == null)
                    continue;

                method.Invoke(target, new object[] { callback });
                WriteLog("Registered listener: " + registerMethodName);
                return;
            }
        }
        catch (Exception ex)
        {
            WriteLog("Listener skipped (" + registerMethodName + "): " + ex.Message);
        }
    }

    private static bool PushSnapshot(string payloadJson)
    {
        try
        {
            var url = _consoleEndpoint + "/api/import?matchId=" + Uri.EscapeDataString(_matchId);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Proxy = null; // Critical on Windows/Mono: bypass system HTTP proxy for 127.0.0.1!
            request.KeepAlive = false;
            request.ContentType = "application/json; charset=utf-8";
            request.Headers["X-Silverfish-Console"] = "1";
            request.Timeout = 2000;
            request.ReadWriteTimeout = 2000;

            var bytes = Encoding.UTF8.GetBytes(payloadJson);
            request.ContentLength = bytes.Length;
            using (var stream = request.GetRequestStream())
                stream.Write(bytes, 0, bytes.Length);
            using (var response = (HttpWebResponse)request.GetResponse())
                return (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;
        }
        catch (WebException webEx)
        {
            string? body = null;
            try
            {
                if (webEx.Response != null)
                {
                    using var reader = new StreamReader(webEx.Response.GetResponseStream(), Encoding.UTF8);
                    body = reader.ReadToEnd();
                }
            }
            catch { }
            WriteLog($"HTTP push failed: {webEx.Status} {webEx.Message} {body}");
            return false;
        }
        catch (Exception ex)
        {
            WriteLog($"HTTP push exception: {ex.GetType().Name}: {ex.Message}");
            return false;
        }
    }

    private static void PollNextCommand()
    {
        try
        {
            var url = _consoleEndpoint + "/api/commands/next";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Proxy = null;
            request.KeepAlive = false;
            request.Timeout = 1000;
            request.ReadWriteTimeout = 1000;

            using var response = (HttpWebResponse)request.GetResponse();
            if ((int)response.StatusCode != 200)
                return;
            using var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            var text = reader.ReadToEnd();
            if (string.IsNullOrWhiteSpace(text) || text.Trim() == "null")
                return;

            var cmd = ParseSimpleCommand(text);
            if (cmd != null && !string.IsNullOrEmpty(cmd.Id) && !string.IsNullOrEmpty(cmd.Kind))
                VirtualInputExecutor.Enqueue(cmd);
        }
        catch { }
    }

    private static void SendCommandAck(string id, bool ok, string message)
    {
        try
        {
            var url = _consoleEndpoint + "/api/commands/ack";
            var payload = "{\"id\":\"" + EscapeJson(id) + "\",\"ok\":" + (ok ? "true" : "false") +
                          ",\"message\":\"" + EscapeJson(message) + "\"}";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Proxy = null;
            request.KeepAlive = false;
            request.ContentType = "application/json; charset=utf-8";
            request.Headers["X-Silverfish-Console"] = "1";
            request.Timeout = 1500;
            var bytes = Encoding.UTF8.GetBytes(payload);
            request.ContentLength = bytes.Length;
            using (var stream = request.GetRequestStream())
                stream.Write(bytes, 0, bytes.Length);
            using var response = (HttpWebResponse)request.GetResponse();
            WriteLog($"Command ack sent id={id} ok={ok} msg={message}");
        }
        catch (Exception ex)
        {
            WriteLog("Command ack failed: " + ex.Message);
        }
    }

    private static ProbeCommand? ParseSimpleCommand(string json)
    {
        var id = ExtractJsonString(json, "id");
        var kind = ExtractJsonString(json, "kind");
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(kind))
            return null;
        return new ProbeCommand
        {
            Id = id!,
            Kind = kind!,
            SourceEntityId = ExtractJsonInt(json, "sourceEntityId"),
            TargetEntityId = ExtractJsonInt(json, "targetEntityId"),
            Position = ExtractJsonInt(json, "position")
        };
    }

    private static string? ExtractJsonString(string json, string key)
    {
        var token = "\"" + key + "\":\"";
        var idx = json.IndexOf(token, StringComparison.Ordinal);
        if (idx < 0) return null;
        idx += token.Length;
        var end = json.IndexOf('"', idx);
        return end > idx ? json.Substring(idx, end - idx) : null;
    }

    private static int? ExtractJsonInt(string json, string key)
    {
        var token = "\"" + key + "\":";
        var idx = json.IndexOf(token, StringComparison.Ordinal);
        if (idx < 0) return null;
        idx += token.Length;
        while (idx < json.Length && char.IsWhiteSpace(json[idx])) idx++;
        var start = idx;
        if (idx < json.Length && json[idx] == '-') idx++;
        while (idx < json.Length && char.IsDigit(json[idx])) idx++;
        return idx > start && int.TryParse(json.Substring(start, idx - start), NumberStyles.Integer, CultureInfo.InvariantCulture, out var val)
            ? val : (int?)null;
    }

    private static string EscapeJson(string text) =>
        string.IsNullOrEmpty(text) ? string.Empty : text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", " ").Replace("\n", " ");

    private static void WriteLog(string message)
    {
        try
        {
            lock (LogSync)
            {
                var dir = Path.GetDirectoryName(_logPath);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir!);
                if (File.Exists(_logPath) && new FileInfo(_logPath).Length > 512 * 1024)
                    File.WriteAllText(_logPath, string.Empty, Encoding.UTF8);
                File.AppendAllText(_logPath, $"[{DateTime.Now:HH:mm:ss.fff}] {message}{Environment.NewLine}", Encoding.UTF8);
            }
        }
        catch { }
    }
}
