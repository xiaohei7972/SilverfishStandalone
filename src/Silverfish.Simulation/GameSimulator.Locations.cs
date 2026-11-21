using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private static bool IsValidLocationState(EntityKind kind, GameZone zone, LocationState? location)
    {
        if (kind != EntityKind.Location)
            return location is null;

        return zone switch
        {
            GameZone.Play => location is { Health: > 0 },
            GameZone.Graveyard => location is null or { Health: 0 },
            _ => location is null
        };
    }

    private sealed partial class Resolution
    {
        private void ActivateLocation(PlayerId player, EntityId source, EntityId? target, bool resolvesEffects)
        {
            var entity = _state.Entities[source];
            var card = _rules.Card(entity);
            var ability = card.Activation!;
            var removeFirst = ability.RemoveBeforeFinalEffects && entity.Location!.Health == 1;
            _events.Add(new(SimulationEventKind.LocationActivated, player, source, target));
            if (removeFirst)
                SpendLocationUse(source);
            if (resolvesEffects)
                ResolveEffects(new(source, player, target), ability.Effects);
            if (!removeFirst)
                SpendLocationUse(source);
        }

        private void SpendLocationUse(EntityId source)
        {
            var entity = _state.Entities[source];
            var health = entity.Location!.Health - 1;
            var player = entity.Controller;
            var cooldown = health > 0 ? LocationState.StartsUntilReadyAfterUse : 0;
            _state = _state.UpdateEntity(entity with { Location = new(health, cooldown) });
            _events.Add(new(SimulationEventKind.LocationHealthLost, player, source, Amount: 1));
            if (health == 0)
            {
                _state = _state.Move(source, new(player, GameZone.Graveyard));
                _events.Add(new(SimulationEventKind.LocationDestroyed, player, source));
                _state = _rules.RefreshAuras(_state);
            }
        }
    }
}
