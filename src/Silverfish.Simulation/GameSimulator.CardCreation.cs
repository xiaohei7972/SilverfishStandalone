using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed partial class Resolution
    {
        private void CreateCards(EffectContext context, CreateCardEffect effect)
        {
            var recipient = effect.Recipient == CardRecipient.Controller ? context.Controller : _state.OpponentOf(context.Controller);
            for (var i = 0; i < effect.Count; i++)
            {
                if (!TryCreateCard(effect.CardId, recipient, context.Source, effect.Placement))
                    break;
                SpendResolutionStep();
            }

            if (effect.Placement == CardPlacement.ShuffleIntoDeck)
                ShuffleDeck(recipient, context.Source);
        }

        private void CopyDeckCards(EffectContext context, CopyDeckCardsEffect effect)
        {
            var deckOwner = effect.SourceDeck == ConditionPlayer.Friendly
                ? context.Controller : _state.OpponentOf(context.Controller);
            var candidates = _state.Players[deckOwner].In(GameZone.Deck)
                .Where(id => effect.CardFilter is not { } filter || MatchesCardFilter(_rules.Card(_state.Entities[id]).Definition, filter))
                .ToList();
            var count = Math.Min(effect.Count, candidates.Count);
            for (var i = 0; i < count; i++)
            {
                if (_state.Players[context.Controller].In(GameZone.Hand).Length >= HandLimit)
                    break;
                SpendResolutionStep();
                var index = ChooseIndex(candidates.Count);
                var chosen = _state.Entities[candidates[index]];
                candidates.RemoveAt(index);
                TryCreateCard(chosen.CardId, context.Controller, context.Source, CardPlacement.Hand);
            }
        }

        private bool TryCreateCard(string cardId, PlayerId recipient, EntityId source, CardPlacement placement)
        {
            var player = _state.Players[recipient];
            var inHand = placement == CardPlacement.Hand;
            if (inHand && player.In(GameZone.Hand).Length >= HandLimit)
                return false;

            var id = new EntityId(_state.NextEntity);
            var card = new GameEntity(id, cardId, recipient, recipient);
            if (inHand)
            {
                _state = _state.AddToHand(card);
                _events.Add(new(SimulationEventKind.CardGenerated, recipient, source, id));
                return true;
            }

            var deckSize = player.In(GameZone.Deck).Length;
            var index = placement switch
            {
                CardPlacement.DeckTop => 0,
                CardPlacement.DeckBottom or CardPlacement.ShuffleIntoDeck => deckSize,
                CardPlacement.RandomDeckPosition => ChooseIndex(checked(deckSize + 1)),
                _ => throw new InvalidOperationException("无法识别新卡的放置方式。")
            };
            _state = _state.AddToDeck(card, index);
            _events.Add(new(SimulationEventKind.CardAddedToDeck, recipient, source, id));
            return true;
        }

        private void ShuffleDeck(PlayerId recipient, EntityId source)
        {
            var deck = _state.Players[recipient].In(GameZone.Deck).ToArray();
            // 先加入本批新卡，再把整副牌库当作同一个集合洗牌。
            for (var i = deck.Length - 1; i > 0; i--)
            {
                SpendResolutionStep();
                var index = ChooseIndex(i + 1);
                (deck[i], deck[index]) = (deck[index], deck[i]);
            }
            _state = _state.ReorderDeck(recipient, [.. deck]);
            _events.Add(new(SimulationEventKind.DeckShuffled, recipient, source));
        }
    }
}
