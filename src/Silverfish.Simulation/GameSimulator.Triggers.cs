using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed partial class Resolution
    {
        private readonly HashSet<ListenerId> _activeTriggers = [];

        private readonly record struct EffectContext(EntityId Source, PlayerId Controller,
            EntityId? Selected = null, EntityId? Subject = null, int? DeathPosition = null);

        private readonly record struct Occurrence(TriggerTiming Timing, PlayerId Player, EntityId? Subject = null);

        private readonly record struct ListenerId(EntityId Entity, int Index);

        private sealed record Listener(ListenerId Id, string CardId, PlayerId Controller, long PlayOrder, CardTrigger Trigger);

        private abstract record Work;
        private sealed record ApplyEffect(EffectContext Context, CardEffect Effect) : Work;
        private sealed record Dispatch(Occurrence Occurrence) : Work;
        private sealed record Activate(EffectContext Context, long PlayOrder, ImmutableArray<CardEffect> Effects,
            Listener? Listener = null) : Work;
        private sealed record Complete(ListenerId Listener) : Work;

        private ImmutableArray<Listener> CaptureListeners()
        {
            var listeners = ImmutableArray.CreateBuilder<Listener>();
            var sources = _state.Players.Values.SelectMany(player => player.In(GameZone.Play))
                .Select(id => _state.Entities[id]).OrderBy(entity => entity.PlayOrder);
            foreach (var entity in sources)
            {
                var triggers = _rules.Card(entity).Triggers;
                for (var index = 0; index < triggers.Length; index++)
                    listeners.Add(new(new(entity.Id, index), entity.CardId, entity.Controller, entity.PlayOrder!.Value, triggers[index]));
            }
            return listeners.ToImmutable();
        }

        private static IEnumerable<Activate> Queue(Occurrence occurrence, ImmutableArray<Listener> listeners)
        {
            foreach (var listener in listeners)
            {
                if (listener.Trigger.Timing != occurrence.Timing || listener.Id.Entity == occurrence.Subject ||
                    (listener.Trigger.Relation == TriggerRelation.Friendly && listener.Controller != occurrence.Player) ||
                    (listener.Trigger.Relation == TriggerRelation.Opponent && listener.Controller == occurrence.Player))
                    continue;

                yield return new(new(listener.Id.Entity, listener.Controller, Subject: occurrence.Subject),
                    listener.PlayOrder, listener.Trigger.Effects, listener);
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
                    case Dispatch dispatch:
                        foreach (var trigger in Queue(dispatch.Occurrence, CaptureListeners()).Reverse())
                            work.Push(trigger);
                        break;
                    case Activate trigger:
                        SpendResolutionStep();
                        if (trigger.Listener is { } listener)
                        {
                            if (!IsCurrent(listener) || !_activeTriggers.Add(listener.Id))
                                break;
                            work.Push(new Complete(listener.Id));
                            _events.Add(new(SimulationEventKind.Triggered, listener.Controller, listener.Id.Entity,
                                trigger.Context.Subject, Timing: listener.Trigger.Timing));
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
            return _state.LocationOf(entity.Id).Zone == GameZone.Play &&
                entity.Controller == listener.Controller && entity.PlayOrder == listener.PlayOrder &&
                entity.CardId == listener.CardId;
        }

        private static void PushEffects(Stack<Work> work, EffectContext context, ImmutableArray<CardEffect> effects)
        {
            for (var i = effects.Length - 1; i >= 0; i--)
                work.Push(new ApplyEffect(context, effects[i]));
        }
    }
}
