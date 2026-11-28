using System.Collections.ObjectModel;

namespace HREngine.Bots;

public partial class Playfield
{
    private static readonly IReadOnlyDictionary<CardDB.cardIDEnum, int> EmptyDeckEstimate =
        new ReadOnlyDictionary<CardDB.cardIDEnum, int>(new Dictionary<CardDB.cardIDEnum, int>());

    // Forks share the read-only snapshot. A deck event replaces it for that
    // branch, with the same iteration order used by the classic selectors.
    internal IReadOnlyDictionary<CardDB.cardIDEnum, int> DeckEstimate { get; private set; } = EmptyDeckEstimate;

    private void SeedDeckEstimate(IReadOnlyDictionary<CardDB.cardIDEnum, int> estimate)
    {
        if (estimate.Count == 0)
            return;
        var copy = estimate.Where(card => card.Value > 0).ToDictionary(card => card.Key, card => card.Value);
        DeckEstimate = new ReadOnlyDictionary<CardDB.cardIDEnum, int>(copy);
    }

    private void ChangeEstimatedCount(CardDB.cardIDEnum id, int delta)
    {
        var current = DeckEstimate.TryGetValue(id, out var count) ? count : 0;
        if (current == 0 && delta < 0)
            return;

        var updated = new Dictionary<CardDB.cardIDEnum, int>(DeckEstimate);
        if (current + delta > 0)
            updated[id] = current + delta;
        else
            updated.Remove(id);
        DeckEstimate = new ReadOnlyDictionary<CardDB.cardIDEnum, int>(updated);
    }

    internal CardDB.Card DeckCardForWrite(int index) =>
        ownDeck[index] = ownDeck[index].CopyForSimulation();

    internal void DrawKnownDeckCard(int index, int costReduction = 0)
    {
        DrawTakenDeckCard(TakeKnownDeckCard(index), costReduction);
    }

    private void DrawTakenDeckCard(CardDB.Card card, int costReduction)
    {
        if (costReduction > 0 && owncards.Count < 10)
        {
            card = card.CopyForSimulation();
            card.cost = Math.Max(0, card.cost - costReduction);
        }

        // The selected entry has already left the deck. Use the normal draw
        // path for hand limits, mana cost calculation and cast-on-draw effects.
        DrawCard(card.cardIDenum, true, true, card);
    }

    internal void DrawFromDeckEstimate(CardDB.cardIDEnum id, bool own)
    {
        if (!own || id == CardDB.cardIDEnum.None)
        {
            drawACard(CardDB.cardIDEnum.None, own);
            return;
        }

        var card = TakeCardFromDeck(id);
        if (card is not null)
            DrawTakenDeckCard(card, costReduction: 0);
    }

    internal int DrawMatchingDeckCards(bool own, int count, Predicate<CardDB.Card> matches, int costReduction = 0)
    {
        if (!own)
            return 0;

        var drawn = 0;
        while (drawn < count)
        {
            var card = TakeMatchingDeckCard(matches);
            if (card is null)
                break;
            DrawTakenDeckCard(card, costReduction);
            drawn++;
        }
        return drawn;
    }

    internal int RecruitFromDeck(bool own, int count, Predicate<CardDB.Card> matches = null,
        System.Action<Minion> afterSummon = null)
    {
        if (!own)
            return 0;

        var recruited = 0;
        while (recruited < count && ownMinions.Count < 7)
        {
            var card = TakeMatchingDeckCard(candidate => candidate.type == CardDB.cardtype.MOB &&
                (matches is null || matches(candidate)));
            if (card is null)
                break;

            var minion = callKidAndReturn(card, ownMinions.Count, true);
            recruited++;
            afterSummon?.Invoke(minion);
        }
        return recruited;
    }

    internal void PutSecretsFromDeck(bool own, int count)
    {
        if (own)
        {
            for (var placed = 0; placed < count && ownSecretsIDList.Count < 5; placed++)
            {
                var secret = TakeMatchingDeckCard(card => card.Secret && !ownSecretsIDList.Contains(card.cardIDenum));
                if (secret is null)
                    break;
                ownSecretsIDList.Add(secret.cardIDenum);
            }
        }
        else
        {
            for (var placed = 0; placed < count && enemySecretCount < 5 && enemyDeckSize > 0; placed++)
            {
                enemyDeckSize--;
                enemySecretCount++;
                enemySecretList.Add(Probabilitymaker.Instance.getNewSecretGuessedItem(getNextEntity(), enemyHeroStartClass));
            }
        }
    }

    internal bool HasMatchingDeckCard(Predicate<CardDB.Card> matches) => FindMatchingDeckCard(matches).Card is not null;

    internal static bool MatchesDeckRace(CardDB.Card card, CardDB.Race race) =>
        card.race == race || card.race == CardDB.Race.ALL ||
        (card.races?.Contains(race) ?? false) || (card.races?.Contains(CardDB.Race.ALL) ?? false);

