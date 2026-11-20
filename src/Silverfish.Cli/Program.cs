using System.Collections.Immutable;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using Silverfish.Cli;
using Silverfish.Contracts;
using Silverfish.Core;
using Silverfish.Domain;
using Silverfish.Simulation;

Console.OutputEncoding = Encoding.UTF8;

var command = args.FirstOrDefault();
var choiceReplay = command == "choice" && args.Length == 2;
var simulationCommand = command is "simulate" or "plan" or "decide" or "checkpoint";
var simulation = simulationCommand && (args.Length == 2 || (args.Length == 4 && args[2] == "--card-data"));
var plan = simulation && command == "plan";
var decide = simulation && command == "decide";
var checkpoint = simulation && command == "checkpoint";
var import = command == "import-card-data" && args.Length is 3 or 4 &&
    (args.Length == 3 || args[3] == "--starter");
var generateSim = command == "generate-sim" && args.Length >= 4;
var snapshotInput = args.Length == 1 && !simulationCommand && command is not ("import-card-data" or "generate-sim") &&
    command is not null && !command.StartsWith('-');
var help = args.Length == 1 && command is "--help" or "-h";
if (help || (!simulation && !import && !generateSim && !snapshotInput && !choiceReplay))
{
    var output = help ? Console.Out : Console.Error;
    if (!help)
        output.WriteLine("命令或参数无效。");
    output.WriteLine("用法  Silverfish.Cli <对局快照.json>");
    output.WriteLine("       Silverfish.Cli simulate <回放场景.json> [--card-data <卡牌目录.json>]");
    output.WriteLine("       Silverfish.Cli choice <选择包.json>");
    output.WriteLine("       Silverfish.Cli plan <搜索场景.json> [--card-data <卡牌目录.json>]");
    output.WriteLine("       Silverfish.Cli decide <决策请求.json> [--card-data <卡牌目录.json>]");
    output.WriteLine("       Silverfish.Cli checkpoint <回放场景.json> [--card-data <卡牌目录.json>]");
    output.WriteLine("       Silverfish.Cli import-card-data <CardDefs.xml> <卡牌目录.json 或 Generated.g.cs> [--starter]");
    output.WriteLine("       Silverfish.Cli generate-sim <CardDefs.xml 或 卡牌目录.json> <Cards 目录> <卡牌 ID> [...]");
    return help ? 0 : 2;
}

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

