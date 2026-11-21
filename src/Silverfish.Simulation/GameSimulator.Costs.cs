using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private static bool SupportsCostType(TAG_CARD_ALTERNATE_COST type) => type is
        TAG_CARD_ALTERNATE_COST.MANA or TAG_CARD_ALTERNATE_COST.HEALTH or
        TAG_CARD_ALTERNATE_COST.ARMOR or TAG_CARD_ALTERNATE_COST.CORPSES;

    private static TAG_CARD_ALTERNATE_COST EffectiveCostType(GameEntity entity, SimulationCard card) =>
        entity.CostTypeModifiers is { IsEmpty: false } modifiers ? modifiers[^1].Type :
            card.Definition.AlternateCost.GetValueOrDefault();

    private static bool CanPayCardCost(GameState state, PlayerState player, TAG_CARD_ALTERNATE_COST type, int cost) =>
        type switch
        {
            TAG_CARD_ALTERNATE_COST.MANA => player.Mana >= cost,
            TAG_CARD_ALTERNATE_COST.HEALTH => state.Entities[player.Hero].Character is { } health && health.Health > cost,
            TAG_CARD_ALTERNATE_COST.ARMOR => state.Entities[player.Hero].Character is { } armor && armor.Armor >= cost,
            TAG_CARD_ALTERNATE_COST.CORPSES => player.AvailableCorpses >= cost,
            _ => false
        };

    /// <summary>计算指定手牌或英雄技能在当前局面下的实际费用。</summary>
    public int EffectiveCost(GameState state, EntityId id)
    {
        if (state is null)
            throw new ArgumentNullException(nameof(state), "必须提供对局状态。");
        if (!state.Entities.TryGetValue(id, out var entity) ||
            state.LocationOf(id).Zone is not (GameZone.Hand or GameZone.HeroPower))
            throw new ArgumentException($"实体 {id} 必须是手牌或英雄技能，才能查询当前费用。", nameof(id));

        return EffectiveCost(entity, Card(entity), ActiveCostAuras(state, entity.Controller));
    }

    private ImmutableArray<CostAura> ActiveCostAuras(GameState state, PlayerId recipient)
    {
        if (!_hasCostAuras)
            return [];

        var applicable = ImmutableArray.CreateBuilder<CostAura>();
        foreach (var player in state.Players.Values)
        {
            var relation = player.Id == recipient ? CostAuraRecipient.Friendly : CostAuraRecipient.Opponent;
            foreach (var id in player.In(GameZone.Play))
            {
                var source = state.Entities[id];
                if (source.Silenced ||
                    (source.Character is not { Health: > 0 } &&
                        (player.EquippedWeapon != id || source.Weapon is not { Durability: > 0 })))
                    continue;

                foreach (var aura in Card(source).CostAuras)
                    if (aura.Recipient == relation || aura.Recipient == CostAuraRecipient.BothPlayers)
                        applicable.Add(aura);
            }
        }
        return applicable.ToImmutable();
    }

    private static int EffectiveCost(GameEntity entity, SimulationCard card, ImmutableArray<CostAura> auras)
    {
        long cost = card.Definition.PrintedCost;
        foreach (var modifier in entity.CostModifiers)
            cost += modifier.Delta;

        if (!auras.IsEmpty)
        {
            var target = card.Definition.Kind switch
            {
                EntityKind.Minion => CostAuraTarget.Minion,
                EntityKind.Spell => CostAuraTarget.Spell,
                EntityKind.Weapon => CostAuraTarget.Weapon,
                EntityKind.HeroPower => CostAuraTarget.HeroPower,
                EntityKind.Location => CostAuraTarget.Location,
                _ => throw new InvalidOperationException("该实体类型不支持计算使用费用。")
            };
            foreach (var aura in auras)
                if ((aura.Target == target || (aura.Target == CostAuraTarget.Hand && target != CostAuraTarget.HeroPower)) &&
                    (aura.CardFilter is not { } cards || MatchesCardFilter(card.Definition, cards)))
                    cost += aura.Delta;
        }

        // 把单卡修正和场上光环全部相加后，再限制实际费用的范围。
        return (int)Math.Clamp(cost, 0, int.MaxValue);
    }

    private sealed partial class Resolution
    {
        private void PayCardCost(PlayerId playerId, EntityId source, SimulationCard card)
        {
            var cost = _rules.EffectiveCost(_state, source);
            var type = EffectiveCostType(_state.Entities[source], card);
            if (type == TAG_CARD_ALTERNATE_COST.MANA)
            {
                PayMana(playerId, cost);
                return;
            }
            if (cost == 0)
                return;

            var player = _state.Players[playerId];
            if (type == TAG_CARD_ALTERNATE_COST.CORPSES)
            {
                _state = _state.UpdateResources(player with { CorpsesSpentThisGame = checked(player.CorpsesSpentThisGame + cost) });
                _events.Add(new(SimulationEventKind.AlternateCostPaid, playerId, source, Amount: cost) { CostType = type });
                return;
            }

            // 支付生命或护甲直接扣除资源，伤害结算由独立的伤害流程处理。
            var hero = _state.Entities[player.Hero].Character!;
            var remaining = type switch
            {
                TAG_CARD_ALTERNATE_COST.HEALTH => hero with { Damage = checked(hero.Damage + cost) },
                TAG_CARD_ALTERNATE_COST.ARMOR => hero with { Armor = hero.Armor - cost },
                _ => throw new InvalidOperationException($"卡牌「{card.Definition.CardId}」的替代费用类型为 {type}，尚未接入该支付方式。")
            };
            _state = _state.UpdateCharacter(player.Hero, remaining);
            _events.Add(new(SimulationEventKind.AlternateCostPaid, playerId, source, player.Hero, cost) { CostType = type });
        }

        private IEnumerable<GameEntity> HandCostTargets(EffectContext context, HandCostTarget target, CardFilter? filter)
        {
            ImmutableArray<EntityId> targets = target switch
            {
                HandCostTarget.Source => [context.Source],
                HandCostTarget.DrawnCard => [context.Subject!.Value],
                HandCostTarget.FriendlyHand => _state.Players[context.Controller].In(GameZone.Hand),
                _ => throw new InvalidOperationException("无法识别手牌费用修正的目标。")
            };
            foreach (var id in targets)
            {
                if (_state.LocationOf(id).Zone != GameZone.Hand)
                    continue;

                var entity = _state.Entities[id];
                if (filter is null || MatchesCardFilter(_rules.Card(entity).Definition, filter))
                    yield return entity;
            }
        }

        private int? CostExpiry(EnchantmentDuration duration) =>
            duration == EnchantmentDuration.CurrentTurn ? _state.Turn : null;

        private ManaCostModifier CreateCostModifier(EntityId source, int delta, string? cardId, EnchantmentDuration duration) =>
            new(source, delta) { CardId = cardId, ExpiresAfterTurn = CostExpiry(duration) };

        private void ChangeHandCost(EffectContext context, ChangeHandCostEffect effect)
        {
            var modifier = CreateCostModifier(context.Source, effect.Delta, effect.EnchantmentCardId, effect.Duration);
            foreach (var entity in HandCostTargets(context, effect.Target, effect.CardFilter))
            {
                _state = _state.UpdateEntity(entity with { CostModifiers = entity.CostModifiers.Add(modifier) });
                _events.Add(new(SimulationEventKind.EnchantmentApplied, context.Controller, context.Source, entity.Id, effect.Delta)
                    { EnchantmentCardId = modifier.CardId });
            }
        }

        private void ChangeCostType(EffectContext context, ChangeCostTypeEffect effect)
        {
            var expires = CostExpiry(effect.Duration);
            var modifier = new CostTypeModifier(context.Source, effect.Type, expires) { CardId = effect.EnchantmentCardId };
            foreach (var entity in HandCostTargets(context, effect.Target, effect.CardFilter))
            {
                var card = _rules.Card(entity);

                // 永久修正覆盖此前的支付方式。临时修正保留底层效果，到期后可恢复。
                var modifiers = expires is null ? [modifier] : (entity.CostTypeModifiers ?? []).Add(modifier);
                _state = _state.UpdateEntity(entity with { CostTypeModifiers = modifiers });
                if (EffectiveCostType(entity, card) != effect.Type)
                    _events.Add(new(SimulationEventKind.CostTypeChanged, context.Controller, context.Source, entity.Id)
                        { CostType = effect.Type, EnchantmentCardId = modifier.CardId });
            }
        }

        private void ExpireTurnHandCostModifiers()
        {
            var turn = _state.Turn;
            foreach (var id in _state.Players.Values.SelectMany(player => player.In(GameZone.Hand)))
            {
                var entity = _state.Entities[id];
                var amounts = entity.CostModifiers.RemoveAll(cost => cost.ExpiresAfterTurn <= turn);
                var types = entity.CostTypeModifiers?.RemoveAll(cost => cost.ExpiresAfterTurn <= turn);
                if (amounts == entity.CostModifiers && types == entity.CostTypeModifiers)
                    continue;

                var updated = entity with { CostModifiers = amounts, CostTypeModifiers = types is { IsEmpty: true } ? null : types };
                _state = _state.UpdateEntity(updated);

                foreach (var cost in entity.CostModifiers)
                    if (cost.ExpiresAfterTurn <= turn)
                        _events.Add(new(SimulationEventKind.EnchantmentExpired, entity.Controller, cost.Source, id)
                            { EnchantmentCardId = cost.CardId });

                if (types == entity.CostTypeModifiers)
                    continue;

                var card = _rules.Card(entity);
                var restoredType = EffectiveCostType(updated, card);
                if (restoredType != EffectiveCostType(entity, card))
                {
                    var expired = entity.CostTypeModifiers!.Value[^1];
                    _events.Add(new(SimulationEventKind.CostTypeChanged, entity.Controller, expired.Source, id)
                        { CostType = restoredType, EnchantmentCardId = expired.CardId });
                }
            }
        }
    }
}
