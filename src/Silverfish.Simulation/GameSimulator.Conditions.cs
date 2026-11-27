using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private static bool IsValidCondition(EffectCondition? condition, TargetRequirement target,
        bool hasCharacterSubject, bool hasInPlaySource, bool canUseCombo = false) => condition switch
    {
        CharacterCondition character => IsValidCharacterComparison(character.Metric, character.Comparison, character.Value) &&
            IsValidConditionCharacter(character.Target, target, hasCharacterSubject, hasInPlaySource),
        CharacterStateCondition state => Enum.IsDefined(state.State) &&
            IsValidConditionCharacter(state.Target, target, hasCharacterSubject, hasInPlaySource),
        ZoneCountCondition count => count.Value >= 0 && Enum.IsDefined(count.Player) && Enum.IsDefined(count.Zone) &&
            Enum.IsDefined(count.Comparison) && IsValidCardFilter(count.CardFilter) &&
            (count.EntityKind is not EntityKind kind || (Enum.IsDefined(kind) && kind != EntityKind.Unknown)),
        ComboCondition => canUseCombo,
        WeaponEquippedCondition weapon => Enum.IsDefined(weapon.Player),
        CharacterMatchCondition match => Enum.IsDefined(match.Relation) && IsValidCardFilter(match.CardFilter) &&
            IsValidConditionCharacter(match.Target, target, hasCharacterSubject, hasInPlaySource),
        MinionDiedThisGameCondition died => !string.IsNullOrWhiteSpace(died.CardId),
        _ => false
    };

    private static bool IsValidConditionCharacter(ConditionCharacter character, TargetRequirement target,
        bool hasCharacterSubject, bool hasInPlaySource) => character switch
    {
        ConditionCharacter.Source => hasInPlaySource,
        ConditionCharacter.Selected => target != TargetRequirement.None,
        ConditionCharacter.TriggerSubject => hasCharacterSubject,
        ConditionCharacter.FriendlyHero or ConditionCharacter.EnemyHero => true,
        _ => false
    };

    private static bool IsValidCharacterComparison(CharacterMetric metric, NumberComparison comparison, int value) =>
        value >= 0 && Enum.IsDefined(metric) && Enum.IsDefined(comparison);

    private bool MatchesCharacterComparison(GameState state, GameEntity entity, CharacterMetric metric,
        NumberComparison comparison, int expected)
    {
        var character = entity.Character!;
        var observed = metric switch
        {
            CharacterMetric.Health => character.Health,
            CharacterMetric.Damage => character.Damage,
            CharacterMetric.Attack => EffectiveAttack(state, entity),
            CharacterMetric.Armor => character.Armor,
            _ => throw new InvalidOperationException("无法识别条件引用的角色属性。")
        };
        return TestComparison(observed, comparison, expected);
    }

    private static bool TestComparison(int observed, NumberComparison comparison, int expected) => comparison switch
    {
        NumberComparison.Equal => observed == expected,
        NumberComparison.AtLeast => observed >= expected,
        NumberComparison.AtMost => observed <= expected,
        _ => throw new InvalidOperationException("无法识别条件的比较方式。")
    };

    private sealed partial class Resolution
    {
        private bool MeetsCondition(EffectContext context, EffectCondition condition) => condition switch
        {
            CharacterCondition character => MeetsCharacterCondition(context, character),
            CharacterStateCondition state => MeetsCharacterStateCondition(context, state),
            ZoneCountCondition count => TestComparison(CountEntities(context, count), count.Comparison, count.Value),
            ComboCondition => context.CardsPlayedBefore > 0,
            WeaponEquippedCondition weapon => _state.Players[weapon.Player == ConditionPlayer.Friendly
                ? context.Controller : _state.OpponentOf(context.Controller)].EquippedWeapon is EntityId equipped &&
                _state.LocationOf(equipped).Zone == GameZone.Play &&
                _state.Entities[equipped].Weapon is { Durability: > 0 },
            CharacterMatchCondition match => MeetsCharacterMatchCondition(context, match),
            MinionDiedThisGameCondition died => _state.MinionDeaths.Any(entry => entry.CardId == died.CardId),
            _ => throw new InvalidOperationException("无法识别效果条件。")
        };

        private EntityId? ResolveConditionCharacter(EffectContext context, ConditionCharacter target) => target switch
        {
            ConditionCharacter.Source => context.Source,
            ConditionCharacter.Selected => context.Selected,
            ConditionCharacter.TriggerSubject => context.Subject,
            ConditionCharacter.FriendlyHero => _state.Players[context.Controller].Hero,
            ConditionCharacter.EnemyHero => _state.Players[_state.OpponentOf(context.Controller)].Hero,
            _ => throw new InvalidOperationException("无法识别条件引用的角色。")
        };

        private bool MeetsCharacterCondition(EffectContext context, CharacterCondition condition)
        {
            var id = ResolveConditionCharacter(context, condition.Target);
            if (!IsLivingCharacter(id))
                return false;

            return _rules.MatchesCharacterComparison(_state, _state.Entities[id!.Value],
                condition.Metric, condition.Comparison, condition.Value);
        }

        private bool MeetsCharacterMatchCondition(EffectContext context, CharacterMatchCondition condition)
        {
            var id = ResolveConditionCharacter(context, condition.Target);
            if (!IsLivingCharacter(id))
                return false;

            var entity = _state.Entities[id!.Value];
            if ((condition.Relation == TriggerRelation.Friendly && entity.Controller != context.Controller) ||
                (condition.Relation == TriggerRelation.Opponent && entity.Controller == context.Controller))
                return false;
            return condition.CardFilter is null || MatchesCardFilter(_rules.Card(entity).Definition, condition.CardFilter);
        }

        private bool MeetsCharacterStateCondition(EffectContext context, CharacterStateCondition condition)
        {
            var id = ResolveConditionCharacter(context, condition.Target);
            return condition.State switch
            {
                CharacterDisposition.Alive => IsLivingCharacter(id),
                CharacterDisposition.Dead => IsDeadOrDyingCharacter(id),
                CharacterDisposition.Frozen => IsLivingCharacter(id) && _state.Entities[id!.Value].Character is { Frozen: true },
                _ => throw new InvalidOperationException("无法识别角色状态条件。")
            };
        }

        private bool IsDeadOrDyingCharacter(EntityId? id) =>
            id is EntityId entity && _state.Entities.TryGetValue(entity, out var state) &&
            (_state.LocationOf(entity).Zone == GameZone.Graveyard ||
                (_state.LocationOf(entity).Zone == GameZone.Play &&
                    (state.Character is { Health: <= 0 } || _pendingDestruction.Contains(entity))));

        private int CountEntities(EffectContext context, ZoneCountCondition condition)
        {
            var player = condition.Player == ConditionPlayer.Friendly ? context.Controller : _state.OpponentOf(context.Controller);
            var entities = _state.Players[player].In(condition.Zone);
            if (condition.EntityKind is null && condition.CardFilter is null)
                return entities.Length;

            return entities.Count(id =>
            {
                var definition = _rules.Card(_state.Entities[id]).Definition;
                return (condition.EntityKind is not EntityKind kind || definition.Kind == kind) &&
                    (condition.CardFilter is not { } cards || MatchesCardFilter(definition, cards));
            });
        }
    }
}
