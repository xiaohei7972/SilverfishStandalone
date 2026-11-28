using System.Globalization;
using System.Text;
using Silverfish.Contracts;
using Silverfish.Core;

namespace Silverfish.ClassicBridge;

/// <summary>
/// Restricted offline serializer for the classic Chinese BoardTester fixture
/// format. This is NOT a complete live snapshot adapter.
/// </summary>
public static class ClassicSnapshotFormatter
{
    public static string Format(GameSnapshot snapshot)
    {
        SnapshotValidator.Validate(snapshot);

        if (snapshot.Phase != GamePhase.Main || !snapshot.IsOurTurn)
            throw new NotSupportedException("Classic offline board formatting requires our main turn.");

        if (snapshot.MaximumMana is null || snapshot.MaximumMana < snapshot.AvailableMana)
            throw new ArgumentException("A valid maximum mana count is required.");
        if (snapshot.EnemyHandCount is null || snapshot.EnemyDeckCount is null ||
            snapshot.EnemyHandCount < 0 || snapshot.EnemyDeckCount < 0)
            throw new ArgumentException("Enemy hand and deck counts must be explicit.");
        if (snapshot.OurSecretCount != 0 || snapshot.EnemySecretCount != 0)
            throw new NotSupportedException("This fixture adapter only supports explicitly verified zero secrets.");
        if (snapshot.OurHand.Count != 0)
            throw new NotSupportedException("Hand card conversion is not yet supported.");
        if (snapshot.OurBoard.Count > 7 || snapshot.EnemyBoard.Count > 7)
            throw new ArgumentException("The board cannot contain more than seven minions per player.");
        if (snapshot.OurHero.EntityId != 0 || snapshot.EnemyHero.EntityId != 1)
            throw new NotSupportedException("Classic BoardTester fixture hero IDs must be 0 and 1.");
        if (snapshot.OurHero.Attack != 0 || snapshot.EnemyHero.Attack != 0)
            throw new NotSupportedException("This fixture adapter cannot express hero attack or weapons.");
        if (snapshot.OurHero.Health <= 0 || snapshot.EnemyHero.Health <= 0 ||
            snapshot.OurHero.Armor < 0 || snapshot.EnemyHero.Armor < 0)
            throw new ArgumentException("Both heroes must have valid health and armor.");

        ValidateToken(snapshot.OurHero.ClassNameZh, "Our hero class");
        ValidateToken(snapshot.EnemyHero.ClassNameZh, "Enemy hero class");
        ValidateMinions(snapshot.OurBoard, true);
        ValidateMinions(snapshot.EnemyBoard, false);

        var sb = new StringBuilder();
        sb.Append("水晶： ").Append(snapshot.AvailableMana)
            .Append(" / ").Append(snapshot.MaximumMana)
            .Append(" [我方英雄] ").Append(snapshot.OurHero.ClassNameZh)
            .Append(" 生命值:( ").Append(snapshot.OurHero.Health)
            .Append(" + ").Append(snapshot.OurHero.Armor)
            .Append(" 奥秘数: 0 ) [敌方英雄] ").Append(snapshot.EnemyHero.ClassNameZh)
            .Append(" 生命值:( ").Append(snapshot.EnemyHero.Health)
            .Append(" + ").Append(snapshot.EnemyHero.Armor)
            .Append(" 奥秘数: 0 )")
            .AppendLine();

        AppendMinions(sb, "[敌方场面]", snapshot.EnemyBoard);
        AppendMinions(sb, "[我方场面]", snapshot.OurBoard);

        sb.Append("Enemy cards: ").Append(snapshot.EnemyHandCount).AppendLine();
        sb.Append("Enemy deck: ").Append(snapshot.EnemyDeckCount).AppendLine();
        return sb.ToString();
    }

    private static void ValidateMinions(IReadOnlyList<MinionSnapshot> minions, bool own)
    {
        for (int i = 0; i < minions.Count; i++)
        {
            var minion = minions[i] ?? throw new ArgumentException("Null minion.");
            ValidateToken(minion.CardNameZh, "Minion's localized name");
            if (minion.Attack < 0 || minion.Health <= 0)
                throw new ArgumentException("Minion statistics must be valid.");
            if (minion.HasTaunt || minion.HasDivineShield)
                throw new NotSupportedException("Taunt and divine shield require a richer state importer.");
            // The classic Chinese BoardTester sets every own minion Ready=true,
            // so exhausted minions must be rejected rather than misrepresented.
            if (own && !minion.CanAttack)
                throw new NotSupportedException("Exhausted minions are not representable by this fixture adapter.");
            int expectedId = (i + 1) * (own ? 10 : 100) + (i + 1);
            if (minion.EntityId != expectedId)
                throw new NotSupportedException("Classic BoardTester assigns synthetic entity IDs by position.");
        }
    }

    private static void ValidateToken(string value, string field)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Any(char.IsWhiteSpace))
            throw new ArgumentException($"{field} must be a single nonempty Chinese BoardTester token.");
    }

    private static void AppendMinions(
        StringBuilder sb, string heading, IReadOnlyList<MinionSnapshot> minions)
    {
        if (minions.Count == 0) return;
        sb.Append(heading);
        foreach (var minion in minions)
        {
            sb.Append(' ').Append(minion.CardNameZh)
                .Append(" ( ").Append(minion.Attack)
                .Append(" / ").Append(minion.Health).Append(" )");
        }
        sb.AppendLine();
    }
}
