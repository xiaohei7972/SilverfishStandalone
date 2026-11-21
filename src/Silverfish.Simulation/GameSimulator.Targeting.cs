using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private static bool IsValidTarget(TargetRequirement requirement, TargetFilter? filter) =>
        Enum.IsDefined(requirement) && (filter is null || (requirement != TargetRequirement.None &&
            IsValidCardFilter(filter.CardFilter) && !filter.Comparisons.IsDefault && filter.Comparisons.All(comparison => comparison is not null &&
                IsValidCharacterComparison(comparison.Metric, comparison.Comparison, comparison.Value))));

    private static bool TargetsOnlyMinions(TargetRequirement requirement) =>
        requirement is TargetRequirement.Minion or TargetRequirement.FriendlyMinion or TargetRequirement.EnemyMinion;

    private bool MatchesTargetFilter(GameState state, GameEntity entity, TargetFilter filter) =>
        (filter.Frozen is not bool frozen || entity.Character!.Frozen == frozen) &&
        (filter.CardFilter is not { } cards || MatchesCardFilter(Card(entity).Definition, cards)) &&
        filter.Comparisons.All(comparison => MatchesCharacterComparison(
            state, entity, comparison.Metric, comparison.Comparison, comparison.Value));
}
