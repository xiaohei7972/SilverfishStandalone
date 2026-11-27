using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed partial class Resolution
    {
        private readonly HashSet<ListenerId> _activeTriggers = [];

        private readonly record struct EffectContext(EntityId Source, PlayerId Controller,
            EntityId? Selected = null, EntityId? Subject = null, int? DeathPosition = null,
            int CardsPlayedBefore = 0);

        private readonly record struct DamageSource(EntityId Id, PlayerId Controller);

        private readonly record struct Occurrence(TriggerTiming Timing, PlayerId Player, EntityId? Subject = null,
            DamageSource? Source = null, int? Amount = null, EntityId? Defender = null, bool Poisoned = false,
            TriggerTiming? LethalBonus = null, DamageSource? AttackingWeapon = null);

        private readonly record struct ListenerId(EntityId Entity, int Index);

        private sealed record Listener(ListenerId Id, string CardId, PlayerId Controller, long PlayOrder, GameZone Zone, CardTrigger Trigger);

        private abstract record Work;
        private sealed record ApplyEffect(EffectContext Context, CardEffect Effect) : Work;
        private sealed record DrawCards(PlayerId Player, int Remaining, CardFilter? Filter = null) : Work;
        private sealed record RecruitCards(PlayerId Player, int Remaining, CardFilter? Filter) : Work;
        private sealed record CastDrawnSpell(PlayerId Player, EntityId Source) : Work;
        private sealed record FinishDrawnSpell(PlayerId Player, EntityId Source) : Work;
        private sealed record DealDrawnCostDamage(EffectContext Context, DamageTarget Target, int PrintedCost) : Work;
        private sealed record ResolveDiscardedCard(PlayerId Player, EntityId Card, ImmutableArray<CardEffect> Effects) : Work;
        // Damage events can be dispatched after an entire damage batch has been applied.
        // Freeze matched observers at enqueue time, before an earlier event's
        // triggers can summon new listeners or change a later target's identity.
        // Other event families retain their existing deferred-dispatch semantics.
        private sealed record Dispatch(Occurrence Occurrence,
            ImmutableArray<Activate>? DamageTriggers = null,
            ImmutableArray<Activate>? LethalTriggers = null,
            ImmutableArray<Activate>? WeaponLethalTriggers = null) : Work;
        private sealed record DestroyPoisoned(EntityId Target) : Work;
        private sealed record Activate(EffectContext Context, long PlayOrder, ImmutableArray<CardEffect> Effects,
            Listener? Listener = null, EntityId? LivingEntity = null, int? Amount = null) : Work;
        private sealed record Complete(ListenerId Listener) : Work;

        private ImmutableArray<Listener> CaptureListeners()
        {
            var listeners = ImmutableArray.CreateBuilder<Listener>();
            var sources = _state.Players.Values.SelectMany(player => player.In(GameZone.Play).Concat(player.In(GameZone.Secret)))
                .Select(id => _state.Entities[id]).OrderBy(entity => entity.PlayOrder);
            foreach (var entity in sources)
            {
                if (entity.Silenced)
                    continue;
                var zone = _state.LocationOf(entity.Id).Zone;
                var triggers = _rules.Card(entity).Triggers;
                for (var index = 0; index < triggers.Length; index++)
                    listeners.Add(new(new(entity.Id, index), entity.CardId, entity.Controller, entity.PlayOrder!.Value, zone, triggers[index]));
            }
            return listeners.ToImmutable();
        }

        private IEnumerable<Activate> Queue(Occurrence occurrence, ImmutableArray<Listener> listeners)
        {
            foreach (var listener in listeners)
            {
                var trigger = listener.Trigger;
                var matches = trigger.Timing == occurrence.Timing ||
                    (occurrence.Timing == TriggerTiming.AfterMinionPlayed && trigger.Timing == TriggerTiming.AfterMinionSummoned);
                if (!matches ||
                    (occurrence.Timing == TriggerTiming.Overheal && listener.Id.Entity != occurrence.Subject) ||
                    (occurrence.Timing is TriggerTiming.Overkill or TriggerTiming.HonorableKill &&
                        listener.Id.Entity != occurrence.Source?.Id) ||
                    (listener.Zone == GameZone.Secret && listener.Controller == _state.ActivePlayer) ||
                    (occurrence.Timing is TriggerTiming.WhenMinionSummoned or TriggerTiming.AfterMinionSummoned or TriggerTiming.AfterMinionPlayed &&
                        listener.Id.Entity == occurrence.Subject))
                    continue;

                var entityId = occurrence.Subject;
                var player = occurrence.Player;
                if (trigger.Focus == TriggerFocus.DamageSource)
                {
                    if (occurrence.Source is not { } source)
                        continue;
                    entityId = source.Id;
                    player = source.Controller;
                }
                else if (trigger.Focus == TriggerFocus.AttackDefender)
                {
                    entityId = occurrence.Defender!.Value;
                    player = _state.Entities[entityId.Value].Controller;
                }

                if ((trigger.Relation == TriggerRelation.Friendly && listener.Controller != player) ||
                    (trigger.Relation == TriggerRelation.Opponent && listener.Controller == player) ||
                    (trigger.EntityMatch == TriggerEntityMatch.Self && listener.Id.Entity != entityId) ||
                    (trigger.EntityMatch == TriggerEntityMatch.Other && listener.Id.Entity == entityId) ||
                    (trigger.SecretOnly &&
                        (entityId is not EntityId secretId || !_rules.Card(_state.Entities[secretId]).IsSecret)) ||
                    (trigger.EntityKind is EntityKind kind &&
                        (entityId is not EntityId id || _rules.Card(_state.Entities[id]).Definition.Kind != kind)) ||
                    (trigger.AttackingKind is EntityKind atkKind &&
                        (occurrence.Subject is not EntityId attackerId || _rules.Card(_state.Entities[attackerId]).Definition.Kind != atkKind)) ||
                    (trigger.CardFilter is { } cards &&
                        (entityId is not EntityId filtered || !MatchesCardFilter(_rules.Card(_state.Entities[filtered]).Definition, cards))) ||
                    (trigger.RequireLivingEntity && !IsLivingCharacter(entityId)))
                    continue;

                yield return new(new(listener.Id.Entity, listener.Controller, Subject: occurrence.Subject),
                    listener.PlayOrder, trigger.Effects, listener, trigger.RequireLivingEntity ? entityId : null, occurrence.Amount);
            }
        }

        private void RunTriggers(Occurrence occurrence, ImmutableArray<Listener> listeners) =>
            RunQueue(Queue(occurrence, listeners));

        private void RunQueue(IEnumerable<Activate> queue) =>
            RunWork(new Stack<Work>(queue.Reverse()));

        private void ResolveEffects(EffectContext context, ImmutableArray<CardEffect> effects)
        {
            var work = new Stack<Work>();
            PushEffects(work, context, effects);
            RunWork(work);
        }

        private void RunWork(Stack<Work> work)
        {
            while (work.TryPop(out var item))
            {
                switch (item)
                {
                    case ApplyEffect effect:
                        SpendResolutionStep();
                        ExecuteEffect(work, effect.Context, effect.Effect);
                        break;
                    case DrawCards draw:
                        Draw(work, draw);
                        break;
                    case RecruitCards recruit:
                        Recruit(work, recruit);
                        break;
                    case CastDrawnSpell spell:
                        CastWhenDrawn(work, spell);
                        break;
                    case FinishDrawnSpell spell:
                        if (_state.LocationOf(spell.Source).Zone == GameZone.Play)
                            _state = _state.Move(spell.Source, new(spell.Player, GameZone.Graveyard));
                        break;
                    case DealDrawnCostDamage costDamage:
                        ApplyDamageTarget(work, costDamage.Context, costDamage.Target,
                            ModifiedEffectAmount(costDamage.Context, costDamage.PrintedCost, damage: true));
                        break;
                    case ResolveDiscardedCard discarded:
                        _events.Add(new(SimulationEventKind.DiscardedEffect, discarded.Player, discarded.Card));
                        PushEffects(work, new(discarded.Card, discarded.Player), discarded.Effects);
                        break;
                    case Dispatch dispatch:
                        if (dispatch.Occurrence.LethalBonus is TriggerTiming bonus)
                        {
                            // LIFO: regular damage triggers must run before the lethal-bonus triggers.
                            var bonusEvent = dispatch.Occurrence with { Timing = bonus, LethalBonus = null };
                            if (dispatch.Occurrence.AttackingWeapon is DamageSource weapon)
                            {
                                IEnumerable<Activate> weaponQueue = dispatch.WeaponLethalTriggers is { } captured
                                    ? captured : Queue(bonusEvent with { Source = weapon }, CaptureListeners());
                                PushTriggers(work, weaponQueue);
                            }
                            IEnumerable<Activate> lethalQueue = dispatch.LethalTriggers is { } saved
                                ? saved : Queue(bonusEvent, CaptureListeners());
                            PushTriggers(work, lethalQueue);
                        }
                        if (dispatch.Occurrence is { Poisoned: true, Subject: EntityId poisoned })
                            work.Push(new DestroyPoisoned(poisoned));
                        IEnumerable<Activate> damageQueue = dispatch.DamageTriggers is { } beforeTriggers
                            ? beforeTriggers : Queue(dispatch.Occurrence, CaptureListeners());
                        PushTriggers(work, damageQueue);
                        break;
                    case DestroyPoisoned poison:
                        if (_state.LocationOf(poison.Target).Zone == GameZone.Play)
                            _pendingDestruction.Add(poison.Target);
                        break;
                    case Activate trigger:
                        SpendResolutionStep();
                        if (trigger.Listener is { } listener)
                        {
                            if (!IsCurrent(listener) ||
                                (listener.Trigger.Timing == TriggerTiming.HeroAttacking &&
                                    _state.Entities[listener.Id.Entity].Weapon is not null &&
                                    _state.Players[listener.Controller].EquippedWeapon != listener.Id.Entity) ||
                                (listener.Trigger.Timing == TriggerTiming.SpellCounterWindow &&
                                    !CanCounterSpell(trigger.Context.Subject!.Value)) ||
                                (listener.Trigger.RequiresBoardSpace && _rules.BoardSlotCount(_state, listener.Controller) >= BoardLimit) ||
                                (trigger.LivingEntity is EntityId living && !IsLivingCharacter(living)) ||
                                (listener.Trigger.Condition is { } condition && !MeetsCondition(trigger.Context, condition)) ||
                                !_activeTriggers.Add(listener.Id))
                                break;
                            work.Push(new Complete(listener.Id));
                            var kind = listener.Zone == GameZone.Secret ? SimulationEventKind.SecretRevealed : SimulationEventKind.Triggered;
                            if (listener.Zone == GameZone.Secret)
                                _state = _state.Move(listener.Id.Entity, new(listener.Controller, GameZone.Graveyard));
                            _events.Add(new(kind, listener.Controller, listener.Id.Entity,
                                trigger.Context.Subject, trigger.Amount, listener.Trigger.Timing));
                        }
                        else
                        {
                            _events.Add(new(SimulationEventKind.Deathrattle, trigger.Context.Controller, trigger.Context.Source));
                        }
                        PushEffects(work, trigger.Context, trigger.Effects);
                        break;
                    case Complete complete:
                        _activeTriggers.Remove(complete.Listener);
                        break;
                }
            }
        }

        private bool IsCurrent(Listener listener)
        {
            var entity = _state.Entities[listener.Id.Entity];
            return !entity.Silenced && _state.LocationOf(entity.Id).Zone == listener.Zone &&
                entity.Controller == listener.Controller && entity.PlayOrder == listener.PlayOrder &&
                entity.CardId == listener.CardId;
        }

        private bool IsLivingCharacter(EntityId? id) => id is EntityId entity &&
            _state.LocationOf(entity).Zone == GameZone.Play &&
            _state.Entities[entity].Character is { Health: > 0 } && !_pendingDestruction.Contains(entity);

        private static void PushTriggers(Stack<Work> work, IEnumerable<Activate> queue)
        {
            foreach (var trigger in queue.Reverse())
                work.Push(trigger);
        }

        private static void PushEffects(Stack<Work> work, EffectContext context, ImmutableArray<CardEffect> effects)
        {
            for (var i = effects.Length - 1; i >= 0; i--)
                work.Push(new ApplyEffect(context, effects[i]));
        }
    }
}
