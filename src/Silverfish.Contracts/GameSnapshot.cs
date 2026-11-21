namespace Silverfish.Contracts;

/// <summary>与传输方式无关的可观察对局快照。版本 1 提供候选动作决策所需的基础字段。</summary>
public sealed record GameSnapshot
{
    public int SchemaVersion { get; init; } = 1;
    public string MatchId { get; init; } = string.Empty;
    public long Sequence { get; init; }
    public GamePhase Phase { get; init; }
    public bool IsOurTurn { get; init; }
    public int AvailableMana { get; init; }
    // 这些字段在快照中可选，经典 BoardTester 适配器需要它们。
    public int? MaximumMana { get; init; }
    public int? EnemyHandCount { get; init; }
    public int? EnemyDeckCount { get; init; }
    public int? OurSecretCount { get; init; }
    public int? EnemySecretCount { get; init; }
    public HeroSnapshot OurHero { get; init; } = new();
    public HeroSnapshot EnemyHero { get; init; } = new();
    public IReadOnlyList<MinionSnapshot> OurBoard { get; init; } = Array.Empty<MinionSnapshot>();
    public IReadOnlyList<MinionSnapshot> EnemyBoard { get; init; } = Array.Empty<MinionSnapshot>();
    public IReadOnlyList<HandCardSnapshot> OurHand { get; init; } = Array.Empty<HandCardSnapshot>();
    public IReadOnlyList<CandidateAction> LegalActions { get; init; } = Array.Empty<CandidateAction>();
}

public enum GamePhase
{
    Unknown,
    Mulligan,
    Main,
    Choice,
    GameOver
}

public sealed record HeroSnapshot
{
    public int EntityId { get; init; }
    public int Health { get; init; }
    public int Armor { get; init; }
    public int Attack { get; init; }
    public string ClassNameZh { get; init; } = string.Empty;
}

public sealed record MinionSnapshot
{
    public int EntityId { get; init; }
    public string CardId { get; init; } = string.Empty;
    // 供经典 BoardTester 的中文文本格式使用。
    public string CardNameZh { get; init; } = string.Empty;
    public int Attack { get; init; }
    public int Health { get; init; }
    public bool CanAttack { get; init; }
    public bool HasTaunt { get; init; }
    public bool HasDivineShield { get; init; }
}

public sealed record HandCardSnapshot
{
    public int EntityId { get; init; }
    public string CardId { get; init; } = string.Empty;
    public int Cost { get; init; }
}

public enum GameActionKind
{
    Unknown = -1,
    EndTurn,
    PlayCard,
    Attack,
    HeroPower,
    ChooseOption,
    ActivateLocation
}

/// <summary>由调用方提供的候选动作及其标识。</summary>
public sealed record CandidateAction
{
    public string Id { get; init; } = string.Empty;
    public GameActionKind Kind { get; init; } = GameActionKind.Unknown;
    public int? SourceEntityId { get; init; }
    public int? TargetEntityId { get; init; }
    public int? Position { get; init; }
    public string? Choice { get; init; }
    public int ManaCost { get; init; }
    public string Description { get; init; } = string.Empty;
}

public sealed record ScoredAction
{
    public required CandidateAction Action { get; init; }
    public double Score { get; init; }
}

public sealed record BotDecision
{
    public string MatchId { get; init; } = string.Empty;
    public long SnapshotSequence { get; init; }
    public string Engine { get; init; } = string.Empty;
    public CandidateAction? ChosenAction { get; init; }
    public double? Score { get; init; }
    public string Reason { get; init; } = string.Empty;
    public IReadOnlyList<ScoredAction> Alternatives { get; init; } = Array.Empty<ScoredAction>();
}
