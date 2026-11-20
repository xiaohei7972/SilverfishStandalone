using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using Silverfish.Contracts;
using Silverfish.Core;
using Silverfish.Domain;
using Silverfish.Simulation;

Console.OutputEncoding = Encoding.UTF8;

var simulation = (args.Length == 2 || (args.Length == 4 && args[2] == "--card-data")) &&
    args[0] is "simulate" or "plan" or "decide" or "checkpoint";
var plan = simulation && args[0] == "plan";
var decide = simulation && args[0] == "decide";
var checkpoint = simulation && args[0] == "checkpoint";
var import = args.Length is 3 or 4 && args[0] == "import-card-data" &&
    (args.Length == 3 || args[3] == "--starter");
if ((!simulation && !import && args.Length != 1) || args[0] is "--help" or "-h")
{
    Console.Error.WriteLine("用法  Silverfish.Cli <game-snapshot.json>");
    Console.Error.WriteLine("       Silverfish.Cli simulate <scenario.json> [--card-data <catalog.json>]");
    Console.Error.WriteLine("       Silverfish.Cli plan <scenario.json> [--card-data <catalog.json>]");
    Console.Error.WriteLine("       Silverfish.Cli decide <decision.json> [--card-data <catalog.json>]");
    Console.Error.WriteLine("       Silverfish.Cli checkpoint <scenario.json> [--card-data <catalog.json>]");
    Console.Error.WriteLine("       Silverfish.Cli import-card-data <CardDefs.xml> <catalog.json> [--starter]");
    return args.Length == 1 ? 0 : 2;
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

    await using var stream = File.OpenRead(simulation ? args[1] : args[0]);
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
            CardDataBuild = cardData?.SourceBuild,
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
    var operation = import ? "导入卡牌数据" : decide ? "模拟决策" : plan ? "搜索回合" :
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
            data.WriteJson(destination, indented: starterOnly);
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

        if (cardData is null)
            throw new InvalidDataException("使用 cardRules 时，请通过 --card-data <catalog.json> 指定卡牌目录。");
        return builtIn.Concat(Cards).Concat(CardRules.Select(rules =>
        {
            if (string.IsNullOrWhiteSpace(rules?.CardId))
                throw new InvalidDataException("每条 cardRules 规则必须提供 cardId。");
            return SimulationCard.FromMetadata(cardData.GetCard(rules.CardId), rules);
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
