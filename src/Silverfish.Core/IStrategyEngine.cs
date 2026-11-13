using Silverfish.Contracts;

namespace Silverfish.Core;

/// <summary>
/// Strategy implementations consume complete snapshots and never acquire game
/// process handles, use game assemblies, or execute game actions.
/// </summary>
public interface IStrategyEngine
{
    string Name { get; }
    Task<BotDecision> DecideAsync(GameSnapshot snapshot, CancellationToken cancellationToken = default);
}
