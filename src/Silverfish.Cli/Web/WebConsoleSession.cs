using Silverfish.Contracts;
using Silverfish.Core;
using Silverfish.Domain;

namespace Silverfish.Cli.Web;

internal sealed record ConsoleDecision(string Reason, BotDecision? Baseline = null, TurnPlan? Plan = null);
internal sealed record QueuedCommand(string Id, string Kind, int? SourceEntityId, int? TargetEntityId, int? Position);
internal sealed record CommandAck(string Id, bool Ok, string Message);
internal sealed record ConsoleView(Guid Version, string? Mode, string? MatchId, string? Sequence, ConsoleBoard? Board,
    IReadOnlyList<CandidateAction>? ObservedActions, IReadOnlyList<SimulationAction>? LegalActions,
    IReadOnlyList<ConsoleEvent>? Events, bool Automatic, bool Busy, ConsoleDecision? Decision, string? DecisionError,
    bool AutoPilot, CommandAck? LastCommandAck, QueuedCommand? LastCommandAction);

internal sealed class ConsoleConflictException(string message) : InvalidOperationException(message);

internal sealed class WebConsoleSession(CardNames names, TimeProvider? timeProvider = null)
{
    private static readonly TimeSpan s_autoPilotRetryDelay = TimeSpan.FromSeconds(4);
    private readonly object _gate = new();
    private readonly BaselineStrategyEngine _baseline = new();
    private readonly TimeProvider _timeProvider = timeProvider ?? TimeProvider.System;
    private readonly Queue<(QueuedCommand Command, bool AutoPilot)> _commandQueue = new();
    private readonly QueuedCommand?[] _recentCommands = new QueuedCommand?[32];
    private int _nextRecentCommand;
    private CommandAck? _lastCommandAck;
    private QueuedCommand? _lastCommandAction;
    private ConsolePosition? _position;
    private Guid _version = Guid.NewGuid();
    private Guid _viewVersion = Guid.NewGuid();
    private ConsoleDecision? _decision;
    private string? _decisionError;
    private bool _automatic;
    private bool _autoPilot;
    private int _autoPilotModeVersion;
    private Guid? _lastAutoPilotVersion;
    private long _lastAutoPilotTimestamp;
    private bool _busy;
    private int _modeVersion;

    internal ConsoleView Read() => Read(out _);

    internal ConsoleView Read(out Guid viewVersion)
    {
        lock (_gate)
        {
            var position = _position;
            viewVersion = _viewVersion;
            return new(_version, position is null ? null : position.Game is null ? "snapshot" : "simulation",
                position?.MatchId, position?.SequenceText, position?.Board, position?.Snapshot?.LegalActions,
                position?.Game is null ? null : position.Actions, position?.Events, _automatic, _busy, _decision, _decisionError,
                _autoPilot, _lastCommandAck, _lastCommandAction);
        }
    }

    internal void Import(ConsolePosition position, bool sample = false)
    {
        lock (_gate)
        {
            Replace(position);
        }
    }

    internal void Reset()
    {
        lock (_gate)
        {
            _automatic = false;
            _modeVersion++;
            SetAutoPilotLocked(false);
            Replace(null);
        }
    }

    internal void SetAutomatic(bool enabled)
    {
        lock (_gate)
        {
            if (_automatic == enabled)
                return;
            _automatic = enabled;
            _modeVersion++;
            if (enabled)
                _decisionError = null;
            _viewVersion = Guid.NewGuid();
        }
    }

    internal void SetAutoPilot(bool enabled)
    {
        lock (_gate)
            SetAutoPilotLocked(enabled);
    }

    private void SetAutoPilotLocked(bool enabled)
    {
        if (!enabled)
        {
            _commandQueue.Clear();
            _lastAutoPilotVersion = null;
            _lastAutoPilotTimestamp = 0;
        }
        if (_autoPilot == enabled)
            return;
        _autoPilot = enabled;
        _autoPilotModeVersion++;
        _viewVersion = Guid.NewGuid();
    }

