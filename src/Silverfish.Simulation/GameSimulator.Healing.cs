using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed partial class Resolution
    {
        /// <summary>
        /// Restore Health for one character or an ordered group. Apply area healing
        /// to all recipients before dispatching resulting Overheal triggers.
        /// </summary>
        private void HealCharacters(Stack<Work> work, EffectContext context, HealEffect effect)
        {
            var targetIds = effect.Target switch
            {
                HealTarget.Selected => new[] { context.Selected!.Value },
                HealTarget.Source => new[] { context.Source },
                HealTarget.TriggerSubject => new[] { context.Subject!.Value },
                HealTarget.FriendlyHero => new[] { _state.Players[context.Controller].Hero },
                HealTarget.EnemyHero => new[] { _state.Players[_state.OpponentOf(context.Controller)].Hero },
                HealTarget.FriendlyMinions => _rules.BoardMinions(_state, context.Controller).ToArray(),
                HealTarget.AllMinions => _state.Players.Keys
                    .SelectMany(player => _rules.BoardMinions(_state, player))
                    .OrderBy(id => _state.Entities[id].PlayOrder).ToArray(),
                HealTarget.AllFriendlyCharacters => _state.Players[context.Controller].In(GameZone.Play)
                    .Where(id => _state.Entities[id].Character is not null)
                    .OrderBy(id => _state.Entities[id].PlayOrder).ToArray(),
                HealTarget.AllCharacters => _state.Players.Keys
                    .SelectMany(player => _state.Players[player].In(GameZone.Play))
                    .Where(id => _state.Entities[id].Character is not null)
                    .OrderBy(id => _state.Entities[id].PlayOrder).ToArray(),
                HealTarget.RandomDamagedFriendlyCharacter => PickRandomDamagedFriendlyCharacter(context),
                _ => throw new InvalidOperationException("无法识别治疗目标。")
            };

            var outcomes = targetIds.Select(id => HealCharacter(context, id, effect.Amount)).ToArray();
            for (var i = outcomes.Length - 1; i >= 0; i--)
            {
                PushOccurrence(work, outcomes[i].Overhealed);
                PushOccurrence(work, outcomes[i].Healed);
            }
        }

        /// <summary>
        /// Healing always caps at current max Health; an Overheal minion's own
        /// trigger activates when the requested heal exceeds missing Health,
        /// including a positive heal applied to an already-full minion.
        /// </summary>
        private (Occurrence? Healed, Occurrence? Overhealed) HealCharacter(EffectContext context, EntityId target, int amount)
        {
            if (amount <= 0 || !_state.Entities.TryGetValue(target, out var entity) ||
                _state.LocationOf(target).Zone != GameZone.Play ||
                entity.Character is not { } character || _pendingDestruction.Contains(target))
                return (null, null);

            if (HasHealingDealsDamage(context.Controller))
                return (Damage(context.Source, target, amount, allowLifesteal: false), null);

            // Death checking is deferred until the resolution phase ends. A
            // character at zero or negative Health may still be saved by healing.
            var restored = Math.Min(amount, character.Damage);
            var excess = amount - restored;
            Occurrence? healedEvent = null;
            if (restored > 0)
            {
                _state = _state.UpdateCharacter(target, character with { Damage = character.Damage - restored });
                _state = _rules.RefreshAuras(_state);
                _events.Add(new(SimulationEventKind.Healed, entity.Controller, context.Source, target, restored));
                healedEvent = new(TriggerTiming.Healed, entity.Controller, target, Amount: restored);
            }

            if (excess <= 0 || _rules.Card(entity).Definition.Kind != EntityKind.Minion ||
                !_rules.HasKeyword(entity, CardKeywords.Overheal))
                return (healedEvent, null);

            _events.Add(new(SimulationEventKind.Overhealed, entity.Controller, context.Source, target, excess));
            // The event belongs to the healed minion; the healer may be its opponent.
            return (healedEvent, new(TriggerTiming.Overheal, entity.Controller, target, Amount: excess));
        }

        private EntityId[] PickRandomDamagedFriendlyCharacter(EffectContext context)
        {
            var candidates = _state.Players[context.Controller].In(GameZone.Play)
                .Where(id => IsLivingCharacter(id) && _state.Entities[id].Character is { Damage: > 0 })
                .OrderBy(id => _state.Entities[id].PlayOrder)
                .ToArray();
            return candidates.Length == 0 ? [] : [candidates[ChooseIndex(candidates.Length)]];
        }
    }
}