try
{
    if (import)
    {
        ImportCardData(args[1], args[2], args.Length == 4);
        return 0;
    }
    if (generateSim)
    {
        CardSimGenerator.Generate(args[1], args[2], args.Skip(3));
        return 0;
    }

    await using var stream = File.OpenRead(simulation || choiceReplay ? args[1] : args[0]);
    if (choiceReplay)
    {
        var strictOptions = new JsonSerializerOptions(options) { UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow };
        var request = await JsonSerializer.DeserializeAsync<EntityChoiceReplayRequest>(stream, strictOptions)
            ?? throw new InvalidDataException("选择包 JSON 不能为空。");
        if (request.SchemaVersion != 1)
            throw new InvalidDataException($"不支持选择包版本 {request.SchemaVersion}。");
        if (request.Entities is null || request.UnchoosableEntities is null || request.Clicks is null)
            throw new InvalidDataException("候选、不可选和点击列表必须是数组。");

        var packet = new EntityChoicePacket(request.ChoiceId, request.ChoiceType, new(request.PlayerId),
            request.SourceEntityId is int source ? new EntityId(source) : null,
            request.CountMin, request.CountMax,
            request.Entities.Select(id => new EntityId(id)).ToImmutableArray(),
            request.UnchoosableEntities.Select(id => new EntityId(id)).ToImmutableArray());
        var selection = EntityChoiceSelection.Begin(packet);
        foreach (var id in request.Clicks)
            selection = selection.Click(new EntityId(id));
        var submission = request.Confirm ? selection.Confirm(request.SubmitChoiceId ?? request.ChoiceId) : null;
        Console.WriteLine(JsonSerializer.Serialize(new
        {
            packet.ChoiceId, packet.Type, packet.CountMin, packet.CountMax,
            IsSingleChoice = packet.IsSingleChoice(), selection.CanConfirm,
            selection.ShouldSubmitImmediately,
            SelectedEntityIds = selection.OrderedEntityIds.Select(id => id.Value).ToArray(),
            Submission = submission is null ? null : new
            {
                submission.ChoiceId,
                EntityIds = submission.OrderedEntityIds.Select(id => id.Value).ToArray()
            }
        }, options));
        return 0;
    }

    if (simulation)
    {
        CardDataCatalog? cardData = null;
        if (args.Length == 4)
        {
            using var cardStream = File.OpenRead(args[3]);
            cardData = CardDataCatalog.LoadJson(cardStream);
        }

        if (decide)
        {
            var requestOptions = new JsonSerializerOptions(options) { UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow };
            var request = await JsonSerializer.DeserializeAsync<SimulationDecisionRequest>(stream, requestOptions)
                ?? throw new InvalidDataException("输入 JSON 必须包含模拟决策请求。");
            if (request.SchemaVersion != 1)
                throw new InvalidDataException($"不支持模拟决策请求版本 {request.SchemaVersion}。");
            if (request.State is null)
                throw new InvalidDataException("模拟决策请求必须提供完整对局状态。");

            var (rules, decisionState) = request.State.CreateGame(cardData);
            var result = new SimulationDecisionEngine(rules).Decide(decisionState, request.MatchId,
                request.SnapshotSequence, request.Candidates);
            Console.WriteLine(JsonSerializer.Serialize(result, options));
            return 0;
        }

        var scenario = await JsonSerializer.DeserializeAsync<SimulationScenario>(stream, options)
            ?? throw new InvalidDataException("输入 JSON 必须包含模拟场景。");
        if (scenario.Actions is null && scenario.StateVersion is null)
            throw new InvalidDataException("场景动作必须使用数组。");
        var (simulator, state) = scenario.CreateGame(cardData);
        var steps = new List<SimulationStep>();
        foreach (var action in scenario.Actions ?? [])
        {
            var result = simulator.Apply(state, action);
            state = result.State;
            if (!checkpoint)
                steps.Add(new SimulationStep(action, result.Events));
        }

        if (checkpoint)
        {
            Console.WriteLine(JsonSerializer.Serialize(SimulationStateInput.Capture(simulator, state), options));
            return 0;
        }

        Console.WriteLine(JsonSerializer.Serialize(new
        {
            state.Turn, state.ActivePlayer, state.Outcome, state.Winner,
            state.MinionDeaths, state.RandomState,
            CardDataBuild = cardData?.SourceBuild ?? scenario.CompiledDataBuild,
            Players = state.Players.Values.OrderBy(player => player.Id.Value).ToArray(),
            Entities = state.Entities.Values.OrderBy(entity => entity.Id.Value).ToArray(),
            Steps = steps,
            LegalActions = simulator.LegalActions(state),
            Plan = plan ? new SimulationTurnPlanner(simulator, SimulationMaterialEvaluator.Evaluate).Plan(state) : null
        }, options));
        return 0;
    }

    var snapshot = await JsonSerializer.DeserializeAsync<GameSnapshot>(stream, options)
        ?? throw new InvalidDataException("输入 JSON 必须包含对局快照。");

    IStrategyEngine engine = new BaselineStrategyEngine();
    var decision = await engine.DecideAsync(snapshot);
    Console.WriteLine(JsonSerializer.Serialize(decision, options));
    return 0;
}
catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or XmlException or
    ArgumentException or InvalidDataException or InvalidOperationException or KeyNotFoundException or
    FormatException or OverflowException)
{
    var operation = choiceReplay ? "处理选择包" : generateSim ? "生成 Sim" : import ? "导入卡牌数据" : decide ? "模拟决策" : plan ? "搜索回合" :
        checkpoint ? "保存对局" : simulation ? "回放场景" : "分析快照";
    Console.Error.WriteLine($"{operation}失败。{DescribeFailure(ex)}");
    return 1;
}

