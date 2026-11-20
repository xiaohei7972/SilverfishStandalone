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

    private IEnumerable<EntityId?> PlayTargets(GameState state, PlayerId controller, TargetRequirement requirement, TargetFilter? filter)
    {
        if (requirement == TargetRequirement.None)
        {
            yield return null;
            yield break;
        }

        var minionsOnly = TargetsOnlyMinions(requirement);
        foreach (var player in state.Players.Values.OrderBy(player => player.Id.Value))
        {
            var friendly = player.Id == controller;
            if ((!friendly && requirement is TargetRequirement.FriendlyCharacter or TargetRequirement.FriendlyMinion) ||
                (friendly && requirement is TargetRequirement.EnemyCharacter or TargetRequirement.EnemyMinion))
                continue;

            foreach (var id in player.In(GameZone.Play))
            {
                var entity = state.Entities[id];
                if (entity.Character is { Health: > 0 } &&
                    (!minionsOnly || Card(entity).Definition.Kind == EntityKind.Minion) &&
                    (filter is null || MatchesTargetFilter(state, entity, filter)))
                    yield return id;
            }
        }
    }

    private bool MatchesTargetFilter(GameState state, GameEntity entity, TargetFilter filter) =>
        (filter.Frozen is not bool frozen || entity.Character!.Frozen == frozen) &&
        (filter.CardFilter is not { } cards || MatchesCardFilter(Card(entity).Definition, cards)) &&
        filter.Comparisons.All(comparison => MatchesCharacterComparison(
            state, entity, comparison.Metric, comparison.Comparison, comparison.Value));
}
