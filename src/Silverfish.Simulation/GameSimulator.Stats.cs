using System.Collections.Immutable;
using System.Text.Json;
using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private void RegisterEnchantmentRemovalEffects(SimulationCard card, EnchantStatsEffect enchantment)
    {
        if (enchantment.EnchantmentCardId is not { } cardId)
            throw new ArgumentException("声明移除效果的属性附魔必须提供卡牌 ID。");

        var reasons = new HashSet<EnchantmentRemovalReason>();
        foreach (var removal in enchantment.OnRemoved)
        {
            if (removal is null || !Enum.IsDefined(removal.Reason) || !reasons.Add(removal.Reason) || removal.Effects.IsDefaultOrEmpty)
                throw new ArgumentException($"附魔「{cardId}」的移除规则无效或原因重复。");

            var key = (cardId, removal.Reason);
            if (_enchantmentRemovalEffects.TryAdd(key, removal.Effects))
            {
                ValidateEffects(card, removal.Effects, TargetRequirement.Character, hasCharacterSubject: true);
                continue;
            }

            // 同一附魔可被多张牌引用。存档还原后，嵌套效果的数组也按内容比较。
            var existing = _enchantmentRemovalEffects[key];
            if (existing != removal.Effects && JsonSerializer.Serialize(existing) != JsonSerializer.Serialize(removal.Effects))
                throw new ArgumentException($"附魔「{cardId}」同一移除原因的效果定义不一致。");
        }
    }

    private void RegisterEnchantmentAttackEffects(SimulationCard card, EnchantStatsEffect enchantment)
    {
        if (enchantment.EnchantmentCardId is not { } cardId)
            throw new ArgumentException("声明攻击触发效果的属性附魔必须提供卡牌 ID。");

        if (_enchantmentAttackEffects.TryAdd(cardId, enchantment.OnAttack))
        {
            ValidateEffects(card, enchantment.OnAttack, hasCharacterSubject: true);
            return;
        }
        var existing = _enchantmentAttackEffects[cardId];
        if (existing != enchantment.OnAttack && JsonSerializer.Serialize(existing) != JsonSerializer.Serialize(enchantment.OnAttack))
            throw new ArgumentException($"附魔「{cardId}」的攻击触发效果定义不一致。");
    }

    private static bool IsValidModifier(StatModifier? modifier) =>
        modifier is not null && (modifier.Attack is not null || modifier.MaximumHealth is not null) &&
        (modifier.Mode switch
        {
            StatModifierMode.Add => modifier.MaximumHealth is null or >= 0,
            StatModifierMode.Set => (modifier.Attack is null or >= 0) && (modifier.MaximumHealth is null or >= 0),
            _ => false
        });

    private static bool IsSupportedAuraTag(GAME_TAG tag) => tag is
        GAME_TAG.TAUNT or GAME_TAG.CHARGE or GAME_TAG.RUSH or GAME_TAG.WINDFURY or GAME_TAG.ELUSIVE or
        GAME_TAG.STEALTH or GAME_TAG.POISONOUS or GAME_TAG.LIFESTEAL or GAME_TAG.IMMUNE;

    private static bool IsSupportedGrantedTag(GAME_TAG tag) =>
        IsSupportedAuraTag(tag) || tag is GAME_TAG.DIVINE_SHIELD or GAME_TAG.DEATHRATTLE or GAME_TAG.CONTROLLER or GAME_TAG.SPELLPOWER;

    private static bool IsValidEnchantment(StatModifier? modifier, GAME_TAG? tag, bool hasTrigger = false) =>
        (modifier is not null || tag is not null || hasTrigger) &&
        (modifier is null || IsValidModifier(modifier)) &&
        (tag is not GAME_TAG granted || IsSupportedGrantedTag(granted));

    private void ValidateStatLayers(GameState state, GameEntity entity)
    {
        foreach (var enchantment in entity.Enchantments)
        {
            if (enchantment is null || !state.Entities.ContainsKey(enchantment.Source) ||
                !IsValidEnchantment(enchantment.Modifier, enchantment.GrantedTag,
                    enchantment.CardId is { } cid &&
                    (_enchantmentAttackEffects.ContainsKey(cid) || _enchantmentRemovalEffects.Keys.Any(k => k.CardId == cid))) ||
                !IsOptionalCardIdValid(enchantment.CardId) ||
                !IsValidCapturedMinion(state, enchantment.CapturedMinion) ||
                (enchantment.ExpiresAfterTurn is int turn && turn < state.Turn))
                throw new ArgumentException($"实体 {entity.Id} 的属性附魔无效。");
        }

        foreach (var aura in entity.AuraGrants)
        {
            if (aura is null || !state.Entities.TryGetValue(aura.Source, out var source) || aura.Index < 0 ||
                aura.Index >= Card(source).Auras.Length || aura.Modifier != Card(source).Auras[aura.Index].Modifier ||
                aura.GrantedTag != Card(source).Auras[aura.Index].GrantedTag ||
                (aura.CardId is not null && aura.CardId != Card(source).Auras[aura.Index].EnchantmentCardId))
                throw new ArgumentException($"实体 {entity.Id} 获得的光环无效。");
        }
    }

    private bool IsAuraActive(GameState state, GameEntity source, AuraCondition condition) => condition switch
    {
        AuraCondition.None => true,
        AuraCondition.WhileDamaged => source.Character is { Damage: > 0 },
        AuraCondition.WhileWeaponEquipped => state.Players[source.Controller].EquippedWeapon is EntityId weapon &&
            state.Entities[weapon].Weapon is { Durability: > 0 },
        AuraCondition.WhileControllingMech => BoardMinions(state, source.Controller).Any(id =>
            state.Entities[id].Character is { Health: > 0 } &&
            MatchesMinionType(Card(state.Entities[id]).Definition, MinionType.Mech)),
        AuraCondition.WhileOpponentHasAtLeastSixHandCards => state.Players[state.OpponentOf(source.Controller)].In(GameZone.Hand).Length >= 6,
        AuraCondition.WhileHavingSpellDamage => state.Players.Keys.SelectMany(player => BoardMinions(state, player))
            .Any(id => state.Entities[id] is { Character.Health: > 0 } minion && SpellDamageBonus(minion, source.Controller) > 0),
        AuraCondition.ControllerTurnOnly => state.ActivePlayer == source.Controller,
        _ => false
    };

    private GameState RefreshAuras(GameState state)
    {
        var grants = new Dictionary<EntityId, ImmutableArray<StatAuraGrant>.Builder>();
        foreach (var player in state.Players.Values)
        {
            var slots = BoardSlots(state, player.Id).Select(id => state.Entities[id]).ToArray();
            var emitters = Enumerable.Range(0, slots.Length).OrderBy(i => slots[i].PlayOrder);
            foreach (var index in emitters)
            {
                var source = slots[index];
                if (source.Character is null || source.Silenced)
                    continue;

                var auras = Card(source).Auras;
                for (var auraIndex = 0; auraIndex < auras.Length; auraIndex++)
                {
                    var aura = auras[auraIndex];
                    if (!IsAuraActive(state, source, aura.Condition))
                        continue;
                    if (aura.Recipients == AuraRecipients.Self)
                    {
                        AddGrant(source.Id, source.Id, auraIndex, aura);
                        continue;
                    }
                    if (aura.Recipients == AuraRecipients.FriendlyHero)
                    {
                        if (source.Character.Health > 0)
                            AddGrant(source.Id, player.Hero, auraIndex, aura);
                        continue;
                    }

                    var first = aura.Recipients == AuraRecipients.AdjacentFriendlyMinions ? Math.Max(0, index - 1) : 0;
                    var last = aura.Recipients == AuraRecipients.AdjacentFriendlyMinions
                        ? Math.Min(slots.Length - 1, index + 1) : slots.Length - 1;
                    for (var position = first; position <= last; position++)
                    {
                        if (position == index || slots[position].Character is null ||
                            (aura.CardFilter is { } cards && !MatchesCardFilter(Card(slots[position]).Definition, cards)))
                            continue;
                        AddGrant(source.Id, slots[position].Id, auraIndex, aura);
                    }
                }
            }
        }

        foreach (var id in state.Players.Values.SelectMany(player => player.In(GameZone.Play)))
        {
            var entity = state.Entities[id];
            if (entity.Character is null)
                continue;
            var active = grants.TryGetValue(id, out var received) ? received.ToImmutable() : [];
            var updated = RecalculateStats(entity, active, !entity.Silenced && Card(entity).AttackEqualsHealth);
            if (updated != entity)
                state = state.UpdateEntity(updated);
        }
        return state;

        void AddGrant(EntityId sourceId, EntityId targetId, int auraIndex, StatAura aura)
        {
            if (!grants.TryGetValue(targetId, out var received))
            {
                received = ImmutableArray.CreateBuilder<StatAuraGrant>();
                grants.Add(targetId, received);
            }
            received.Add(new(sourceId, auraIndex, aura.Modifier) { CardId = aura.EnchantmentCardId, GrantedTag = aura.GrantedTag });
        }
    }

    private static GameEntity RecalculateStats(GameEntity entity, ImmutableArray<StatAuraGrant> grants, bool attackEqualsHealth = false)
    {
        var character = entity.Character!;
        if (!attackEqualsHealth && entity.Enchantments.IsEmpty && grants.IsEmpty && entity.AuraGrants.IsEmpty &&
            character.BaseAttack is null && character.BaseMaximumHealth is null)
            return entity;

        var baseAttack = character.BaseAttack ?? character.Attack;
        var baseHealth = character.BaseMaximumHealth ?? character.MaximumHealth;
        var attack = baseAttack;
        var maximumHealth = baseHealth;
        foreach (var enchantment in entity.Enchantments)
            if (enchantment.Modifier is { } modifier)
                Apply(modifier);
        foreach (var aura in grants)
            if (aura.Modifier is { } modifier)
                Apply(modifier);

        // 生命上限提高时等量增加当前生命值。上限降低时只截断超出的生命值，不触发伤害。
        var damage = maximumHealth < character.MaximumHealth
            ? Math.Max(0, maximumHealth - character.Health)
            : character.Damage;
        if (attackEqualsHealth)
            attack = Math.Max(0, maximumHealth - damage);

        return entity with
        {
            AuraGrants = entity.AuraGrants.SequenceEqual(grants) ? entity.AuraGrants : grants,
            Character = character with
            {
                Attack = Math.Max(0, attack), MaximumHealth = maximumHealth, Damage = damage,
                BaseAttack = baseAttack, BaseMaximumHealth = baseHealth
            }
        };

        void Apply(StatModifier modifier)
        {
            if (modifier.Attack is int a)
                attack = modifier.Mode == StatModifierMode.Set ? a : checked(attack + a);
            if (modifier.MaximumHealth is int hp)
                maximumHealth = modifier.Mode == StatModifierMode.Set ? hp : checked(maximumHealth + hp);
        }
    }

    private sealed partial class Resolution
    {
        private void EnchantStats(EffectContext context, EnchantStatsEffect effect, CapturedMinion? captured = null)
        {
            IEnumerable<EntityId> targets = effect.Target switch
            {
                StatTarget.Selected => [context.Selected!.Value],
                StatTarget.Source => [context.Source],
                StatTarget.TriggerSubject => [context.Subject!.Value],
                StatTarget.FriendlyHero => [_state.Players[context.Controller].Hero],
                StatTarget.FriendlyMinions => _rules.BoardMinions(_state, context.Controller),
                StatTarget.EnemyMinions => _rules.BoardMinions(_state, _state.OpponentOf(context.Controller)),
                StatTarget.FriendlyHandAndDeck => _state.Players[context.Controller].In(GameZone.Hand)
                    .Concat(_state.Players[context.Controller].In(GameZone.Deck)),
                StatTarget.FriendlyHand => _state.Players[context.Controller].In(GameZone.Hand),
                StatTarget.OtherFriendlyMinions => _rules.BoardMinions(_state, context.Controller).Where(id => id != context.Source),
                StatTarget.AdjacentFriendlyMinions => AdjacentMinions(context.Source),
                StatTarget.RandomOtherFriendlyMinion => PickRandomOtherFriendlyMinion(context, effect.CardFilter),
                StatTarget.RandomFriendlyHandMinion => PickRandomFriendlyHandMinion(context, effect.CardFilter),
                StatTarget.AllMinions => _state.Players.Values.SelectMany(player => _rules.BoardMinions(_state, player.Id))
                    .OrderBy(id => _state.Entities[id].PlayOrder),
                _ => throw new InvalidOperationException("无法识别属性附魔的目标。")
            };
            int? expires = effect.Duration switch
            {
                EnchantmentDuration.CurrentTurn => _state.Turn,
                EnchantmentDuration.UntilNextTurn => checked(_state.Turn + (_state.ActivePlayer == context.Controller ? 1 : 0)),
                _ => null
            };
            var damageCleared = false;
            foreach (var id in targets.ToArray())
            {
                var zone = _state.LocationOf(id).Zone;
                var entity = _state.Entities[id];
                var card = _rules.Card(entity);
                if (effect.CardFilter is { } filter && !_rules.MatchesEntityFilter(entity, filter))
                    continue;
                if (card.Definition.Kind == EntityKind.Minion && effect.Modifier is not null &&
                    (zone == GameZone.Hand && effect.Target is StatTarget.Source or StatTarget.FriendlyHand or StatTarget.RandomFriendlyHandMinion ||
                     zone is GameZone.Hand or GameZone.Deck && effect.Target == StatTarget.FriendlyHandAndDeck))
                {
                    var handEnchant = new StatEnchantment(context.Source, effect.Modifier, expires)
                    {
                        CardId = effect.EnchantmentCardId, GrantedTag = effect.GrantedTag
                    };
                    _state = _state.UpdateEntity(entity with { Enchantments = entity.Enchantments.Add(handEnchant) });
                    _events.Add(new(SimulationEventKind.EnchantmentApplied, context.Controller, context.Source, id));
                    continue;
                }
                if (zone != GameZone.Play)
                    continue;
                if (entity.Character is not { } character ||
                    (effect.ShieldedOnly && !character.DivineShield) ||
                    (effect.Modifier is null && effect.SecondaryGrantedTag is null &&
                        effect.GrantedTag == GAME_TAG.DIVINE_SHIELD && character.DivineShield))
                    continue;
                var layers = entity.Enchantments;
                if (effect.Modifier is not null || effect.GrantedTag is not GAME_TAG primaryTag ||
                    primaryTag != GAME_TAG.DIVINE_SHIELD || !character.DivineShield)
                {
                    var enchantment = new StatEnchantment(context.Source, effect.Modifier, expires)
                    {
                        CardId = effect.EnchantmentCardId, GrantedTag = effect.GrantedTag, CapturedMinion = captured
                    };
                    if (effect.GrantedTag == GAME_TAG.DIVINE_SHIELD)
                        character = character with { DivineShield = true };
                    layers = layers.Add(enchantment);
                }
                if (effect.SecondaryGrantedTag is GAME_TAG secondTag &&
                    !(secondTag == GAME_TAG.DIVINE_SHIELD && character.DivineShield))
                {
                    if (secondTag == GAME_TAG.DIVINE_SHIELD)
                        character = character with { DivineShield = true };
                    layers = layers.Add(new StatEnchantment(context.Source, null, expires)
                    {
                        CardId = effect.EnchantmentCardId, GrantedTag = secondTag
                    });
                }
                // 设定生命值时只在效果应用的那一刻清除已有伤害。
                if (effect.Modifier is { Mode: StatModifierMode.Set, MaximumHealth: not null })
                {
                    damageCleared |= character.Damage > 0;
                    character = character with { Damage = 0 };
                }
                if (layers.Length == entity.Enchantments.Length)
                    continue;
                _state = _state.UpdateEntity(RecalculateStats(entity with
                {
                    Character = character, Enchantments = layers
                }, entity.AuraGrants, !entity.Silenced && _rules.Card(entity).AttackEqualsHealth));
                _events.Add(new(SimulationEventKind.EnchantmentApplied, context.Controller, context.Source, id));
            }
            if (damageCleared)
                _state = _rules.RefreshAuras(_state);
        }

        private IEnumerable<EntityId> PickRandomOtherFriendlyMinion(EffectContext context, CardFilter? filter = null)
        {
            var candidates = _rules.BoardMinions(_state, context.Controller)
                .Where(id => id != context.Source && IsLivingCharacter(id) &&
                    (filter is null || MatchesCardFilter(_rules.Card(_state.Entities[id]), filter)))
                .OrderBy(id => _state.Entities[id].PlayOrder)
                .ToArray();
            return candidates.Length == 0 ? [] : [candidates[ChooseIndex(candidates.Length)]];
        }

        private void ApplyRandomEnchantStats(EffectContext context, RandomEnchantStatsEffect effect)
        {
            var atk = effect.MinAttack + ChooseIndex(effect.MaxAttack - effect.MinAttack + 1);
            var hp = effect.MinHealth + ChooseIndex(effect.MaxHealth - effect.MinHealth + 1);
            if (atk <= 0 && hp <= 0)
                return;

            EnchantStats(context, new(effect.Target,
                new(StatModifierMode.Add, atk > 0 ? atk : null, hp > 0 ? hp : null))
            {
                EnchantmentCardId = effect.EnchantmentCardId
            });
        }

        private void ApplyScaledEnchantStats(EffectContext context, ScaledEnchantStatsEffect effect)
        {
            var count = effect.Scale switch
            {
                StatScaleSource.CardsPlayedBeforeThis => context.CardsPlayedBefore,
                StatScaleSource.FriendlyHandCount => _state.Players[context.Controller].In(GameZone.Hand).Length,
                StatScaleSource.EquippedWeaponAttack => _state.Players[context.Controller].EquippedWeapon is EntityId w
                    ? _rules.EffectiveWeaponAttack(_state, _state.Entities[w]) : 0,
                StatScaleSource.EnemyDeathrattleMinions => _rules.BoardMinions(_state, _state.OpponentOf(context.Controller))
                    .Count(id => IsLivingCharacter(id) &&
                        MatchesCardFilter(_rules.Card(_state.Entities[id]), new() { HasDeathrattle = true })),
                StatScaleSource.FriendlyMinions or StatScaleSource.EnemyMinions =>
                    _rules.BoardMinions(_state, effect.Scale == StatScaleSource.FriendlyMinions
                        ? context.Controller : _state.OpponentOf(context.Controller))
                    .Count(id => IsLivingCharacter(id) &&
                        (effect.CardFilter is null || MatchesCardFilter(_rules.Card(_state.Entities[id]), effect.CardFilter))),
                _ => throw new InvalidOperationException("无法识别属性缩放来源。")
            };
            if (count <= 0)
                return;

            int? atk = effect.AttackPerUnit != 0 ? checked(effect.AttackPerUnit * count) : null;
            int? hp = effect.HealthPerUnit != 0 ? checked(effect.HealthPerUnit * count) : null;
            EnchantStats(context, new(effect.Target, new(StatModifierMode.Add, atk, hp))
            {
                EnchantmentCardId = effect.EnchantmentCardId
            });
        }

        private void ApplyDynamicStatChange(EffectContext context, DynamicStatChangeEffect effect)
        {
            if (effect.Target == StatTarget.AllMinions)
            {
                foreach (var id in _state.Players.Keys.SelectMany(player => _rules.BoardMinions(_state, player)).ToArray())
                    ApplyDynamicStatChange(context with { Selected = id }, effect with { Target = StatTarget.Selected });
                return;
            }
            EntityId targetId = effect.Target switch
            {
                StatTarget.Selected => context.Selected!.Value,
                StatTarget.Source => context.Source,
                StatTarget.TriggerSubject => context.Subject!.Value,
                _ => throw new InvalidOperationException("无法识别动态属性修改的目标。")
            };
            if (_state.LocationOf(targetId).Zone != GameZone.Play || _state.Entities[targetId].Character is not { } character)
                return;

            var entity = _state.Entities[targetId];
            var modifier = effect.Mode switch
            {
                DynamicStatMode.AttackEqualsHealth => new StatModifier(StatModifierMode.Set, Attack: Math.Max(0, character.Health)),
                DynamicStatMode.DoubleAttack => new StatModifier(StatModifierMode.Set, Attack: checked(_rules.EffectiveAttack(_state, entity) * 2)),
                DynamicStatMode.SwapAttackAndHealth => new StatModifier(StatModifierMode.Set,
                    Attack: Math.Max(0, character.Health),
                    MaximumHealth: Math.Max(0, _rules.EffectiveAttack(_state, entity))),
                _ => throw new InvalidOperationException("无法识别动态属性修改的模式。")
            };
            EnchantStats(context, new(effect.Target, modifier) { EnchantmentCardId = effect.EnchantmentCardId });
        }

        private void Silence(Stack<Work> work, EffectContext context, SilenceEffect effect)
        {
            if (effect.AllEnemyMinions)
            {
                var enemies = _rules.BoardMinions(_state, _state.OpponentOf(context.Controller))
                    .OrderBy(id => _state.Entities[id].PlayOrder).ToArray();
                foreach (var enemyId in enemies)
                    SilenceEntity(work, context.Controller, context.Source, enemyId);
                return;
            }
            if (effect.OtherFriendlyMinions)
            {
                var friendlies = _rules.BoardMinions(_state, context.Controller)
                    .Where(id => id != context.Source)
                    .OrderBy(id => _state.Entities[id].PlayOrder).ToArray();
                foreach (var friendlyId in friendlies)
                    SilenceEntity(work, context.Controller, context.Source, friendlyId);
                return;
            }
            SilenceEntity(work, context.Controller, context.Source, context.Selected!.Value);
        }

        private void SilenceEntity(Stack<Work> work, PlayerId controller, EntityId source, EntityId id)
        {
            if (_state.LocationOf(id).Zone != GameZone.Play)
                return;

            var entity = _state.Entities[id];
            var silenced = entity with
            {
                Silenced = true, Stealthed = false, Enchantments = [],
                Character = entity.Character! with { DivineShield = false, Frozen = false }
            };
            _state = _state.UpdateEntity(RecalculateStats(silenced, entity.AuraGrants));
            // A silenced aura source stops buffing other minions immediately,
            // including during a multi-effect spell before its damage step.
            _state = _rules.RefreshAuras(_state);
            _events.Add(new(SimulationEventKind.Silenced, controller, source, id));
            PushEnchantmentRemovalEffects(work, entity, entity.Enchantments, EnchantmentRemovalReason.Silenced);
        }

        private void PushEnchantmentRemovalEffects(Stack<Work> work, GameEntity holder,
            ImmutableArray<StatEnchantment> removed, EnchantmentRemovalReason reason)
        {
            for (var index = removed.Length - 1; index >= 0; index--)
            {
                var layer = removed[index];
                if (layer.CardId is { } cardId && _rules._enchantmentRemovalEffects.TryGetValue((cardId, reason), out var effects))
                    PushEffects(work, new(layer.Source, holder.Controller, Selected: holder.Id, Subject: holder.Id), effects);
            }
        }

        private bool ExpireTurnEnchantments()
        {
            List<(GameEntity Holder, ImmutableArray<StatEnchantment> Layers)>? removed = null;
            var entities = _state.Players.Values.SelectMany(player => player.In(GameZone.Play))
                .Select(id => _state.Entities[id]).OrderBy(entity => entity.PlayOrder).ToArray();
            foreach (var entity in entities)
            {
                var expired = entity.Enchantments.Where(modifier => modifier.ExpiresAfterTurn <= _state.Turn).ToImmutableArray();
                if (expired.IsEmpty)
                    continue;
                var remaining = entity.Enchantments.Where(modifier => !(modifier.ExpiresAfterTurn <= _state.Turn)).ToImmutableArray();
                var updated = entity with { Enchantments = remaining };
                if (expired.Any(layer => layer.GrantedTag == GAME_TAG.DIVINE_SHIELD))
                    updated = updated with { Character = updated.Character! with { DivineShield = false } };
                _state = _state.UpdateEntity(RecalculateStats(updated, entity.AuraGrants,
                    !updated.Silenced && _rules.Card(updated).AttackEqualsHealth));
                foreach (var modifier in expired)
                    _events.Add(new(SimulationEventKind.EnchantmentExpired, entity.Controller, modifier.Source, entity.Id));
                if (_rules._enchantmentRemovalEffects.Count != 0)
                    (removed ??= []).Add((entity, expired));
            }

            if (removed is null)
                return false;
            var work = new Stack<Work>();
            for (var index = removed.Count - 1; index >= 0; index--)
            {
                var (holder, layers) = removed[index];
                PushEnchantmentRemovalEffects(work, holder, layers, EnchantmentRemovalReason.Expired);
            }
            var hasEffects = work.Count != 0;
            RunWork(work);
            return hasEffects;
        }
        private ImmutableArray<EntityId> PickRandomFriendlyHandMinion(EffectContext context, CardFilter? filter)
        {
            var candidates = _state.Players[context.Controller].In(GameZone.Hand)
                .Where(id => _rules.Card(_state.Entities[id]).Definition.Kind == EntityKind.Minion)
                .Where(id => filter is null || _rules.MatchesEntityFilter(_state.Entities[id], filter))
                .ToArray();
            return candidates.Length == 0 ? [] : [candidates[ChooseIndex(candidates.Length)]];
        }
    }
}
