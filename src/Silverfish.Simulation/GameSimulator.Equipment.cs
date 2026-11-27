using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    public int EffectiveWeaponAttack(GameState state, GameEntity weapon)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(weapon);
        if (weapon.Weapon is not { Durability: > 0 } stats || state.LocationOf(weapon.Id).Zone != GameZone.Play)
            return 0;

        long attack = stats.Attack;
        if (_hasWeaponAuras)
        {
            foreach (var id in BoardMinions(state, weapon.Controller))
            {
                var source = state.Entities[id];
                if (source.Silenced || source.Character is not { Health: > 0 })
                    continue;
                foreach (var aura in Card(source).WeaponAuras)
                    if (IsAuraActive(state, source, aura.Condition))
                        attack += aura.Attack;
            }
        }
        return (int)Math.Clamp(attack, 0, int.MaxValue);
    }

    public int EffectiveAttack(GameState state, GameEntity entity)
    {
        if (entity.Character is not { } character)
            return 0;

        var player = state.Players[entity.Controller];
        if (entity.Id != player.Hero)
            return character.Attack;
        if (entity.Controller != state.ActivePlayer)
            return 0;

        var weaponAttack = player.EquippedWeapon is EntityId weapon ? EffectiveWeaponAttack(state, state.Entities[weapon]) : 0;
        return checked(character.Attack + weaponAttack);
    }

    private sealed partial class Resolution
    {
        private void UseHeroPower(PlayerId playerId, EntityId source, EntityId? target, bool resolvesEffects)
        {
            var power = _rules.Card(_state.Entities[source]);
            PayCardCost(playerId, source, power);
            _events.Add(new(SimulationEventKind.HeroPowerUsed, playerId, source, target));
            if (resolvesEffects)
                ResolveEffects(new(source, playerId, target), power.OnPlay);
            _state = _state.UpdateResources(_state.Players[playerId] with { HeroPowerUsesThisTurn = 1 });
            ResolveDeaths();
            RunTriggers(new(TriggerTiming.AfterHeroPowerUsed, playerId, source), CaptureListeners());
        }

        private void CreateAndEquipWeapon(PlayerId player, string cardId)
        {
            var id = new EntityId(_state.NextEntity);
            var weapon = new GameEntity(id, cardId, player, player) { Weapon = _rules.GetCard(cardId).CreateWeapon() };
            MarkEquippedWeaponForDestruction(player);
            _state = _state.AddToPlay(weapon, _state.Players[player].In(GameZone.Play).Length);
            SetEquippedWeapon(player, id);
        }

        private void MarkEquippedWeaponForDestruction(PlayerId player)
        {
            if (_state.Players[player].EquippedWeapon is EntityId oldWeapon)
                _pendingDestruction.Add(oldWeapon);
        }

        private void SetEquippedWeapon(PlayerId player, EntityId id)
        {
            _state = _state.EquipWeapon(player, id);
            _state = _rules.RefreshAuras(_state);
            _events.Add(new(SimulationEventKind.WeaponEquipped, player, id, _state.Players[player].Hero));
        }

        private void SpendWeaponDurability(EntityId? weapon)
        {
            if (weapon is not EntityId id || _state.LocationOf(id).Zone != GameZone.Play)
                return;

            var entity = _state.Entities[id];
            var stats = entity.Weapon!;
            _state = _state.UpdateEntity(entity with { Weapon = stats with { Durability = stats.Durability - 1 } });
            _events.Add(new(SimulationEventKind.WeaponDurabilityLost, entity.Controller, id, Amount: 1));
        }

        private Occurrence? HealFriendlyHero(EffectContext context, int amount) =>
            HealCharacter(context, _state.Players[context.Controller].Hero, amount);
    }
}
