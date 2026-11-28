using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    // Phase 2 extraction: keep damage mutation and event-time trigger capture
    // together while preserving the established work-stack ordering.
    private sealed partial class Resolution
    {
        private Occurrence? Damage(EntityId? source, EntityId target, int amount, EntityId? attackingWeapon = null,
            List<(EffectContext Context, int Amount)>? deferredLifesteal = null, bool allowLifesteal = true)
        {
            if (amount <= 0 || _state.LocationOf(target).Zone != GameZone.Play)
                return null;
            var entity = _state.Entities[target];
            if (_rules.IsEntityImmune(_state, entity) || IsImmuneAttackingHero(target))
                return null;
            var defendingPlayer = _state.Players[entity.Controller];
            if (target == defendingPlayer.Hero)
            {
                foreach (var ally in _rules.BoardMinions(_state, entity.Controller))
                    if (IsLivingCharacter(ally) && !_state.Entities[ally].Silenced &&
                        _rules.Card(_state.Entities[ally]).RedirectFriendlyHeroDamageToSelf)
                        return Damage(source, ally, amount, attackingWeapon, deferredLifesteal, allowLifesteal);
                if (defendingPlayer.EquippedWeapon is EntityId weapon && !_state.Entities[weapon].Silenced &&
                    _rules.Card(_state.Entities[weapon]).FriendlyHeroDamageMultiplier is int multiplier)
                    amount = checked(amount * multiplier);
                foreach (var ally in _rules.BoardMinions(_state, entity.Controller))
                    if (IsLivingCharacter(ally) && !_state.Entities[ally].Silenced &&
                        _rules.Card(_state.Entities[ally]).FriendlyHeroDamageCap is int cap)
                        amount = Math.Min(cap, amount);
            }
            var character = entity.Character!;
            if (character.DivineShield)
            {
                // Preserve the foundation branch's enchantment-layer cleanup.
                _state = _state.UpdateEntity(entity with
                {
                    Character = character with { DivineShield = false },
                    Enchantments = [.. entity.Enchantments.Where(layer => layer.GrantedTag != GAME_TAG.DIVINE_SHIELD)]
                });
                _events.Add(new(SimulationEventKind.ShieldBroken, entity.Controller, source, target)
                {
                    AttackingWeapon = attackingWeapon
                });
                return null;
            }

            var absorbed = Math.Min(character.Armor, amount);
            var healthDamage = amount - absorbed;
            if (_rules.Card(entity).Definition.Kind == EntityKind.Minion &&
                _state.Players[entity.Controller].MinionsMinimumOneHealthThisTurn)
            {
                var maxAllowed = Math.Max(0, character.Health - 1);
                if (healthDamage > maxAllowed)
                {
                    healthDamage = maxAllowed;
                    amount = absorbed + healthDamage;
                    if (amount <= 0)
                        return null;
                }
            }
            if (character.Health > 0 && healthDamage >= character.Health && ResolveFatalDamageWindow(target, amount))
                return null;

            var fromActivePlayer = source is EntityId attacker &&
                _state.Entities.TryGetValue(attacker, out var attackingEntity) &&
                attackingEntity.Controller == _state.ActivePlayer;
            TriggerTiming? lethalTiming = null;
            if (fromActivePlayer && character.Health > 0)
            {
                if (healthDamage > character.Health)
                    lethalTiming = TriggerTiming.Overkill;
                else if (healthDamage == character.Health)
                    lethalTiming = TriggerTiming.HonorableKill;
            }

            _state = _state.UpdateCharacter(target, character with
            {
                Armor = character.Armor - absorbed,
                Damage = checked(character.Damage + healthDamage)
            });
            if (healthDamage > 0)
                _state = _rules.RefreshAuras(_state);
            _events.Add(new(SimulationEventKind.Damage, entity.Controller, source, target, amount)
            {
                AttackingWeapon = attackingWeapon,
                DamageToArmor = absorbed, DamageToHealth = healthDamage,
                IsOverkill = lethalTiming == TriggerTiming.Overkill,
                IsHonorableKill = lethalTiming == TriggerTiming.HonorableKill
            });

            var poisonous = healthDamage > 0 && _rules.Card(entity).Definition.Kind == EntityKind.Minion &&
                HasDamageKeyword(CardKeywords.Poisonous);
            if (allowLifesteal && HasDamageKeyword(CardKeywords.Lifesteal) && source is EntityId sourceId &&
                _state.Entities.TryGetValue(sourceId, out var sourceEntity))
            {
                var keywordOwner = attackingWeapon is EntityId weaponId &&
                    _state.Entities.TryGetValue(weaponId, out var weaponEntity) &&
                    _rules.HasKeyword(weaponEntity, CardKeywords.Lifesteal) ? weaponId : sourceId;
                var healContext = new EffectContext(keywordOwner, sourceEntity.Controller);
                if (deferredLifesteal is null)
                    _ = HealFriendlyHero(healContext, amount);
                else
                    deferredLifesteal.Add((healContext, amount));
            }

            return new(TriggerTiming.Damage, entity.Controller, target,
                source is EntityId triggerSource ? new(triggerSource, _state.Entities[triggerSource].Controller) : null,
                amount, Poisoned: poisonous, LethalBonus: lethalTiming,
                AttackingWeapon: attackingWeapon is EntityId w && _state.Entities.TryGetValue(w, out var heldWeapon)
                    ? new(w, heldWeapon.Controller) : null);

            bool HasDamageKeyword(CardKeywords keyword) =>
                (source is EntityId s && _state.Entities.TryGetValue(s, out var damageSource) &&
                    (_rules.HasKeyword(damageSource, keyword) ||
                     keyword == CardKeywords.Lifesteal && _rules.Card(damageSource).Definition.Kind == EntityKind.Spell &&
                        _rules.BoardMinions(_state, damageSource.Controller).Any(id => IsLivingCharacter(id) &&
                            !_state.Entities[id].Silenced && _rules.Card(_state.Entities[id]).SpellsHaveLifesteal) ||
                     keyword == CardKeywords.Poisonous && _rules.HasEffectiveTag(damageSource, GAME_TAG.POISONOUS))) ||
                (attackingWeapon is EntityId w && _state.Entities.TryGetValue(w, out var equipped) &&
                    (_rules.HasKeyword(equipped, keyword) ||
                     keyword == CardKeywords.Poisonous && _rules.HasEffectiveTag(equipped, GAME_TAG.POISONOUS)));
        }

        private void PushOccurrence(Stack<Work> work, Occurrence? occurrence)
        {
            if (occurrence is not { } happened)
                return;

            // Overheal and other non-damage event families keep their previous
            // deferred listener registration; only batch damage is corrected here.
            if (happened.Timing != TriggerTiming.Damage)
            {
                work.Push(new Dispatch(happened));
                return;
            }

            // Damage may be committed to several entities before any damage
            // triggers run (combat, AOE and distinct random batches). Register
            // eligible observers now: a minion summoned by the first damage
            // event must not observe a second hit that already happened.
            var listeners = CaptureListeners();
            var damageTriggers = Queue(happened, listeners).ToImmutableArray();
            ImmutableArray<Activate>? lethalTriggers = null;
            ImmutableArray<Activate>? weaponLethalTriggers = null;
            if (happened.LethalBonus is TriggerTiming bonus)
            {
                var bonusEvent = happened with { Timing = bonus, LethalBonus = null };
                lethalTriggers = Queue(bonusEvent, listeners).ToImmutableArray();
                if (happened.AttackingWeapon is DamageSource weapon)
                    weaponLethalTriggers = Queue(bonusEvent with { Source = weapon }, listeners).ToImmutableArray();
            }
            work.Push(new Dispatch(happened, damageTriggers, lethalTriggers, weaponLethalTriggers));
        }
    }
}
