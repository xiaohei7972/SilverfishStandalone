using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed partial class Resolution
    {
        private EntityId _proposedDefender;

        private bool CanCounterSpell(EntityId id) => _state.LocationOf(id).Zone is GameZone.Play or GameZone.Secret;

        private void CounterSpell(EffectContext context)
        {
            var id = context.Subject!.Value;
            if (!CanCounterSpell(id))
                return;

            _state = _state.Move(id, new(_state.Entities[id].Controller, GameZone.Graveyard));
            _events.Add(new(SimulationEventKind.SpellCountered, context.Controller, context.Source, id));
        }

        private EntityId ResolveAttackProposal(PlayerId actor, EntityId attacker, EntityId defender)
        {
            while (true)
            {
                _proposedDefender = defender;
                RunTriggers(new(TriggerTiming.AttackProposed, actor, attacker, Defender: defender), CaptureListeners());
                if (_proposedDefender == defender)
                    return defender;
                defender = _proposedDefender;
            }
        }

        private void SummonAttackDefender(Stack<Work> work, EffectContext context, string cardId)
        {
            if (Summon(context.Controller, cardId, position: null) is not EntityId defender)
                return;

            _proposedDefender = defender;
            _events.Add(new(SimulationEventKind.AttackRedirected, context.Controller, context.Source, defender));
            PushSummonTriggers(work, context.Controller, defender);
        }
    }
}
