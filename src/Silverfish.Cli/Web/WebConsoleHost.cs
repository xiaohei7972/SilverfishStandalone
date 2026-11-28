using System.Globalization;
using System.Net;
using System.Text.Json;
using Silverfish.Simulation;

namespace Silverfish.Cli.Web;

internal sealed class WebConsoleHost(int port, WebConsoleInput input, IClientProbe probe)
{
    internal const int MaxBody = 2 * 1024 * 1024;
    private readonly WebConsoleSession _session = new(input.Names);
    private readonly WebClientConnection _connection = new(probe);
    private readonly string _authority = $"127.0.0.1:{port}";

    internal string Origin => $"http://{_authority}";

    internal static async Task<int> RunCommandAsync(string[] args)
    {
        const string usage = "用法  Silverfish.Cli ui [--port 5088] [--probe <探测程序.exe>] [--card-data <卡牌目录.json>]";
        if (args is ["--help"] or ["-h"])
        {
            Console.WriteLine(usage);
            return 0;
        }
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        const int defaultPort = 5088;
        var port = defaultPort;
        for (var index = 0; index < args.Length; index += 2)
        {
            if (index + 1 >= args.Length || args[index] is not ("--port" or "--probe" or "--card-data") ||
                string.IsNullOrWhiteSpace(args[index + 1]) || !values.TryAdd(args[index], args[index + 1]))
            {
                Console.Error.WriteLine(usage);
                return 2;
            }
        }
        if (values.TryGetValue("--port", out var value) &&
            (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out port) || port is < 1024 or > 65535))
        {
            Console.Error.WriteLine("端口必须是 1024 到 65535 之间的整数。");
            return 2;
        }

