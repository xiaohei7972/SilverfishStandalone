using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    private sealed partial class Resolution
    {
        public GameState ResolveStartOfGame()
        {
            var players = _state.Players.Values.OrderBy(player => player.Id != _state.ActivePlayer);
            var sources = players.SelectMany(player => player.In(GameZone.Hand).Concat(player.In(GameZone.Deck)))
                .Select(id => _state.Entities[id])
                .Where(entity => !_rules.Card(entity).StartOfGame.IsEmpty)
                .ToArray();
            foreach (var source in sources)
            {
                if (_state.LocationOf(source.Id).Zone is not (GameZone.Hand or GameZone.Deck))
                    continue;
                var effects = _rules.Card(source).StartOfGame;
                ResolveEffects(new(source.Id, source.Controller), effects);
                ResolveDeaths();
                FinishIfHeroesDied();
                if (_state.Outcome != MatchOutcome.InProgress)
                    break;
            }
            return _state;
        }

        private void AddUniqueClassMinionsToDeck(EffectContext context, AddUniqueClassMinionsToDeckEffect effect)
        {
            var player = _state.Players[context.Controller];
            var existing = player.In(GameZone.Hand).Concat(player.In(GameZone.Deck))
                .Select(id => OriginalCardId(_state.Entities[id].CardId)).ToHashSet(StringComparer.Ordinal);
            var candidates = _rules.DiscoverCardCandidates(_state, player.Id, effect.CardFilter)
                .Where(id => !existing.Contains(OriginalCardId(id))).DistinctBy(OriginalCardId).ToList();

            var count = Math.Min(effect.Count, candidates.Count);
            for (var i = 0; i < count; i++)
            {
                SpendResolutionStep();
                var index = ChooseIndex(candidates.Count);
                TryCreateCard(candidates[index], player.Id, context.Source, CardPlacement.DeckBottom);
                candidates.RemoveAt(index);
            }
            if (count > 0)
                ShuffleDeck(player.Id, context.Source);
        }

        private static string OriginalCardId(string id) => id.StartsWith("CORE_", StringComparison.Ordinal) ? id[5..] : id;
    }
}
