using Silverfish.Contracts;

namespace Silverfish.Core;

public static class SnapshotValidator
{
    public static void Validate(GameSnapshot? snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (snapshot.SchemaVersion != 1)
            throw new ArgumentException("Unsupported snapshot schema version.", nameof(snapshot));

        if (string.IsNullOrWhiteSpace(snapshot.MatchId))
            throw new ArgumentException("MatchId is required.", nameof(snapshot));

        if (snapshot.Sequence < 0)
            throw new ArgumentException("Sequence cannot be negative.", nameof(snapshot));

        if (snapshot.AvailableMana < 0)
            throw new ArgumentException("AvailableMana cannot be negative.", nameof(snapshot));

        if (snapshot.MaximumMana is < 0 || snapshot.EnemyHandCount is < 0 ||
            snapshot.EnemyDeckCount is < 0 || snapshot.OurSecretCount is < 0 ||
            snapshot.EnemySecretCount is < 0)
            throw new ArgumentException("Optional game-state counts cannot be negative.", nameof(snapshot));
        if (snapshot.MaximumMana is int max && snapshot.AvailableMana > max)
            throw new ArgumentException("Available mana cannot exceed maximum mana.", nameof(snapshot));

        if (snapshot.OurHero is null || snapshot.EnemyHero is null)
            throw new ArgumentException("Both hero snapshots are required.", nameof(snapshot));

        if (snapshot.OurBoard is null || snapshot.EnemyBoard is null ||
            snapshot.OurHand is null || snapshot.LegalActions is null)
            throw new ArgumentException("Snapshot collections must not be null.", nameof(snapshot));

        if (snapshot.LegalActions.Any(a => a is null ||
            string.IsNullOrWhiteSpace(a.Id) || a.ManaCost < 0))
            throw new ArgumentException("Every legal action needs an ID and a nonnegative mana cost.", nameof(snapshot));

        if (snapshot.LegalActions.Select(a => a.Id).Distinct(StringComparer.Ordinal).Count() !=
            snapshot.LegalActions.Count)
            throw new ArgumentException("Legal action IDs must be unique within the snapshot.", nameof(snapshot));
    }
}
