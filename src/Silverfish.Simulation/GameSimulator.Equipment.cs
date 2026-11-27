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

        private void SpendWeaponDurability(EntityId? weapon, EntityId defender)
        {
            if (weapon is not EntityId id || _state.LocationOf(id).Zone != GameZone.Play)
                return;

            var entity = _state.Entities[id];
            var stats = entity.Weapon!;
            if (!entity.Silenced && _rules.Card(entity).MinionAttackRule is { } minionRule &&
                _state.Entities.TryGetValue(defender, out var targetEntity) &&
                _rules.Card(targetEntity).Definition.Kind == EntityKind.Minion)
            {
                var reducedAttack = Math.Max(0, stats.Attack - minionRule.AttackLoss);
                _state = _state.UpdateEntity(entity with { Weapon = stats with { Attack = reducedAttack } });
                _events.Add(new(SimulationEventKind.EnchantmentApplied, entity.Controller, id, id)
                {
                    EnchantmentCardId = minionRule.EnchantmentCardId
                });
                _state = _rules.RefreshAuras(_state);
                return;
            }

            _state = _state.UpdateEntity(entity with { Weapon = stats with { Durability = stats.Durability - 1 } });
            _events.Add(new(SimulationEventKind.WeaponDurabilityLost, entity.Controller, id, Amount: 1));
        }

        private void ReplaceHeroPower(PlayerId playerId, EntityId source, string cardId)
        {
            var player = _state.Players[playerId];
            if (player.HeroPowerUsesThisTurn != 0)
                _state = _state.UpdateResources(player with { HeroPowerUsesThisTurn = 0 });
            foreach (var oldPowerId in _state.Players[playerId].In(GameZone.HeroPower))
                _state = _state.Move(oldPowerId, new(playerId, GameZone.Removed));

            var newId = new EntityId(_state.NextEntity);
            _state = _state.AddToHeroPower(new(newId, cardId, playerId, playerId));
            _events.Add(new(SimulationEventKind.CardGenerated, playerId, source, newId));
        }

        private void PlayHeroCard(PlayerId playerId, EntityId source)
        {
            var oldHeroId = _state.Players[playerId].Hero;
            var oldHero = _state.Entities[oldHeroId];
            var baseChar = oldHero.Character!;
            var tempHero = _state.Entities[source] with
            {
                Character = baseChar,
                Enchantments = oldHero.Enchantments,
                AuraGrants = []
            };
            tempHero = RecalculateStats(tempHero, []);
            _state = _state.ReplaceHero(playerId, source, tempHero.Character!, tempHero.Enchantments);
            _state = _rules.RefreshAuras(_state);
            _events.Add(new(SimulationEventKind.Transformed, playerId, source, source));
        }

        private void ModifyWeapon(EffectContext context, ModifyWeaponEffect effect)
        {
            var owner = effect.TargetWeapon == ConditionPlayer.Friendly
                ? context.Controller
                : _state.OpponentOf(context.Controller);
            if (_state.Players[owner].EquippedWeapon is not EntityId id || _state.LocationOf(id).Zone != GameZone.Play)
                return;

            var entity = _state.Entities[id];
            if (entity.Weapon is not { Durability: > 0 } stats)
                return;

            var attack = Math.Max(0, checked(stats.Attack + effect.AttackDelta));
            var durability = Math.Max(0, checked(stats.Durability + effect.DurabilityDelta));
            var maxDurability = effect.DurabilityDelta > 0
                ? checked(stats.MaximumDurability + effect.DurabilityDelta)
                : stats.MaximumDurability;
            _state = _state.UpdateEntity(entity with
            {
                Weapon = stats with { Attack = attack, Durability = durability, MaximumDurability = maxDurability }
            });
            if (effect.AttackDelta != 0 || effect.DurabilityDelta > 0)
            {
                _events.Add(new(SimulationEventKind.EnchantmentApplied, context.Controller, context.Source, id)
                {
                    EnchantmentCardId = effect.EnchantmentCardId
                });
            }
            if (effect.DurabilityDelta < 0)
            {
                var lost = stats.Durability - durability;
                if (lost > 0)
                    _events.Add(new(SimulationEventKind.WeaponDurabilityLost, owner, id, Amount: lost));
            }
            _state = _rules.RefreshAuras(_state);
        }

        private Occurrence? HealFriendlyHero(EffectContext context, int amount) =>
            HealCharacter(context, _state.Players[context.Controller].Hero, amount);
    }
}