static string DescribeFailure(Exception error) => error switch
{
    JsonException json => $"JSON 格式或字段值无效。字段 {json.Path ?? "$"}，第 {(json.LineNumber ?? 0) + 1} 行，" +
        $"第 {(json.BytePositionInLine ?? 0) + 1} 个字节。",
    XmlException xml => $"XML 格式无效。第 {xml.LineNumber} 行，第 {xml.LinePosition} 列。",
    FileNotFoundException { FileName: { } path } => $"找不到文件「{path}」。",
    FileNotFoundException => "找不到输入文件。",
    DirectoryNotFoundException => "文件所在的目录不存在。",
    UnauthorizedAccessException => "没有访问文件的权限。",
    InvalidDataException => error.Message,
    IOException => $"读写文件失败（错误代码 0x{error.HResult:X8}）。",
    OverflowException => "数值超出可处理的范围。",
    FormatException => "有字段的数值格式无效。",
    KeyNotFoundException => "找不到引用的玩家、实体或区域。",
    ArgumentException { ParamName: "path" or "fileName" } => "文件路径无效。",
    ArgumentException { ParamName: { } name } argument => DescribeArgument(argument, name),
    _ => error.Message
};

static string DescribeArgument(ArgumentException error, string name)
{
    // 运行时会按操作系统语言追加参数名。用中文显示该信息。
    var suffix = new ArgumentException("_", name).Message[1..];
    var message = error.Message;
    return message.EndsWith(suffix, StringComparison.Ordinal)
        ? $"{message[..^suffix.Length]}（参数 {name}）"
        : $"参数 {name} 无效。";
}

static void ImportCardData(string input, string output, bool starterOnly)
{
    var outputPath = Path.GetFullPath(output);
    var generateSource = outputPath.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase);
    var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
    if (string.Equals(Path.GetFullPath(input), outputPath, comparison))
        throw new ArgumentException("输出目录文件必须与输入 XML 使用不同的路径。");

    CardDataCatalog data;
    using (var source = File.OpenRead(input))
        data = CardDataCatalog.ImportXml(source);
    if (starterOnly)
    {
        var cards = StarterCardCatalog.FromCardData(data);
        _ = new GameSimulator(cards);
        data = data.SelectCards(cards.Select(card => card.Definition.CardId));
    }

    var directory = Path.GetDirectoryName(outputPath)!;
    Directory.CreateDirectory(directory);
    var temporaryPath = Path.Combine(directory, $"{Guid.NewGuid():N}.tmp");
    try
    {
        using (var destination = File.Open(temporaryPath, FileMode.CreateNew, FileAccess.Write))
        {
            if (generateSource)
                CardDataSourceWriter.Write(destination, data);
            else
                data.WriteJson(destination, indented: starterOnly);
        }
        File.Move(temporaryPath, outputPath, overwrite: true);
    }
    finally
    {
        File.Delete(temporaryPath);
    }
    Console.WriteLine($"已将构建 {data.SourceBuild} 的 {data.Cards.Length} 条卡牌资料导出到「{outputPath}」。");
}

internal record SimulationStateInput
{
    public const int CurrentStateVersion = 1;
    public int? StateVersion { get; init; }
    public string? CardSet { get; init; }
    public SimulationCard[] Cards { get; init; } = [];
    public CatalogCardRules[] CardRules { get; init; } = [];
    public required PlayerState[] Players { get; init; }
    public required GameEntity[] Entities { get; init; }
    public required PlayerId ActivePlayer { get; init; }
    public int Turn { get; init; } = 1;
    public MinionDeath[] MinionDeaths { get; init; } = [];
    public ulong RandomState { get; init; }
    public int? NextEntity { get; init; }
    public long? NextPlayOrder { get; init; }
    public MatchOutcome? Outcome { get; init; }
    public PlayerId? Winner { get; init; }

    internal int? CompiledDataBuild => CardSet == "starter" || CardRules.Length != 0 ? CompiledCardData.Build : null;

