using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Silverfish.Cli;
using Silverfish.Cli.Web;
using Silverfish.Contracts;
using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Tests;

public sealed class WebConsoleTests
{
    [Fact]
    public async Task ObservableInputsPreserveSequenceAndNeverApplyActions()
    {
        var input = new WebConsoleInput();
        var session = new WebConsoleSession(input.Names);
        using var source = WebConsoleInput.Resource("Samples/standalone-state-snapshot.json");
        var defaultIdPos = await input.LoadAsync(source, null);
        Assert.Equal("imported-match", defaultIdPos.MatchId);
        source.Position = 0;
        var position = await input.LoadAsync(source, "fixture-match");
        session.Import(position);
        var view = session.Read();
        Assert.Equal("fixture-match", view.MatchId);
        Assert.Equal("snapshot", view.Mode);
        Assert.Equal(position.Sequence.ToString(), view.Sequence);
        Assert.Equal("法师", view.Board!.OurHero.ClassName);
        Assert.True(view.Board.CanAct);
        Assert.Null(view.Board.BlockReason);
        Assert.Null(view.Board.ResponseMode);
        Assert.Contains(view.Board.OurHand, card => card is { CardId: "CS2_029", Name: "火球术" });
        Assert.NotNull(view.Board.EnemyHandCount);
        Assert.Empty(view.ObservedActions!);
        Assert.Null(view.LegalActions);
        Assert.Throws<ConsoleConflictException>(() => session.Apply(view.Version, 0));
        await session.DecideAsync(view.Version, default);
        Assert.Null(session.Read().Decision!.Baseline!.ChosenAction);
        session.Import(position with { Sequence = position.Sequence + 1 });
        Assert.Null(session.Read().Decision);
        session.Reset();
        session.Import(position);
        Assert.Equal(view.Sequence, session.Read().Sequence);
    }

