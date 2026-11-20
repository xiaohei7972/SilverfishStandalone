using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private static bool IsValidCondition(EffectCondition? condition, TargetRequirement target,
        bool hasCharacterSubject, bool hasInPlaySource) => condition switch
    {
        CharacterCondition character => character.Value >= 0 && Enum.IsDefined(character.Metric) &&
            Enum.IsDefined(character.Comparison) && (character.Target switch
            {
                ConditionCharacter.Source => hasInPlaySource,
                ConditionCharacter.Selected => target != TargetRequirement.None,
                ConditionCharacter.TriggerSubject => hasCharacterSubject,
                ConditionCharacter.FriendlyHero or ConditionCharacter.EnemyHero => true,
                _ => false
            }),
        ZoneCountCondition count => count.Value >= 0 && Enum.IsDefined(count.Player) && Enum.IsDefined(count.Zone) &&
            Enum.IsDefined(count.Comparison) && (count.EntityKind is not EntityKind kind || (Enum.IsDefined(kind) && kind != EntityKind.Unknown)),
        _ => false
    };

    private sealed partial class Resolution
    {
        private bool MeetsCondition(EffectContext context, EffectCondition condition) => condition switch
        {
            CharacterCondition character => MeetsCharacterCondition(context, character),
            ZoneCountCondition count => TestComparison(CountEntities(context, count), count.Comparison, count.Value),
            _ => throw new InvalidOperationException("无法识别效果条件。")
        };

        private bool MeetsCharacterCondition(EffectContext context, CharacterCondition condition)
        {
            EntityId? id = condition.Target switch
            {
                ConditionCharacter.Source => context.Source,
                ConditionCharacter.Selected => context.Selected,
                ConditionCharacter.TriggerSubject => context.Subject,
                ConditionCharacter.FriendlyHero => _state.Players[context.Controller].Hero,
                ConditionCharacter.EnemyHero => _state.Players[_state.OpponentOf(context.Controller)].Hero,
                _ => throw new InvalidOperationException("无法识别条件引用的角色。")
            };
            if (!IsLivingCharacter(id))
                return false;

            var entity = _state.Entities[id!.Value];
            var character = entity.Character!;
            var metric = condition.Metric switch
            {
                CharacterMetric.Health => character.Health,
                CharacterMetric.Damage => character.Damage,
                CharacterMetric.Attack => _rules.EffectiveAttack(_state, entity),
                CharacterMetric.Armor => character.Armor,
                _ => throw new InvalidOperationException("无法识别条件引用的角色属性。")
            };
            return TestComparison(metric, condition.Comparison, condition.Value);
        }

        private int CountEntities(EffectContext context, ZoneCountCondition condition)
        {
            var player = condition.Player == ConditionPlayer.Friendly ? context.Controller : _state.OpponentOf(context.Controller);
            var entities = _state.Players[player].In(condition.Zone);
            return condition.EntityKind is EntityKind kind
                ? entities.Count(id => _rules.Card(_state.Entities[id]).Definition.Kind == kind)
                : entities.Length;
        }

        private static bool TestComparison(int observed, NumberComparison comparison, int expected) => comparison switch
        {
            NumberComparison.Equal => observed == expected,
            NumberComparison.AtLeast => observed >= expected,
            NumberComparison.AtMost => observed <= expected,
            _ => throw new InvalidOperationException("无法识别条件的比较方式。")
        };
    }
}
