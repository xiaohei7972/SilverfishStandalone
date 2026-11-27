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

        private EntityId? ChooseDeckCard(ImmutableArray<EntityId> deck, CardFilter? filter, EntityKind? kind = null)
        {
            var candidates = deck.Where(id =>
            {
                var definition = _rules.Card(_state.Entities[id]).Definition;
                return (kind is null || definition.Kind == kind) &&
                    (filter is null || MatchesCardFilter(definition, filter));
            }).ToArray();
            return candidates.Length == 0 ? null : candidates[ChooseIndex(candidates.Length)];
        }
    }
}
