using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    public int EffectiveAttack(GameState state, GameEntity entity)
    {
        if (entity.Character is not { } character)
            return 0;

        var player = state.Players[entity.Controller];
        if (entity.Id != player.Hero)
            return character.Attack;
        if (entity.Controller != state.ActivePlayer)
            return 0;

        var weaponAttack = player.EquippedWeapon is EntityId weapon ? state.Entities[weapon].Weapon!.Attack : 0;
        return checked(character.Attack + weaponAttack);
    }

    private sealed partial class Resolution
    {
        private readonly HashSet<EntityId> _pendingWeaponDestruction = [];

        private void UseHeroPower(PlayerId playerId, EntityId source, EntityId? target)
        {
            var power = _rules.Card(_state.Entities[source]);
            PayMana(playerId, power.Definition.PrintedCost);
            _events.Add(new(SimulationEventKind.HeroPowerUsed, playerId, source, target));
            ResolveEffects(new(source, playerId, target), power.OnPlay);
            _state = _state.SetHeroPowerUses(playerId, 1);
            ResolveDeaths();
            RunTriggers(new(TriggerTiming.AfterHeroPowerUsed, playerId, source), CaptureListeners());
        }

        private void PayMana(PlayerId playerId, int cost)
        {
            var player = _state.Players[playerId];
            _state = _state.SetResources(playerId, player.Mana - cost, player.ManaCrystals, player.Fatigue);
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
                _pendingWeaponDestruction.Add(oldWeapon);
        }

        private void SetEquippedWeapon(PlayerId player, EntityId id)
        {
            _state = _state.EquipWeapon(player, id);
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

        private void HealFriendlyHero(EffectContext context, int amount)
        {
            var hero = _state.Players[context.Controller].Hero;
            if (!IsLivingCharacter(hero))
                return;

            var character = _state.Entities[hero].Character!;
            var restored = Math.Min(character.Damage, amount);
            if (restored == 0)
                return;

            _state = _state.UpdateCharacter(hero, character with { Damage = character.Damage - restored });
            _events.Add(new(SimulationEventKind.Healed, context.Controller, context.Source, hero, restored));
        }
    }
}