    [Fact]
    public async Task ClientFramePreservesEntityTagsAndProjectsPartialMulligan()
    {
        var frame = ClientFrame();
        var players = frame["match"]!["players"]!;
        players[0]!.AsObject().Remove("zones");
        players[1]!["zones"] = JsonNode.Parse("""
            [{"name":"Battlefield","count":7,"hiddenCount":3,"entityIds":[22,22,923,21,31,32]},
             {"name":"Hand","count":4,"hiddenCount":2,"entityIds":[998,23]}]
            """);
        var entities = frame["entities"]!.AsArray();
        entities[0]!["tags"]![0]!["value"] = 4;
        entities.Add(JsonNode.Parse("""
            {"id":31,"cardId":"SYNTHETIC_ENCHANTMENT","controllerId":1,"zone":"GRAVEYARD","zonePosition":5,
             "cardType":"ENCHANTMENT","tags":[{"id":123456,"name":"untrusted-name","value":-12},{"id":45,"value":7}],
             "cachedTagsForDormant":[{"id":123456,"value":2147483647}]}
            """));
        entities.Add(JsonNode.Parse("""
            {"id":32,"cardId":"SYNTHETIC_PENDING","controllerId":1,"zone":"PLAY","cardType":"MINION"}
            """));
        entities.Add(JsonNode.Parse("""
            {"id":33,"cardId":"SYNTHETIC_READY","controllerId":1,"zone":"PLAY","zonePosition":3,"cardType":"MINION",
             "tags":[{"id":45,"value":2}]}
            """));

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(frame.ToJsonString()));
        var board = (await new WebConsoleInput().LoadAsync(stream, "partial")).Board;
        Assert.Equal(GamePhase.Mulligan, board.Phase);
        Assert.Equal(new[] { 22, 21, 33 }, board.OurBoard.Select(card => card.EntityId));
        Assert.Single(board.OurHand);
        Assert.Null(board.EnemyHandCount);
        Assert.Equal(entities.Count, board.AllEntities!.Length);
        var enchantment = Assert.Single(board.AllEntities, entity => entity.Id == 31);
        Assert.Equal("GRAVEYARD", enchantment.Zone);
        Assert.Equal(5, enchantment.ZonePosition);
        Assert.Equal(new ConsoleEntityTag(123456, "TAG_123456", -12), enchantment.Tags[0]);
        Assert.Equal(new ConsoleEntityTag(45, "HEALTH", 7), enchantment.Tags[1]);
        Assert.Equal(new ConsoleEntityTag(123456, "TAG_123456", int.MaxValue), Assert.Single(enchantment.CachedTagsForDormant));
        Assert.Empty(Assert.Single(board.AllEntities, entity => entity.Id == 32).Tags);
    }

    [Fact]
    public async Task ClientFrameRedactsPrivateEntitiesEvenWithConflictingZoneMetadata()
    {
        var frame = ClientFrame();
        frame["match"]!["players"]![0]!["zones"]![1]!["entityIds"] = new JsonArray(24);
        var entities = frame["entities"]!.AsArray();
        entities.Add(JsonNode.Parse("""
            {"id":31,"cardId":"SYNTHETIC_HAND","controllerId":2,"zone":"PLAY","cardType":"MINION",
             "tags":[{"id":49,"value":3},{"id":45,"value":5},{"id":123456,"value":-2}],
             "cachedTagsForDormant":[{"id":123456,"value":8}]}
            """));
        entities.Add(JsonNode.Parse("""
            {"id":32,"cardId":"SYNTHETIC_DECK","controllerId":2,"zone":"2","tags":[{"id":123456,"value":-2}]}
            """));
        entities.Add(JsonNode.Parse("""
            {"id":33,"cardId":"SYNTHETIC_SECRET","zone":"secret","tags":[{"id":123456,"value":-2}]}
            """));

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(frame.ToJsonString()));
        var board = (await new WebConsoleInput().LoadAsync(stream, "redacted")).Board;
        Assert.Empty(board.EnemyBoard);
        var redacted = board.AllEntities!.Where(entity => entity.IsRedacted).ToArray();
        Assert.Equal(new[] { 24, 31, 32, 33 }, redacted.Select(entity => entity.Id));
        Assert.All(redacted, entity =>
        {
            Assert.Null(entity.CardId);
            Assert.Null(entity.CardType);
            Assert.Empty(entity.Tags);
            Assert.Empty(entity.CachedTagsForDormant);
        });
        Assert.DoesNotContain("SYNTHETIC_", JsonSerializer.Serialize(board, WebConsoleInput.Json));
    }

    [Fact]
    public async Task PlannerAndApplyUseBoardVersionAndUpdateEventsAndPerspective()
    {
        var input = new WebConsoleInput();
        var session = new WebConsoleSession(input.Names);
        var position = await Sample(input, "simulation-combat.json");
        session.Import(position);
        var before = session.Read();
        session.SetAutomatic(true);
        await session.DecideAsync(null, default);
        var plan = Assert.IsType<Silverfish.Core.TurnPlan>(session.Read().Decision!.Plan);
        Assert.Contains(plan.Actions[0], before.LegalActions!);
        Assert.InRange(plan.WinProbability, 0, 1);
        Assert.Equal(before.Version, session.Read().Version);

        Assert.Throws<ArgumentException>(() => session.Apply(before.Version, -1));
        var endTurn = Assert.Single(position.Actions, action => action.Kind == SimulationActionKind.EndTurn);
        session.Apply(before.Version, position.Actions.IndexOf(endTurn));
        var after = session.Read();
        Assert.False(after.Board!.IsOurTurn);
        Assert.Equal(before.Board!.OurHero.EntityId, after.Board.OurHero.EntityId);
        Assert.Equal((position.Sequence + 1).ToString(), after.Sequence);
        Assert.Contains(after.Events!, entry => entry.Sequence == after.Sequence && entry.Event.Kind == SimulationEventKind.TurnStarted);
        Assert.Null(after.Decision);
        Assert.Throws<ConsoleConflictException>(() => session.Apply(before.Version, 0));
        await Assert.ThrowsAsync<ConsoleConflictException>(() => session.DecideAsync(before.Version, default));

        session.SetAutomatic(false);
        await session.DecideAsync(null, default);
        Assert.Null(session.Read().Decision);
        await session.DecideAsync(after.Version, default);
        Assert.NotNull(session.Read().Decision!.Plan);
        session.Reset();
        Assert.Null(session.Read().Board);
        Assert.Empty(session.Read().Events ?? []);
    }

    [Fact]
    public async Task EmbeddedSamplesAndVersionedCheckpointAreLoadable()
    {
        var input = new WebConsoleInput();
        Assert.Contains("simulation-card-data.json", WebConsoleInput.Samples.Keys);
        Assert.DoesNotContain("entity-choice-targets.json", WebConsoleInput.Samples.Keys);
        foreach (var name in WebConsoleInput.Samples.Keys)
            Assert.NotNull((await Sample(input, name)).Board);

        var pending = await Sample(input, "simulation-pending-choice.json");
        using var file = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(
            SimulationStateInput.Capture(pending.Simulator!, pending.Game!), WebConsoleInput.Json));
        var restored = await input.LoadAsync(file, "checkpoint-test");
        Assert.Equal("checkpoint-test", restored.MatchId);
        Assert.Equal(0, restored.Sequence);
        Assert.Equal(pending.Actions.Length, restored.Actions.Length);
        Assert.Equal(GamePhase.Choice, restored.Board.Phase);
    }

    [Fact]
    public async Task ClientBindingVerifiesIdentityAndSignatureAndDetectsPidReuse()
    {
        var probe = new FakeProbe();
        var connection = new WebClientConnection(probe);
        var selected = Assert.Single((await connection.ListAsync(default)).Clients);
        await Assert.ThrowsAsync<InvalidOperationException>(() => connection.BindAsync(selected.Pid, selected.StartTime - 1, default));
        probe.Signature = "ZZ";
        await Assert.ThrowsAsync<InvalidOperationException>(() => connection.BindAsync(selected.Pid, selected.StartTime, default));
        Assert.Null(connection.Read());
        probe.Signature = "MZ";
        Assert.Equal("online", (await connection.BindAsync(selected.Pid, selected.StartTime, default)).Status);
        probe.Client = selected with { StartTime = selected.StartTime + 1 };
        await connection.RefreshAsync(default);
        Assert.Equal("offline", connection.Read()!.Status);
        connection.Disconnect();
        Assert.Null(connection.Read());
    }

    [Fact]
    public async Task LoopbackApiServesAndProtectsOfflineWorkflow()
    {
        var socket = new TcpListener(IPAddress.Loopback, 0);
        socket.Start();
        var port = ((IPEndPoint)socket.LocalEndpoint).Port;
        socket.Stop();
        var host = new WebConsoleHost(port, new(), new FakeProbe());
        using var shutdown = new CancellationTokenSource(TimeSpan.FromSeconds(25));
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var running = host.RunAsync(shutdown.Token, () => started.SetResult());
        using var client = new HttpClient { BaseAddress = new(host.Origin), Timeout = TimeSpan.FromSeconds(10) };
        try
        {
            await await Task.WhenAny(started.Task, running).WaitAsync(shutdown.Token);
            Assert.Contains("对局战场", await client.GetStringAsync("/"));
            Assert.Contains("entity-inspector", await client.GetStringAsync("/"));
            Assert.Contains("function renderBoard", await client.GetStringAsync("/app.js"));
            using (var noMarker = await client.PostAsJsonAsync("/api/reset", new { }))
                Assert.Equal(HttpStatusCode.Forbidden, noMarker.StatusCode);
            using (var crossSite = await Post(client, "reset", "{}", "http://example.invalid"))
                Assert.Equal(HttpStatusCode.Forbidden, crossSite.StatusCode);
            using (var traversal = await Post(client, "sample", """{"name":"../README.md"}"""))
                Assert.Equal(HttpStatusCode.NotFound, traversal.StatusCode);
            using (var invalid = await Post(client, "import", """{"ourHero":{}}"""))
                Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
            using (var oversized = await Post(client, "import", new string(' ', WebConsoleHost.MaxBody + 1)))
                Assert.Equal(HttpStatusCode.RequestEntityTooLarge, oversized.StatusCode);

            var snapshot = await PostJson(client, "sample", """{"name":"classic-snapshot-lethal.json"}""");
            var decision = await PostJson(client, "decide", JsonSerializer.Serialize(new { version = snapshot.GetProperty("version") }));
            Assert.Equal("attack-face", decision.GetProperty("decision").GetProperty("baseline").GetProperty("chosenAction").GetProperty("id").GetString());

            var frame = ClientFrame();
            var sourceCard = frame["entities"]![6]!;
            var extraTag = JsonNode.Parse("""{"id":123456,"value":-11}""")!;
            sourceCard["tags"]!.AsArray().Add(extraTag);
            const string route = "import?matchId=stream-fixture";
            var first = await PostJson(client, route, frame.ToJsonString());
            using var fetched = await GetState(client);
            var firstEtag = fetched.Headers.ETag!.Tag;
            using (var unchanged = await GetState(client, firstEtag))
            {
                Assert.Equal(HttpStatusCode.NotModified, unchanged.StatusCode);
                Assert.Empty(await unchanged.Content.ReadAsByteArrayAsync());
            }

            frame["revision"] = 43;
            sourceCard["tags"]![0]!["value"] = 5;
            extraTag["value"] = -12;
            await PostJson(client, route, frame.ToJsonString());
            using var updated = await GetState(client, firstEtag);
            Assert.Equal(HttpStatusCode.OK, updated.StatusCode);
            var latest = await updated.Content.ReadFromJsonAsync<JsonElement>();
            Assert.NotEqual(first.GetProperty("version").GetString(), latest.GetProperty("version").GetString());
            Assert.Equal("43", latest.GetProperty("sequence").GetString());
            var latestBoard = latest.GetProperty("board");
            Assert.Equal(5, latestBoard.GetProperty("ourBoard")[0].GetProperty("health").GetInt32());
            var entity = Assert.Single(latestBoard.GetProperty("allEntities").EnumerateArray(), item => item.GetProperty("id").GetInt32() == 22);
            var tag = Assert.Single(entity.GetProperty("tags").EnumerateArray(), item => item.GetProperty("id").GetInt32() == 123456);
            Assert.Equal("TAG_123456", tag.GetProperty("name").GetString());
            Assert.Equal(-12, tag.GetProperty("value").GetInt32());
            using (var unchanged = await GetState(client, updated.Headers.ETag!.Tag))
                Assert.Equal(HttpStatusCode.NotModified, unchanged.StatusCode);

            await PostJson(client, "automatic", """{"enabled":true}""");
            using (var settings = await GetState(client, updated.Headers.ETag!.Tag))
            {
                var state = await settings.Content.ReadFromJsonAsync<JsonElement>();
                Assert.True(state.GetProperty("automatic").GetBoolean());
                Assert.Equal(latest.GetProperty("version").GetString(), state.GetProperty("version").GetString());
            }
            await PostJson(client, "automatic", """{"enabled":false}""");

            var simulation = await PostJson(client, "sample", """{"name":"simulation-combat.json"}""");
            var action = JsonSerializer.Serialize(new { version = simulation.GetProperty("version"), index = 0 });
            var changed = await PostJson(client, "apply", action);
            Assert.NotEqual(simulation.GetProperty("sequence").GetString(), changed.GetProperty("sequence").GetString());
            Assert.True(changed.GetProperty("events").GetArrayLength() > 0);
            using (var stale = await Post(client, "apply", action))
                Assert.Equal(HttpStatusCode.Conflict, stale.StatusCode);

            var instances = await client.GetFromJsonAsync<JsonElement>("/api/clients");
            var instance = instances.GetProperty("clients")[0];
            Assert.Equal(JsonValueKind.String, instance.GetProperty("startTime").ValueKind);
            var bound = await PostJson(client, "bind", JsonSerializer.Serialize(new
                { pid = instance.GetProperty("pid"), startTime = instance.GetProperty("startTime") }));
            Assert.Equal("online", bound.GetProperty("status").GetString());
            var injected = await PostJson(client, "inject", "{}");
            Assert.True(injected.GetProperty("injected").GetBoolean());
            Assert.Equal(JsonValueKind.Null, (await PostJson(client, "disconnect", "{}")).ValueKind);
        }
        finally
        {
            shutdown.Cancel();
            await running;
        }
    }

    [Fact]
    public async Task LoopbackStateReflectsOfflineImportAndReset()
    {
        var socket = new TcpListener(IPAddress.Loopback, 0);
        socket.Start();
        var port = ((IPEndPoint)socket.LocalEndpoint).Port;
        socket.Stop();
        var host = new WebConsoleHost(port, new(), new FakeProbe());
        using var shutdown = new CancellationTokenSource(TimeSpan.FromSeconds(25));
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var running = host.RunAsync(shutdown.Token, () => started.SetResult());
        using var client = new HttpClient(new HttpClientHandler { UseProxy = false })
            { BaseAddress = new(host.Origin), Timeout = TimeSpan.FromSeconds(10) };
        try
        {
            await await Task.WhenAny(started.Task, running).WaitAsync(shutdown.Token);
            using var initial = await GetState(client);
            Assert.Equal(HttpStatusCode.OK, initial.StatusCode);
            var initialState = await initial.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal(JsonValueKind.Null, initialState.GetProperty("board").ValueKind);
            Assert.Equal(JsonValueKind.Null, initialState.GetProperty("matchId").ValueKind);
            var initialEtag = initial.Headers.ETag!.Tag;
            using (var unchanged = await GetState(client, initialEtag))
                Assert.Equal(HttpStatusCode.NotModified, unchanged.StatusCode);

            var imported = await PostJson(client, "import?matchId=offline-fixture", ClientFrame().ToJsonString());
            using var loaded = await GetState(client, initialEtag);
            Assert.Equal(HttpStatusCode.OK, loaded.StatusCode);
            var loadedState = await loaded.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal("offline-fixture", loadedState.GetProperty("matchId").GetString());
            Assert.Equal(imported.GetProperty("version").GetGuid(), loadedState.GetProperty("version").GetGuid());
            Assert.Equal(JsonValueKind.Object, loadedState.GetProperty("board").ValueKind);

            var loadedEtag = loaded.Headers.ETag!.Tag;
            await PostJson(client, "reset", "{}");
            using var cleared = await GetState(client, loadedEtag);
            Assert.Equal(HttpStatusCode.OK, cleared.StatusCode);
            var clearedState = await cleared.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal(JsonValueKind.Null, clearedState.GetProperty("board").ValueKind);
            Assert.Equal(JsonValueKind.Null, clearedState.GetProperty("matchId").ValueKind);
            using var clearedUnchanged = await GetState(client, cleared.Headers.ETag!.Tag);
            Assert.Equal(HttpStatusCode.NotModified, clearedUnchanged.StatusCode);
        }
        finally
        {
            shutdown.Cancel();
            await running;
        }
    }

    [Fact]
    public async Task LoopbackAutoPilotQueuesChoicesAndTurnActionsAndStopsWhenDisabled()
    {
        using var socket = new TcpListener(IPAddress.Loopback, 0);
        socket.Start();
        var port = ((IPEndPoint)socket.LocalEndpoint).Port;
        socket.Stop();
        var host = new WebConsoleHost(port, new WebConsoleInput(), new FakeProbe());
        using var shutdown = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var running = host.RunAsync(shutdown.Token, () => started.SetResult());
        using var client = new HttpClient(new HttpClientHandler { UseProxy = false }) { BaseAddress = new(host.Origin) };

        Task<JsonElement> NextCommandAsync() => client.GetFromJsonAsync<JsonElement>("/api/commands/next", shutdown.Token);

        async Task<JsonElement> WaitForCommandAsync()
        {
            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(shutdown.Token);
            deadline.CancelAfter(TimeSpan.FromSeconds(4));
            while (true)
            {
                var command = await client.GetFromJsonAsync<JsonElement>("/api/commands/next", deadline.Token);
                if (command.ValueKind != JsonValueKind.Null)
                    return command;
                await Task.Delay(50, deadline.Token);
            }
        }

        try
        {
            await await Task.WhenAny(started.Task, running).WaitAsync(shutdown.Token);
            var initial = await client.GetFromJsonAsync<JsonElement>("/api/state", shutdown.Token);
            Assert.False(initial.GetProperty("autoPilot").GetBoolean());
            Assert.Equal(JsonValueKind.Null, initial.GetProperty("lastCommandAck").ValueKind);
            var enabled = await PostJson(client, "autopilot", """{"enabled":true}""");
            Assert.True(enabled.GetProperty("autoPilot").GetBoolean());
            Assert.False(enabled.GetProperty("automatic").GetBoolean());

            var frame = ClientFrame();
            frame["legalActions"] = JsonNode.Parse("""
                [{"id":"end","kind":"endTurn"},{"id":"attack","kind":"attack","sourceEntityId":21,"targetEntityId":11}]
                """);
            frame["pendingChoice"] = JsonNode.Parse("""
                {"choiceId":7,"countMin":2,"countMax":2,"hasConfirmButton":true,"cards":[
                  {"entityId":41,"cardType":"MINION","cost":3,"attack":3,"health":2},
                  {"entityId":42,"cardType":"MINION","cost":2,"attack":4,"health":3},
                  {"entityId":43,"cardType":"MINION","attack":9,"health":9,"unchoosable":true},
                  {"entityId":44,"cardType":"MINION","attack":8,"health":8,"chosen":true}
                ]}
                """);
            await PostJson(client, "import", frame.ToJsonString());
            var choice = await WaitForCommandAsync();
            Assert.Equal("choose_entity", choice.GetProperty("kind").GetString());
            Assert.Equal(42, choice.GetProperty("sourceEntityId").GetInt32());
            await Task.Delay(1100, shutdown.Token);
            Assert.Equal(JsonValueKind.Null, (await NextCommandAsync()).ValueKind);

            frame["revision"] = 43;
            frame["pendingChoice"]!["cards"]![1]!["chosen"] = true;
            await PostJson(client, "import", frame.ToJsonString());
            Assert.Equal("confirm_choice", (await WaitForCommandAsync()).GetProperty("kind").GetString());

            frame["revision"] = 44;
            frame.Remove("pendingChoice");
            await PostJson(client, "import", frame.ToJsonString());
            var attack = await WaitForCommandAsync();
            Assert.Equal("attack", attack.GetProperty("kind").GetString());
            Assert.Equal(21, attack.GetProperty("sourceEntityId").GetInt32());
            Assert.Equal(11, attack.GetProperty("targetEntityId").GetInt32());

            using var beforeAck = await GetState(client);
            var beforeAckState = await beforeAck.Content.ReadFromJsonAsync<JsonElement>();
            var id = attack.GetProperty("id").GetString();
            await PostJson(client, "commands/ack", JsonSerializer.Serialize(new { id, ok = true, message = "已完成攻击操作。" }));
            using var afterAck = await GetState(client, beforeAck.Headers.ETag!.Tag);
            Assert.Equal(HttpStatusCode.OK, afterAck.StatusCode);
            var afterAckState = await afterAck.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal(beforeAckState.GetProperty("version").GetGuid(), afterAckState.GetProperty("version").GetGuid());
            var ack = afterAckState.GetProperty("lastCommandAck");
            Assert.Equal(id, ack.GetProperty("id").GetString());
            Assert.True(ack.GetProperty("ok").GetBoolean());
            Assert.Equal("已完成攻击操作。", ack.GetProperty("message").GetString());
            Assert.Equal("attack", afterAckState.GetProperty("lastCommandAction").GetProperty("kind").GetString());

            await PostJson(client, "command", """{"kind":"endTurn"}""");
            var disabled = await PostJson(client, "autopilot", """{"enabled":false}""");
            Assert.False(disabled.GetProperty("autoPilot").GetBoolean());
            Assert.Equal(JsonValueKind.Null, (await NextCommandAsync()).ValueKind);
            frame["revision"] = 45;
            await PostJson(client, "import", frame.ToJsonString());
            await Task.Delay(1100, shutdown.Token);
            Assert.Equal(JsonValueKind.Null, (await NextCommandAsync()).ValueKind);
        }
        finally
        {
            shutdown.Cancel();
            await running;
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task LoopbackAutoPilotWaitsUntilClientCanAct(bool withPendingChoice)
    {
        using var socket = new TcpListener(IPAddress.Loopback, 0);
        socket.Start();
        var port = ((IPEndPoint)socket.LocalEndpoint).Port;
        socket.Stop();
        var host = new WebConsoleHost(port, new WebConsoleInput(), new FakeProbe());
        using var shutdown = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var running = host.RunAsync(shutdown.Token, () => started.SetResult());
        using var client = new HttpClient(new HttpClientHandler { UseProxy = false }) { BaseAddress = new(host.Origin) };
        const string importRoute = "import?matchId=readiness-fixture";
        const string nextRoute = "/api/commands/next";

        try
        {
            await await Task.WhenAny(started.Task, running).WaitAsync(shutdown.Token);
            var frame = ClientFrame();
            frame["canAct"] = false;
            frame["blockReason"] = "动画播放中";
            frame["responseMode"] = "OPTION";
            frame["legalActions"] = JsonNode.Parse("""
                [{"id":"attack","kind":"attack","sourceEntityId":21,"targetEntityId":11}]
                """);
            if (withPendingChoice)
                frame["pendingChoice"] = JsonNode.Parse("""
                    {"choiceId":7,"countMin":1,"countMax":1,"isSingleChoice":true,"cards":[{"entityId":41}]}
                    """);
            await PostJson(client, importRoute, frame.ToJsonString());
            Assert.True((await PostJson(client, "autopilot", """{"enabled":true}""")).GetProperty("autoPilot").GetBoolean());

            var blocked = await client.GetFromJsonAsync<JsonElement>("/api/state", shutdown.Token);
            var board = blocked.GetProperty("board");
            Assert.True(board.GetProperty("isOurTurn").GetBoolean());
            Assert.False(board.GetProperty("canAct").GetBoolean());
            Assert.Equal("动画播放中", board.GetProperty("blockReason").GetString());
            Assert.Equal("OPTION", board.GetProperty("responseMode").GetString());
            await Task.Delay(1800, shutdown.Token);
            Assert.Equal(JsonValueKind.Null, (await client.GetFromJsonAsync<JsonElement>(nextRoute, shutdown.Token)).ValueKind);

            frame["revision"] = 43;
            frame["canAct"] = true;
            frame["blockReason"] = null;
            var ready = await PostJson(client, importRoute, frame.ToJsonString());
            Assert.Equal(blocked.GetProperty("matchId").GetString(), ready.GetProperty("matchId").GetString());
            Assert.Equal("43", ready.GetProperty("sequence").GetString());
            Assert.True(ready.GetProperty("board").GetProperty("canAct").GetBoolean());

            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(shutdown.Token);
            deadline.CancelAfter(TimeSpan.FromSeconds(3));
            JsonElement command;
            do
            {
                command = await client.GetFromJsonAsync<JsonElement>(nextRoute, deadline.Token);
                if (command.ValueKind == JsonValueKind.Null)
                    await Task.Delay(50, deadline.Token);
            } while (command.ValueKind == JsonValueKind.Null);

            Assert.Equal(withPendingChoice ? "choose_entity" : "attack", command.GetProperty("kind").GetString());
            Assert.Equal(withPendingChoice ? 41 : 21, command.GetProperty("sourceEntityId").GetInt32());
            if (!withPendingChoice)
                Assert.Equal(11, command.GetProperty("targetEntityId").GetInt32());
        }
        finally
        {
            shutdown.Cancel();
            await running;
        }
    }

    [Fact]
    public async Task AutoPilotRetriesOnlyAfterCooldownAndClearsStaleCommands()
    {
        var input = new WebConsoleInput();
        var clock = new ManualTimeProvider();
        var session = new WebConsoleSession(input.Names, clock);
        var position = await Sample(input, "lethal.json");
        session.SetAutoPilot(true);
        session.Import(position);
        var manual = session.EnqueueCommand("endTurn", null, null, null);
        await session.DispatchAutoPilotAsync(default);
        Assert.Equal(manual, session.DequeueCommand());
        Assert.Null(session.DequeueCommand());
        await session.DispatchAutoPilotAsync(default);
        var first = Assert.IsType<QueuedCommand>(session.DequeueCommand());
        Assert.Equal("attack", first.Kind);

        clock.Timestamp += TimeSpan.FromSeconds(4).Ticks;
        await session.DispatchAutoPilotAsync(default);
        Assert.Null(session.DequeueCommand());
        clock.Timestamp++;
        await session.DispatchAutoPilotAsync(default);
        var retry = Assert.IsType<QueuedCommand>(session.DequeueCommand());
        Assert.Equal(first with { Id = retry.Id }, retry);
        Assert.NotEqual(first.Id, retry.Id);

        session.Import(position);
        await session.DispatchAutoPilotAsync(default);
        session.Import(await Sample(input, "simulation-combat.json"));
        await session.DispatchAutoPilotAsync(default);
        Assert.Null(session.DequeueCommand());
        session.EnqueueCommand("endTurn", null, null, null);
        session.Reset();
        Assert.False(session.Read().AutoPilot);
        Assert.Null(session.DequeueCommand());
    }

    private sealed class ManualTimeProvider : TimeProvider
    {
        internal long Timestamp { get; set; }
        public override long TimestampFrequency => TimeSpan.TicksPerSecond;
        public override long GetTimestamp() => Timestamp;
    }

    private static JsonObject ClientFrame()
    {
        using var source = WebConsoleInput.Resource("Samples/standalone-state-snapshot.json");
        return JsonNode.Parse(source)!.AsObject();
    }

    private static async Task<HttpResponseMessage> GetState(HttpClient client, string? etag = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/state");
        if (etag is not null)
            request.Headers.IfNoneMatch.ParseAdd(etag);
        return await client.SendAsync(request);
    }

    private static async Task<ConsolePosition> Sample(WebConsoleInput input, string name)
    {
        using var stream = WebConsoleInput.Resource($"Samples/{name}");
        return await input.LoadAsync(stream, null, name);
    }

    private static async Task<HttpResponseMessage> Post(HttpClient client, string route, string json, string? origin = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/{route}")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Add("X-Silverfish-Console", "1");
        if (origin is not null)
            request.Headers.Add("Origin", origin);
        return await client.SendAsync(request);
    }

    private static async Task<JsonElement> PostJson(HttpClient client, string route, string json)
    {
        using var response = await Post(client, route, json);
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"{route}: {response.StatusCode}: {body}");
        return JsonSerializer.Deserialize<JsonElement>(body);
    }

    private sealed class FakeProbe : IClientProbe
    {
        internal ClientInstance Client { get; set; } = new(37, ulong.MaxValue - 1, "Hearthstone.exe", "X64", 64);
        internal string Signature { get; set; } = "MZ";
        public string? Unavailable => null;
        public Task<ClientInstance[]> ListAsync(CancellationToken token) => Task.FromResult<ClientInstance[]>([Client]);
        public Task<ClientInspection> InspectAsync(uint pid, ulong startTime, CancellationToken token) =>
            Task.FromResult(new ClientInspection(Client, Signature, ["Hearthstone.exe"]));
        public Task InjectAsync(uint pid, ulong startTime, string consoleUrl, CancellationToken token) =>
            Task.CompletedTask;
    }
}
