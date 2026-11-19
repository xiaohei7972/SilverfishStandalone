using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private static bool IsValidModifier(StatModifier? modifier) =>
        modifier is not null && (modifier.Attack is not null || modifier.MaximumHealth is not null) &&
        (modifier.Mode switch
        {
            StatModifierMode.Add => modifier.MaximumHealth is null or >= 0,
            StatModifierMode.Set => (modifier.Attack is null or >= 0) && (modifier.MaximumHealth is null or > 0),
            _ => false
        });

    private void ValidateStatLayers(GameState state, GameEntity entity)
    {
        foreach (var enchantment in entity.Enchantments)
        {
            if (enchantment is null || !state.Entities.ContainsKey(enchantment.Source) ||
                !IsValidModifier(enchantment.Modifier) ||
                (enchantment.ExpiresAfterTurn is int turn && turn < state.Turn))
                throw new ArgumentException($"Entity {entity.Id} has an invalid stat enchantment.");
        }

        foreach (var aura in entity.AuraGrants)
        {
            if (aura is null || !state.Entities.TryGetValue(aura.Source, out var source) || aura.Index < 0 ||
                aura.Index >= Card(source).Auras.Length || aura.Modifier != Card(source).Auras[aura.Index].Modifier)
                throw new ArgumentException($"Entity {entity.Id} has an invalid aura grant.");
        }
    }

    private GameState RefreshAuras(GameState state)
    {
        var grants = new Dictionary<EntityId, ImmutableArray<StatAuraGrant>.Builder>();
        foreach (var player in state.Players.Values)
        {
            var minions = BoardMinions(state, player.Id).ToArray();
            var emitters = Enumerable.Range(0, minions.Length).OrderBy(i => state.Entities[minions[i]].PlayOrder);
            foreach (var index in emitters)
            {
                var source = state.Entities[minions[index]];
                if (source.Silenced)
                    continue;

                var auras = Card(source).Auras;
                for (var auraIndex = 0; auraIndex < auras.Length; auraIndex++)
                {
                    var aura = auras[auraIndex];
                    var first = aura.Recipients == AuraRecipients.AdjacentFriendlyMinions ? Math.Max(0, index - 1) : 0;
                    var last = aura.Recipients == AuraRecipients.AdjacentFriendlyMinions
                        ? Math.Min(minions.Length - 1, index + 1) : minions.Length - 1;
                    for (var position = first; position <= last; position++)
                    {
                        if (position == index)
                            continue;
                        var target = minions[position];
                        if (!grants.TryGetValue(target, out var received))
                        {
                            received = ImmutableArray.CreateBuilder<StatAuraGrant>();
                            grants.Add(target, received);
                        }
                        received.Add(new(source.Id, auraIndex, aura.Modifier));
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
            Apply(enchantment.Modifier);
        foreach (var aura in grants)
            Apply(aura.Modifier);

        // A maximum Health increase adds the same current Health. A decrease
        // clamps current Health to the new maximum without counting as damage.
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
                _ => throw new InvalidOperationException("Unknown stat enchantment target.")
            };
            var expires = effect.Duration == EnchantmentDuration.CurrentTurn ? _state.Turn : (int?)null;
            foreach (var id in targets.ToArray())
            {
                if (_state.LocationOf(id).Zone != GameZone.Play)
                    continue;
                var entity = _state.Entities[id];
                var enchantment = new StatEnchantment(context.Source, effect.Modifier, expires);
                // Setting Health clears existing damage once, when the effect is applied.
                if (effect.Modifier is { Mode: StatModifierMode.Set, MaximumHealth: not null })
                    entity = entity with { Character = entity.Character! with { Damage = 0 } };
                _state = _state.UpdateEntity(RecalculateStats(entity with
                {
                    Enchantments = entity.Enchantments.Add(enchantment)
                }, entity.AuraGrants));
                _events.Add(new(SimulationEventKind.EnchantmentApplied, context.Controller, context.Source, id));
            }
        }

        private void Silence(EffectContext context)
        {
            var id = context.Selected!.Value;
            if (_state.LocationOf(id).Zone != GameZone.Play)
                return;

            var entity = _state.Entities[id];
            var silenced = entity with
            {
                Silenced = true, Enchantments = [],
                Character = entity.Character! with { DivineShield = false }
            };
            _state = _state.UpdateEntity(RecalculateStats(silenced, entity.AuraGrants));
            _events.Add(new(SimulationEventKind.Silenced, context.Controller, context.Source, id));
        }

        private void ExpireTurnEnchantments()
        {
            var entities = _state.Players.Values.SelectMany(player => player.In(GameZone.Play))
                .Select(id => _state.Entities[id]).OrderBy(entity => entity.PlayOrder).ToArray();
            foreach (var entity in entities)
            {
                var expired = entity.Enchantments.Where(modifier => modifier.ExpiresAfterTurn <= _state.Turn).ToArray();
                if (expired.Length == 0)
                    continue;
                var remaining = entity.Enchantments.Where(modifier => !(modifier.ExpiresAfterTurn <= _state.Turn));
                var updated = RecalculateStats(entity with { Enchantments = remaining.ToImmutableArray() }, entity.AuraGrants);
                _state = _state.UpdateEntity(updated);
                foreach (var modifier in expired)
                    _events.Add(new(SimulationEventKind.EnchantmentExpired, entity.Controller, modifier.Source, entity.Id));
            }
        }
    }
}