        using var cancellation = new CancellationTokenSource();
        ConsoleCancelEventHandler cancel = (_, signal) => { signal.Cancel = true; cancellation.Cancel(); };
        Console.CancelKeyPress += cancel;
        try
        {
            CardDataCatalog? catalog = null;
            if (values.TryGetValue("--card-data", out var cards))
            {
                using var stream = File.OpenRead(cards);
                catalog = CardDataCatalog.LoadJson(stream);
            }
            var helper = values.TryGetValue("--probe", out var path) ? Path.GetFullPath(path) : null;
            var host = new WebConsoleHost(port, new(catalog), new ProcessClientProbe(helper));
            await host.RunAsync(cancellation.Token, () => Console.WriteLine($"独立银鱼控制台：{host.Origin}/（按 Ctrl+C 停止）"));
            return 0;
        }
        catch (Exception error) when (error is HttpListenerException or IOException or ArgumentException or
            InvalidDataException or UnauthorizedAccessException or JsonException or InvalidOperationException)
        {
            Console.Error.WriteLine($"无法启动控制台：{error.Message}");
            return 1;
        }
        finally { Console.CancelKeyPress -= cancel; }
    }

    internal async Task RunAsync(CancellationToken token, Action started)
    {
        using var listener = new HttpListener();
        listener.Prefixes.Add($"{Origin}/");
        listener.Start();
        using var shutdown = CancellationTokenSource.CreateLinkedTokenSource(token);
        using var stop = shutdown.Token.Register(listener.Stop);
        var running = new List<Task>();
        var decisions = PollAsync(TimeSpan.FromMilliseconds(750), () => _session.DecideAsync(null, shutdown.Token), shutdown.Token);
        var autoPilot = PollAsync(TimeSpan.FromMilliseconds(900), () => _session.DispatchAutoPilotAsync(shutdown.Token), shutdown.Token);
        var presence = PollAsync(TimeSpan.FromSeconds(4), () => _connection.RefreshAsync(shutdown.Token), shutdown.Token);
        try
        {
            started();
            while (!shutdown.IsCancellationRequested)
            {
                var context = await listener.GetContextAsync();
                running.RemoveAll(task => task.IsCompleted);
                if (running.Count >= 64)
                {
                    context.Response.StatusCode = 503;
                    context.Response.Close();
                    continue;
                }
                running.Add(RespondAsync(context, shutdown.Token));
            }
        }
        catch (Exception error) when (shutdown.IsCancellationRequested && error is HttpListenerException or ObjectDisposedException or InvalidOperationException) { }
        finally
        {
            shutdown.Cancel();
            await Task.WhenAll([.. running, decisions, autoPilot, presence]);
        }
    }

    private static async Task PollAsync(TimeSpan period, Func<Task> callback, CancellationToken token)
    {
        using var timer = new PeriodicTimer(period);
        try
        {
            while (await timer.WaitForNextTickAsync(token))
                await callback();
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
    }

    private async Task RespondAsync(HttpListenerContext context, CancellationToken token)
    {
        var request = context.Request;
        var response = context.Response;
        response.Headers["Cache-Control"] = "no-store";
        response.Headers["X-Content-Type-Options"] = "nosniff";
        response.Headers["Cross-Origin-Resource-Policy"] = "same-origin";
        response.Headers["Content-Security-Policy"] = "default-src 'none'; script-src 'self'; style-src 'self'; img-src 'self'; connect-src 'self'; base-uri 'none'; frame-ancestors 'none'; form-action 'none'";
        response.KeepAlive = false;
        try
        {
            try
            {
                var origin = request.Headers["Origin"];
                if (!string.Equals(request.Headers["Host"], _authority, StringComparison.Ordinal) ||
                    (origin is not null && !string.Equals(origin, Origin, StringComparison.Ordinal)))
                    throw new ApiException(403, "只允许从本地控制台来源访问。");
                var route = request.Url?.AbsolutePath;
                if (request.HttpMethod == "GET")
                {
                    await GetAsync(route, request, response, token);
                    return;
                }
                if (request.HttpMethod != "POST")
                    throw new ApiException(405, "请求方法不受支持。");
                if (request.Headers["X-Silverfish-Console"] != "1")
                    throw new ApiException(403, "缺少控制台请求标记。");
                if (!string.Equals(request.ContentType?.Split(';')[0].Trim(), "application/json", StringComparison.OrdinalIgnoreCase))
                    throw new ApiException(415, "请求必须使用 application/json。");
                using var body = await ReadBodyAsync(request, token);
                await WriteJsonAsync(response, await PostAsync(route, request, body, token), token);
            }
            catch (Exception error) when (error is not OperationCanceledException)
            {
                var (status, message) = error switch
                {
                    ApiException api => (api.Status, api.Message),
                    ConsoleConflictException => (409, error.Message),
                    JsonException json => (400, $"JSON 格式或字段值无效（{json.Path ?? "$"}）。"),
                    KeyNotFoundException => (400, "找不到局面引用的玩家、实体或卡牌。"),
                    OverflowException => (400, "数值超出可处理范围。"),
                    ArgumentException or InvalidDataException or InvalidOperationException => (400, error.Message),
                    _ => (500, "控制台未能处理请求。")
                };
                response.StatusCode = status;
                await WriteJsonAsync(response, new { error = message }, token);
            }
        }
        catch (Exception error) when (error is HttpListenerException or IOException or ObjectDisposedException or OperationCanceledException) { }
        finally { response.Close(); }
    }

    private async Task GetAsync(string? route, HttpListenerRequest request, HttpListenerResponse response, CancellationToken token)
    {
        var asset = route switch
        {
            "/" => ("index.html", "text/html"),
            "/app.css" => ("app.css", "text/css"),
            "/app.js" => ("app.js", "text/javascript"),
            _ => default
        };
        if (asset.Item1 is { } name)
        {
            using var stream = WebConsoleInput.Resource($"Web/{name}");
            response.ContentType = $"{asset.Item2}; charset=utf-8";
            response.ContentLength64 = stream.Length;
            await stream.CopyToAsync(response.OutputStream, token);
            return;
        }
        if (route == "/api/state")
        {
            var view = _session.Read(out var version);
            var etag = $"\"{version:N}\"";
            response.Headers["ETag"] = etag;
            if (request.Headers["If-None-Match"]?.Split(',', StringSplitOptions.TrimEntries)
                .Any(candidate => candidate == etag || candidate == "W/" + etag || candidate == "*") == true)
            {
                response.StatusCode = 304;
                return;
            }
            await WriteJsonAsync(response, view, token);
            return;
        }
        object? data = route switch
        {
            "/api/samples" => WebConsoleInput.Samples.Select(sample => new { name = sample.Key, kind = sample.Value }),
            "/api/clients" => await _connection.ListAsync(token),
            "/api/connection" => _connection.Read(),
            "/api/commands/next" => _session.DequeueCommand(),
            "/api/commands/last" => _session.ReadLastAck(),
            _ => throw new ApiException(404, "没有此页面或接口。")
        };
        await WriteJsonAsync(response, data, token);
    }

    private async Task<object?> PostAsync(string? route, HttpListenerRequest request, Stream body, CancellationToken token)
    {
        switch (route)
        {
            case "/api/import":
                var match = request.QueryString["matchId"];
                if (match?.Length > 128)
                    throw new ArgumentException("对局 ID 不能超过 128 个字符。");
                _session.Import(await input.LoadAsync(body, match?.Trim()));
                break;
            case "/api/sample":
                var sample = Read<SampleRequest>(body).Name;
                if (sample is null || !WebConsoleInput.Samples.ContainsKey(sample))
                    throw new ApiException(404, "没有此内置局面样例。");
                using (var stream = WebConsoleInput.Resource($"Samples/{sample}"))
                    _session.Import(await input.LoadAsync(stream, null, sample), sample: true);
                break;
            case "/api/reset":
                _session.Reset();
                break;
            case "/api/automatic":
                _session.SetAutomatic(Read<AutomaticRequest>(body).Enabled);
                break;
            case "/api/autopilot":
                _session.SetAutoPilot(Read<AutomaticRequest>(body).Enabled);
                break;
            case "/api/decide":
                await _session.DecideAsync(Read<DecisionRequest>(body).Version, token);
                break;
            case "/api/apply":
                var action = Read<ActionRequest>(body);
                _session.Apply(action.Version, action.Index);
                break;
            case "/api/bind":
                var client = Read<BindRequest>(body);
                return await _connection.BindAsync(client.Pid, client.StartTime, token);
            case "/api/inject":
                return await _connection.InjectAsync(Origin, token);
            case "/api/command":
                var req = Read<EnqueueCommandRequest>(body);
                if (string.IsNullOrWhiteSpace(req.Kind))
                    throw new ArgumentException("必须指定操作类型。");
                return _session.EnqueueCommand(req.Kind.Trim(), req.SourceEntityId, req.TargetEntityId, req.Position);
            case "/api/commands/ack":
                var ack = Read<CommandAck>(body);
                _session.RecordAck(ack);
                return ack;
            case "/api/disconnect":
                _connection.Disconnect();
                return null;
            default:
                throw new ApiException(404, "没有此页面或接口。");
        }
        return _session.Read();
    }

    private static T Read<T>(Stream body) => JsonSerializer.Deserialize<T>(body, WebConsoleInput.Json)
        ?? throw new InvalidDataException("必须提供 JSON 请求对象。");

    private static async Task<MemoryStream> ReadBodyAsync(HttpListenerRequest request, CancellationToken token)
    {
        if (request.ContentLength64 > MaxBody)
            throw new ApiException(413, "输入不能超过 2 MiB。");
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(token);
        deadline.CancelAfter(TimeSpan.FromSeconds(8));
        var body = new MemoryStream();
        try
        {
            var buffer = new byte[16 * 1024];
            while (await request.InputStream.ReadAsync(buffer, deadline.Token).AsTask().WaitAsync(deadline.Token) is var count && count > 0)
            {
                if (body.Length + count > MaxBody)
                    throw new ApiException(413, "输入不能超过 2 MiB。");
                body.Write(buffer, 0, count);
            }
            body.Position = 0;
            return body;
        }
        catch (OperationCanceledException) when (!token.IsCancellationRequested)
        {
            body.Dispose();
            throw new ApiException(408, "读取输入超时。");
        }
        catch { body.Dispose(); throw; }
    }

    private static async Task WriteJsonAsync(HttpListenerResponse response, object? body, CancellationToken token)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(body, WebConsoleInput.Json);
        response.ContentType = "application/json; charset=utf-8";
        response.ContentLength64 = bytes.Length;
        await response.OutputStream.WriteAsync(bytes, token);
    }

    private sealed record EnqueueCommandRequest { public required string Kind { get; init; } public int? SourceEntityId { get; init; } public int? TargetEntityId { get; init; } public int? Position { get; init; } }
    private sealed record SampleRequest { public required string Name { get; init; } }
    private sealed record AutomaticRequest { public required bool Enabled { get; init; } }
    private sealed record DecisionRequest { public required Guid Version { get; init; } }
    private sealed record ActionRequest { public required Guid Version { get; init; } public required int Index { get; init; } }
    private sealed record BindRequest
    {
        public required uint Pid { get; init; }
        [System.Text.Json.Serialization.JsonNumberHandling(System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString)]
        public required ulong StartTime { get; init; }
    }
    private sealed class ApiException(int status, string message) : Exception(message) { internal int Status { get; } = status; }
}
