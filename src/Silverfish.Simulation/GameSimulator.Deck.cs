using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed partial class Resolution
    {
        private void Draw(Stack<Work> work, DrawCards draw)
        {
            SpendResolutionStep();
            var player = _state.Players[draw.Player];
            var deck = player.In(GameZone.Deck);
            var selected = draw.Filter is { } filter ? ChooseDeckCard(deck, filter)
                : deck.IsEmpty ? null : deck[0];
            if (selected is null && draw.Filter is not null)
                return;

            // 下一次抽牌排在本次触发器和嵌套效果之后。
            if (draw.Remaining > 1)
                work.Push(draw with { Remaining = draw.Remaining - 1 });

            if (selected is not EntityId drawn)
            {
                var fatigue = checked(player.Fatigue + 1);
                _state = _state.UpdateResources(player with { Fatigue = fatigue });
                _events.Add(new(SimulationEventKind.Fatigue, player.Id, Target: player.Hero, Amount: fatigue));
                PushOccurrence(work, Damage(null, player.Hero, fatigue));
                return;
            }

            var burned = player.In(GameZone.Hand).Length >= HandLimit;
            _state = _state.Move(drawn, new(player.Id, burned ? GameZone.Graveyard : GameZone.Hand));
            _events.Add(new(burned ? SimulationEventKind.CardBurned : SimulationEventKind.CardDrawn, player.Id, drawn));
            if (burned)
                return;

            if (draw.CostDelta != 0 && draw.CostSource is EntityId costSource)
            {
                var modifier = CreateCostModifier(costSource, draw.CostDelta, draw.CostEnchantmentId, EnchantmentDuration.Permanent);
                var drawnEntity = _state.Entities[drawn];
                _state = _state.UpdateEntity(drawnEntity with { CostModifiers = drawnEntity.CostModifiers.Add(modifier) });
                _events.Add(new(SimulationEventKind.EnchantmentApplied, player.Id, costSource, drawn, draw.CostDelta)
                    { EnchantmentCardId = modifier.CardId });
            }

            var card = _rules.Card(_state.Entities[drawn]);
            var castsWhenDrawn = card.Keywords.HasFlag(CardKeywords.CastsWhenDrawn);
            // 补抽从牌库顶开始，并排在本次自动施放和抽牌触发器之后。
            if (castsWhenDrawn)
                work.Push(new DrawCards(player.Id, 1));
            PushTriggers(work, Queue(new(TriggerTiming.CardDrawn, player.Id, drawn), CaptureListeners()));
            if (castsWhenDrawn)
                work.Push(new CastDrawnSpell(player.Id, drawn));

            var drawEffects = card.WhenDrawn;
            if (!drawEffects.IsEmpty)
            {
                _events.Add(new(SimulationEventKind.DrawnEffect, player.Id, drawn));
                PushEffects(work, new(drawn, player.Id), drawEffects);
            }
        }

        private void DrawAndTransformIfMinion(Stack<Work> work, EffectContext context, DrawAndTransformIfMinionEffect effect)
        {
            var player = _state.Players[context.Controller];
            var beforeHand = player.In(GameZone.Hand).ToHashSet();
            Draw(work, new DrawCards(player.Id, 1));
            var afterHand = _state.Players[player.Id].In(GameZone.Hand);
            var drawnId = afterHand.FirstOrDefault(id => !beforeHand.Contains(id));
            if (drawnId == default)
                return;

            var drawnEntity = _state.Entities[drawnId];
            if (_rules.Card(drawnEntity).Definition.Kind == EntityKind.Minion)
            {
                _state = _state.UpdateEntity(drawnEntity with
                {
                    CardId = effect.TransformCardId,
                    CostModifiers = [],
                    CostTypeModifiers = null,
                    Enchantments = []
                });
                _events.Add(new(SimulationEventKind.Transformed, player.Id, context.Source, drawnId));
            }
        }

        private void DrawAndDiscountIfMatches(Stack<Work> work, EffectContext context, DrawAndDiscountIfMatchesEffect effect)
        {
            var player = _state.Players[context.Controller];
            var beforeHand = player.In(GameZone.Hand).ToHashSet();
            Draw(work, new DrawCards(player.Id, 1));
            var afterHand = _state.Players[player.Id].In(GameZone.Hand);
            var drawnId = afterHand.FirstOrDefault(id => !beforeHand.Contains(id));
            if (drawnId == default)
                return;

            var drawnEntity = _state.Entities[drawnId];
            if (MatchesCardFilter(_rules.Card(drawnEntity), effect.CardFilter))
            {
                var modifier = CreateCostModifier(context.Source, effect.CostDelta, effect.EnchantmentCardId, EnchantmentDuration.Permanent);
                _state = _state.UpdateEntity(drawnEntity with { CostModifiers = drawnEntity.CostModifiers.Add(modifier) });
                _events.Add(new(SimulationEventKind.EnchantmentApplied, player.Id, context.Source, drawnId, effect.CostDelta)
                {
                    EnchantmentCardId = modifier.CardId
                });
            }
        }

        private void DrawToHandSize(Stack<Work> work, EffectContext context, DrawToHandSizeEffect effect)
        {
            var player = ResolveRecipient(effect.Recipient, context.Controller);
            var needed = Math.Max(0, effect.TargetSize - _state.Players[player].In(GameZone.Hand).Length);
            if (needed > 0)
                work.Push(new DrawCards(player, needed));
        }

        private void MillDeck(EffectContext context, MillDeckEffect effect)
        {
            var player = ResolveRecipient(effect.Recipient, context.Controller);
            for (var i = 0; i < effect.Count; i++)
            {
                var deck = _state.Players[player].In(GameZone.Deck);
                if (deck.IsEmpty)
                    break;
                SpendResolutionStep();
                var top = deck[0];
                _state = _state.Move(top, new(player, GameZone.Graveyard));
                _events.Add(new(SimulationEventKind.CardBurned, player, top));
            }
        }

        private void Recruit(Stack<Work> work, RecruitCards recruit)
        {
            var player = recruit.Player;
            var deck = _state.Players[player].In(GameZone.Deck);
            if (_rules.BoardSlotCount(_state, player) >= BoardLimit ||
                ChooseDeckCard(deck, recruit.Filter, EntityKind.Minion) is not EntityId summoned)
                return;

            SpendResolutionStep();
            if (recruit.Remaining > 1)
                work.Push(recruit with { Remaining = recruit.Remaining - 1 });

            SummonExistingMinion(player, summoned);
            _events.Add(new(SimulationEventKind.Recruited, player, summoned));
            PushSummonTriggers(work, player, summoned);
        }

        private void PutSecretFromDeck(EffectContext context, PutSecretFromDeckEffect effect)
        {
            var player = context.Controller;
            for (var i = 0; i < effect.Count; i++)
            {
                var secrets = _state.Players[player].In(GameZone.Secret);
                if (secrets.Length >= SecretLimit)
                    break;

                var existing = secrets.Select(id => _state.Entities[id].CardId).ToHashSet(StringComparer.Ordinal);
                var candidates = _state.Players[player].In(GameZone.Deck)
                    .Where(id =>
                    {
                        var entity = _state.Entities[id];
                        return _rules.Card(entity).IsSecret && !existing.Contains(entity.CardId);
                    }).ToArray();
                if (candidates.Length == 0)
                    break;

                SpendResolutionStep();
                var chosen = candidates[ChooseIndex(candidates.Length)];
                _state = _state.Move(chosen, new(player, GameZone.Secret));
                _events.Add(new(SimulationEventKind.SecretArmed, player, chosen));
            }
        }

        private EntityId? ChooseDeckCard(ImmutableArray<EntityId> deck, CardFilter? filter, EntityKind? kind = null)
        {
            var candidates = deck.Where(id =>
            {
                var card = _rules.Card(_state.Entities[id]);
                return (kind is null || card.Definition.Kind == kind) &&
                    (filter is null || MatchesCardFilter(card, filter));
            }).ToArray();
            return candidates.Length == 0 ? null : candidates[ChooseIndex(candidates.Length)];
        }
    }
}