    internal QueuedCommand EnqueueCommand(string kind, int? sourceEntityId, int? targetEntityId, int? position)
    {
        var command = CreateCommand(kind, sourceEntityId, targetEntityId, position);
        lock (_gate)
            _commandQueue.Enqueue((command, AutoPilot: false));
        return command;
    }

    internal QueuedCommand? DequeueCommand()
    {
        lock (_gate)
        {
            if (!_commandQueue.TryDequeue(out var next))
                return null;
            _recentCommands[_nextRecentCommand] = next.Command;
            _nextRecentCommand = (_nextRecentCommand + 1) % _recentCommands.Length;
            return next.Command;
        }
    }

    internal CommandAck? ReadLastAck()
    {
        lock (_gate)
            return _lastCommandAck;
    }

    internal void RecordAck(CommandAck ack)
    {
        lock (_gate)
        {
            _lastCommandAck = ack;
            _lastCommandAction = Array.Find(_recentCommands, command => command?.Id == ack.Id);
            _viewVersion = Guid.NewGuid();
        }
    }

    internal async Task DispatchAutoPilotAsync(CancellationToken cancellationToken)
    {
        ConsolePosition position;
        GameSnapshot snapshot;
        Guid version;
        int modeVersion;
        lock (_gate)
        {
            if (_position is not { Snapshot: { } observed, Board.CanAct: true } current || !CanDispatchAutoPilot())
                return;
            position = current;
            snapshot = observed;
            version = _version;
            modeVersion = _autoPilotModeVersion;
        }

        QueuedCommand? command = null;
        if (position.Board.PendingChoice is { } choice)
        {
            if (choice.Cards.Length > 0)
                command = DecideChoice(choice);
        }
        else if (position.Board.IsOurTurn && snapshot.LegalActions.Count > 0)
        {
            var decision = await _baseline.DecideAsync(snapshot, cancellationToken);
            var action = decision.ChosenAction;
            if (action is null || CommandKind(action) is null)
                action = snapshot.LegalActions.FirstOrDefault(candidate => candidate.Kind != GameActionKind.EndTurn && CommandKind(candidate) is not null)
                    ?? snapshot.LegalActions.FirstOrDefault(candidate => candidate.Kind == GameActionKind.EndTurn);
            if (action is not null && CommandKind(action) is { } kind)
                command = CreateCommand(kind, action.SourceEntityId, action.TargetEntityId, action.Position);
        }

        if (command is null)
            return;
        cancellationToken.ThrowIfCancellationRequested();
        lock (_gate)
        {
            if (version != _version || modeVersion != _autoPilotModeVersion || !CanDispatchAutoPilot())
                return;
            _commandQueue.Enqueue((command, AutoPilot: true));
            _lastAutoPilotVersion = version;
            _lastAutoPilotTimestamp = _timeProvider.GetTimestamp();
        }
    }

    private bool CanDispatchAutoPilot() => _autoPilot && _commandQueue.Count == 0 &&
        (_lastAutoPilotVersion != _version || _timeProvider.GetElapsedTime(_lastAutoPilotTimestamp) > s_autoPilotRetryDelay);

    private static QueuedCommand? DecideChoice(ConsolePendingChoice choice)
    {
        var chosen = choice.Cards.Count(card => card.Chosen);
        if (choice.HasConfirmButton && chosen >= Math.Max(choice.CountMin, 1))
            return CreateCommand("confirm_choice");
        var best = choice.Cards.Where(card => !card.Unchoosable && !card.Chosen && card.EntityId > 0)
            .OrderByDescending(card => string.Equals(card.CardType, "MINION", StringComparison.OrdinalIgnoreCase)
                ? (long)(card.Attack ?? 0) + (card.Health ?? 0) : 0)
            .ThenByDescending(card => Math.Clamp(card.Cost, 0, 10))
            .FirstOrDefault();
        return best is null ? null : CreateCommand("choose_entity", best.EntityId);
    }