    public static SimulationStateInput Capture(GameSimulator simulator, GameState state) => new()
    {
        StateVersion = CurrentStateVersion,
        Cards = simulator.Cards.Values.OrderBy(card => card.Definition.CardId, StringComparer.Ordinal).ToArray(),
        Players = state.Players.Values.OrderBy(player => player.Id.Value).ToArray(),
        Entities = state.Entities.Values.OrderBy(entity => entity.Id.Value).ToArray(),
        ActivePlayer = state.ActivePlayer,
        Turn = state.Turn,
        MinionDeaths = [.. state.MinionDeaths],
        RandomState = state.RandomState,
        NextEntity = state.NextEntity,
        NextPlayOrder = state.NextPlayOrder,
        Outcome = state.Outcome,
        Winner = state.Winner
    };

    public (GameSimulator Simulator, GameState State) CreateGame(CardDataCatalog? cardData)
    {
        if (Cards is null || CardRules is null || Players is null || Entities is null || MinionDeaths is null)
            throw new InvalidDataException("卡牌、卡牌规则、玩家、实体和随从死亡记录必须使用数组。");

        var continuation = ReadContinuation();
        var simulator = new GameSimulator(BindCards(cardData));
        var state = continuation is null
            ? simulator.CreateGame(Players, Entities, ActivePlayer, Turn, MinionDeaths, RandomState)
            : simulator.RestoreGame(Players, Entities, ActivePlayer, Turn, MinionDeaths, RandomState, continuation);
        return (simulator, state);
    }

    private GameContinuation? ReadContinuation()
    {
        if (StateVersion is null)
        {
            if (NextEntity is not null || NextPlayOrder is not null || Outcome is not null || Winner is not null)
                throw new InvalidDataException("存档必须提供 stateVersion。");
            return null;
        }
        if (StateVersion != CurrentStateVersion)
            throw new InvalidDataException($"不支持对局存档版本 {StateVersion}。");
        if (NextEntity is not int entity || NextPlayOrder is not long order || Outcome is not MatchOutcome outcome)
            throw new InvalidDataException("存档必须提供下一个实体 ID、下一个进场顺序和对局结果。");
        return new(entity, order, outcome, Winner);
    }

    private IEnumerable<SimulationCard> BindCards(CardDataCatalog? cardData)
    {
        IEnumerable<SimulationCard> builtIn = CardSet switch
        {
            null => [],
            "starter" when cardData is not null => StarterCardCatalog.FromCardData(cardData),
            "starter" => StarterCardCatalog.Cards,
            _ => throw new InvalidDataException($"无法识别卡牌集「{CardSet}」。")
        };
        if (CardRules.Length == 0)
        {
            if (cardData is not null && CardSet is null)
                throw new InvalidDataException("使用 --card-data 时，场景必须指定 starter 卡牌集或提供 cardRules 条目。");
            return builtIn.Concat(Cards);
        }

        return builtIn.Concat(Cards).Concat(CardRules.Select(rules =>
        {
            if (string.IsNullOrWhiteSpace(rules?.CardId))
                throw new InvalidDataException("每条 cardRules 规则必须提供 cardId。");
            return cardData is null ? CompiledCardData.Bind(rules.CardId, rules) :
                SimulationCard.FromMetadata(cardData.GetCard(rules.CardId), rules);
        }));
    }
}

internal sealed record SimulationScenario : SimulationStateInput
{
    public SimulationAction[]? Actions { get; init; }
}

internal sealed record SimulationDecisionRequest
{
    public required int SchemaVersion { get; init; }
    public required string MatchId { get; init; }
    public required long SnapshotSequence { get; init; }
    public required SimulationStateInput State { get; init; }
    public required CandidateAction[] Candidates { get; init; }
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
internal sealed record CatalogCardRules : CardRules
{
    public required string CardId { get; init; }
}

internal sealed record SimulationStep(SimulationAction Action, IReadOnlyList<SimulationEvent> Events);

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
internal sealed record EntityChoiceReplayRequest
{
    public int SchemaVersion { get; init; }
    public int ChoiceId { get; init; }
    public EntityChoiceType ChoiceType { get; init; }
    public int PlayerId { get; init; }
    public int? SourceEntityId { get; init; }
    public int CountMin { get; init; }
    public int CountMax { get; init; }
    public int[] Entities { get; init; } = [];
    public int[] UnchoosableEntities { get; init; } = [];
    public int[] Clicks { get; init; } = [];
    public bool Confirm { get; init; }
    public int? SubmitChoiceId { get; init; }
}
