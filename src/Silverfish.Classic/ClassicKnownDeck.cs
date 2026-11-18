namespace HREngine.Bots;

public partial class Playfield
{
    internal CardDB.Card DeckCardForWrite(int index) =>
        ownDeck[index] = ownDeck[index].CopyForSimulation();

    internal void DrawKnownDeckCard(int index, int costReduction = 0)
    {
        var card = ownDeck[index];
        ownDeck.RemoveAt(index);
        if (ownDeckSize > 0)
            ownDeckSize--;

        if (costReduction > 0 && owncards.Count < 10)
        {
            card = card.CopyForSimulation();
            card.cost = Math.Max(0, card.cost - costReduction);
        }

        // The selected entry has already left the deck. Use the normal draw
        // path for hand limits, mana cost calculation and cast-on-draw effects.
        DrawCard(card.cardIDenum, true, true, card);
    }

    private bool HasSameKnownDeck(Playfield other)
    {
        if (ownDeck.Count != other.ownDeck.Count)
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
        if (ownDeck.Count == 0)
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
        return hash;

        void Mix(int value) => hash = unchecked((hash ^ (ulong)value) * 1099511628211UL);
    }

    private static (int Id, int Cost, int Attack, int Health, int FirstTag, int SecondTag)
        KnownDeckValues(CardDB.Card card) =>
        ((int)card.cardIDenum, card.cost, card.Attack, card.Health,
            card.TAG_SCRIPT_DATA_NUM_1, card.TAG_SCRIPT_DATA_NUM_2);
}
