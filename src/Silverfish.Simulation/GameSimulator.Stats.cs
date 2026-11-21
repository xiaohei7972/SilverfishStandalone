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

    private static bool IsValidModifier(StatModifier? modifier) =>
        modifier is not null && (modifier.Attack is not null || modifier.MaximumHealth is not null) &&
        (modifier.Mode switch
        {
            StatModifierMode.Add => modifier.MaximumHealth is null or >= 0,
            StatModifierMode.Set => (modifier.Attack is null or >= 0) && (modifier.MaximumHealth is null or > 0),
            _ => false
        });

    private static bool IsValidEnchantment(StatModifier? modifier, GAME_TAG? tag) =>
        tag is null ? IsValidModifier(modifier) : modifier is null && tag is
            GAME_TAG.TAUNT or GAME_TAG.WINDFURY or GAME_TAG.ELUSIVE or GAME_TAG.STEALTH or GAME_TAG.DIVINE_SHIELD or
            GAME_TAG.POISONOUS or GAME_TAG.DEATHRATTLE;

    private void ValidateStatLayers(GameState state, GameEntity entity)
    {
        foreach (var enchantment in entity.Enchantments)
        {
            if (enchantment is null || !state.Entities.ContainsKey(enchantment.Source) ||
                !IsValidEnchantment(enchantment.Modifier, enchantment.GrantedTag) ||
                !IsOptionalCardIdValid(enchantment.CardId) ||
                (enchantment.ExpiresAfterTurn is int turn && turn < state.Turn))
                throw new ArgumentException($"实体 {entity.Id} 的属性附魔无效。");
        }

        foreach (var aura in entity.AuraGrants)
        {
            if (aura is null || !state.Entities.TryGetValue(aura.Source, out var source) || aura.Index < 0 ||
                aura.Index >= Card(source).Auras.Length || aura.Modifier != Card(source).Auras[aura.Index].Modifier ||
                (aura.CardId is not null && aura.CardId != Card(source).Auras[aura.Index].EnchantmentCardId))
                throw new ArgumentException($"实体 {entity.Id} 获得的光环无效。");
        }
    }

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
                    var first = aura.Recipients == AuraRecipients.AdjacentFriendlyMinions ? Math.Max(0, index - 1) : 0;
                    var last = aura.Recipients == AuraRecipients.AdjacentFriendlyMinions
                        ? Math.Min(slots.Length - 1, index + 1) : slots.Length - 1;
                    for (var position = first; position <= last; position++)
                    {
                        if (position == index || slots[position].Character is null ||
                            (aura.CardFilter is { } cards && !MatchesCardFilter(Card(slots[position]).Definition, cards)))
                            continue;
                        var target = slots[position].Id;
                        if (!grants.TryGetValue(target, out var received))
                        {
                            received = ImmutableArray.CreateBuilder<StatAuraGrant>();
                            grants.Add(target, received);
                        }
                        received.Add(new(source.Id, auraIndex, aura.Modifier) { CardId = aura.EnchantmentCardId });
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
            var updated = RecalculateStats(entity, active);
            if (updated != entity)
                state = state.UpdateEntity(updated);
        }
        return state;
    }

    private static GameEntity RecalculateStats(GameEntity entity, ImmutableArray<StatAuraGrant> grants)
    {
        var character = entity.Character!;
        if (entity.Enchantments.IsEmpty && grants.IsEmpty &&
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
            Apply(aura.Modifier);

        // 生命上限提高时等量增加当前生命值。上限降低时只截断超出的生命值，不触发伤害。
        var damage = maximumHealth < character.MaximumHealth
            ? Math.Max(0, maximumHealth - character.Health)
            : character.Damage;

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
        private void EnchantStats(EffectContext context, EnchantStatsEffect effect)
        {
            IEnumerable<EntityId> targets = effect.Target switch
            {
                StatTarget.Selected => [context.Selected!.Value],
                StatTarget.Source => [context.Source],
                StatTarget.FriendlyHero => [_state.Players[context.Controller].Hero],
                StatTarget.FriendlyMinions => _rules.BoardMinions(_state, context.Controller),
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
            foreach (var id in targets.ToArray())
            {
                if (_state.LocationOf(id).Zone != GameZone.Play)
                    continue;
                var entity = _state.Entities[id];
                if (entity.Character is not { } character || effect.GrantedTag == GAME_TAG.DIVINE_SHIELD && character.DivineShield)
                    continue;
                var enchantment = new StatEnchantment(context.Source, effect.Modifier, expires)
                {
                    CardId = effect.EnchantmentCardId, GrantedTag = effect.GrantedTag
                };
                // 设定生命值时只在效果应用的那一刻清除已有伤害。
                if (effect.Modifier is { Mode: StatModifierMode.Set, MaximumHealth: not null })
                    character = character with { Damage = 0 };
                if (effect.GrantedTag == GAME_TAG.DIVINE_SHIELD)
                    character = character with { DivineShield = true };
                _state = _state.UpdateEntity(RecalculateStats(entity with
                {
                    Character = character, Enchantments = entity.Enchantments.Add(enchantment)
                }, entity.AuraGrants));
                _events.Add(new(SimulationEventKind.EnchantmentApplied, context.Controller, context.Source, id));
            }
        }

        private void Silence(Stack<Work> work, EffectContext context)
        {
            var id = context.Selected!.Value;
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
            _events.Add(new(SimulationEventKind.Silenced, context.Controller, context.Source, id));
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
                _state = _state.UpdateEntity(RecalculateStats(updated, entity.AuraGrants));
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
    }
}