    private (int KnownIndex, CardDB.Card Card) FindMatchingDeckCard(Predicate<CardDB.Card> matches)
    {
        if (ownDeckSize <= 0)
            return (-1, null);

        var index = ownDeck.FindIndex(matches);
        if (index >= 0)
            return (index, ownDeck[index]);
        if (ownDeck.Count >= ownDeckSize)
            return (-1, null);

        foreach (var entry in DeckEstimate)
        {
            var definition = CardDB.Instance.getCardDataFromID(entry.Key);
            if (definition.cardIDenum != entry.Key || !matches(definition))
                continue;

            // The estimate includes the known copies. Only its residual count
            // can provide a separate candidate with the printed attributes.
            if (entry.Value > ownDeck.Count(card => card.cardIDenum == entry.Key))
                return (-1, definition);
        }
        return (-1, null);
    }

    private CardDB.Card TakeMatchingDeckCard(Predicate<CardDB.Card> matches)
    {
        var (index, card) = FindMatchingDeckCard(matches);
        if (index >= 0)
            return TakeKnownDeckCard(index);
        if (card is null)
            return null;

        ChangeEstimatedCount(card.cardIDenum, -1);
        ownDeckSize--;
        return card;
    }

    private CardDB.Card TakeKnownDeckCard(int index)
    {
        var card = ownDeck[index];
        ownDeck.RemoveAt(index);
        if (ownDeckSize > 0)
            ownDeckSize--;
        ChangeEstimatedCount(card.cardIDenum, -1);
        return card;
    }

    private CardDB.Card TakeCardFromDeck(CardDB.cardIDEnum id)
    {
        var index = ownDeck.FindIndex(card => card.cardIDenum == id);
        if (index >= 0)
            return TakeKnownDeckCard(index);
        if (ownDeckSize <= 0 || !DeckEstimate.ContainsKey(id))
            return null;

        var definition = CardDB.Instance.getCardDataFromID(id);
        if (definition.cardIDenum != id)
            return null;

        ChangeEstimatedCount(id, -1);
        ownDeckSize--;
        return definition;
    }

    private void TradeIntoDeck(Handmanager.Handcard card)
    {
        if (ownDeckSize <= 0)
            return;

        mana -= card.card.TradeCost;
        removeCard(card);
        if (ownDeck.Count >= ownDeckSize)
            DrawKnownDeckCard(0);
        else
            drawACard(CardDB.cardIDEnum.None, true);
        ownDeck.Add(card.card);
        ownDeckSize++;
        ChangeEstimatedCount(card.card.cardIDenum, 1);
    }

    private void TrySummonPatches(Handmanager.Handcard played, int position)
    {
        const CardDB.cardIDEnum patchesId = CardDB.cardIDEnum.CFM_637;
        if (!patchesInDeck || ownMinions.Count >= 7 ||
            played.card.race is not (CardDB.Race.PIRATE or CardDB.Race.ALL) ||
            ownMinions.Any(minion => minion.handcard.card.cardIDenum == patchesId) ||
            owncards.Any(card => card.card.cardIDenum == patchesId))
            return;

        var patches = TakeCardFromDeck(patchesId);
        if (patches is null)
            return;

        patchesInDeck = false;
        var minion = callKidAndReturn(patches, position, true);
        if (minion is not null && deckAngrBuff > 0)
            minionGetBuffed(minion, deckAngrBuff, deckHpBuff);
    }

    private bool HasSameKnownDeck(Playfield other)
    {
        if (ownDeck.Count != other.ownDeck.Count ||
            (!ReferenceEquals(DeckEstimate, other.DeckEstimate) && !DeckEstimate.SequenceEqual(other.DeckEstimate)))
            return false;

        for (var index = 0; index < ownDeck.Count; index++)
        {
            var card = ownDeck[index];
            var otherCard = other.ownDeck[index];
            if (!ReferenceEquals(card, otherCard) && KnownDeckValues(card) != KnownDeckValues(otherCard))
                return false;
        }
        return true;
    }

    private ulong MixKnownDeckState(ulong hash)
    {
        if (ownDeck.Count == 0 && DeckEstimate.Count == 0)
            return hash;

        Mix(ownDeck.Count);
        foreach (var card in ownDeck)
        {
            var values = KnownDeckValues(card);
            Mix(values.Id);
            Mix(values.Cost);
            Mix(values.Attack);
            Mix(values.Health);
            Mix(values.FirstTag);
            Mix(values.SecondTag);
        }
        Mix(DeckEstimate.Count);
        foreach (var estimate in DeckEstimate)
        {
            Mix((int)estimate.Key);
            Mix(estimate.Value);
        }
        return hash;

        void Mix(int value) => hash = unchecked((hash ^ (ulong)value) * 1099511628211UL);
    }

    private static (int Id, int Cost, int Attack, int Health, int FirstTag, int SecondTag)
        KnownDeckValues(CardDB.Card card) =>
        ((int)card.cardIDenum, card.cost, card.Attack, card.Health,
            card.TAG_SCRIPT_DATA_NUM_1, card.TAG_SCRIPT_DATA_NUM_2);
}