    private static string? CommandKind(CandidateAction action) => action.Kind switch
    {
        GameActionKind.PlayCard when action.SourceEntityId is > 0 => "playCard",
        GameActionKind.Attack when action.SourceEntityId is > 0 && action.TargetEntityId is > 0 => "attack",
        GameActionKind.HeroPower when action.SourceEntityId is > 0 => "heroPower",
        GameActionKind.ActivateLocation when action.SourceEntityId is > 0 => "activateLocation",
        GameActionKind.EndTurn => "endTurn",
        _ => null
    };

    private static QueuedCommand CreateCommand(string kind, int? sourceEntityId = null, int? targetEntityId = null, int? position = null) =>
        new(Guid.NewGuid().ToString("N")[..8], kind, sourceEntityId, targetEntityId, position);

    internal async Task DecideAsync(Guid? expectedVersion, CancellationToken cancellationToken)
    {
        ConsolePosition position;
        Guid version;
        int modeVersion;
        var automatic = expectedVersion is null;
        lock (_gate)
        {
            if (expectedVersion is Guid expected)
                CheckVersion(expected);
            else if (!_automatic || _decision is not null || _decisionError is not null || _busy || _position is null)
                return;
            position = _position ?? throw new ConsoleConflictException("请先载入对局。");
            if (_busy)
                throw new ConsoleConflictException("已有一次决策正在计算，请稍后重试。");
            version = _version;
            modeVersion = _modeVersion;
            _busy = true;
            _viewVersion = Guid.NewGuid();
        }

        try
        {
            var decision = position.Snapshot is { } snapshot
                ? new ConsoleDecision("基线只给输入中已有的候选动作评分。", Baseline: await _baseline.DecideAsync(snapshot, cancellationToken))
                : await Task.Run(() => Plan(position), cancellationToken);
            lock (_gate)
            {
                if (automatic && (version != _version || modeVersion != _modeVersion))
                    return;
                CheckVersion(version);
                _decision = decision;
                _decisionError = null;
                _viewVersion = Guid.NewGuid();
            }
        }
        catch (Exception error) when (error is InvalidOperationException or ArgumentException or InvalidDataException or OverflowException)
        {
            lock (_gate)
            {
                if (version == _version && (!automatic || modeVersion == _modeVersion))
                {
                    _decisionError = "本次决策无法完成；请检查局面，或手动重试。";
                    _viewVersion = Guid.NewGuid();
                }
            }
            if (!automatic)
                throw;
        }
        finally
        {
            lock (_gate)
            {
                _busy = false;
                _viewVersion = Guid.NewGuid();
            }
        }
    }

    private static ConsoleDecision Plan(ConsolePosition position) => position.Actions.IsEmpty
        ? new("当前没有可规划的合法动作。")
        : new("当前行动方视角；胜率表示搜索范围内结算出胜局的概率。",
            Plan: new SimulationTurnPlanner(position.Simulator!, SimulationMaterialEvaluator.Evaluate)
                .Plan(position.Game!, depth: 4, beamWidth: 12, transitionLimit: 192));

    internal void Apply(Guid version, int index)
    {
        lock (_gate)
        {
            CheckVersion(version);
            if (_position is not { Simulator: { } simulator, Game: { } game } position)
                throw new ConsoleConflictException("只能在离线模拟局面中执行动作。");
            if (index < 0 || index >= position.Actions.Length)
                throw new ArgumentException("所选动作不在当前合法动作列表中。");
            var result = simulator.Apply(game, position.Actions[index]);
            var sequence = checked(position.Sequence + 1);
            Replace(ConsolePosition.Simulation(position.MatchId, sequence, simulator, result.State, position.Perspective,
                ConsolePosition.AppendEvents(position.Events, sequence, result.Events), names));
        }
    }

    private void CheckVersion(Guid version)
    {
        if (_version != version)
            throw new ConsoleConflictException("局面已更新，请按最新快照重新选择操作。");
    }

    private void Replace(ConsolePosition? position)
    {
        if (_commandQueue.TryPeek(out var next) && next.AutoPilot)
            _commandQueue.Dequeue();
        _position = position;
        _version = Guid.NewGuid();
        _viewVersion = Guid.NewGuid();
        _decision = null;
        _decisionError = null;
    }
}
