using Silverfish.Contracts;

namespace Silverfish.Core;

public static class SnapshotValidator
{
    public static void Validate(GameSnapshot? snapshot)
    {
        if (snapshot is null)
            throw new ArgumentNullException(nameof(snapshot), "必须提供对局快照。");

        if (snapshot.SchemaVersion != 1)
            throw new ArgumentException($"不支持快照格式版本 {snapshot.SchemaVersion}。", nameof(snapshot));

        if (string.IsNullOrWhiteSpace(snapshot.MatchId))
            throw new ArgumentException("快照必须提供对局 ID。", nameof(snapshot));

        if (snapshot.Sequence < 0)
            throw new ArgumentException("快照序号不能为负数。", nameof(snapshot));

        if (snapshot.AvailableMana < 0)
            throw new ArgumentException("可用法力不能为负数。", nameof(snapshot));

        if (snapshot.MaximumMana is < 0 || snapshot.EnemyHandCount is < 0 ||
            snapshot.EnemyDeckCount is < 0 || snapshot.OurSecretCount is < 0 ||
            snapshot.EnemySecretCount is < 0)
            throw new ArgumentException("已填写的对局计数不能为负数。", nameof(snapshot));
        if (snapshot.MaximumMana is int max && snapshot.AvailableMana > max)
            throw new ArgumentException("可用法力不能超过快照中的法力上限。", nameof(snapshot));

        if (snapshot.OurHero is null || snapshot.EnemyHero is null)
            throw new ArgumentException("快照必须提供双方英雄。", nameof(snapshot));

        if (snapshot.OurBoard is null || snapshot.EnemyBoard is null ||
            snapshot.OurHand is null || snapshot.LegalActions is null)
            throw new ArgumentException("快照中的集合必须初始化。", nameof(snapshot));

        if (snapshot.LegalActions.Any(a => a is null ||
            string.IsNullOrWhiteSpace(a.Id) || a.ManaCost < 0))
            throw new ArgumentException("每个合法动作必须有 ID，且法力费用不能为负数。", nameof(snapshot));

        if (snapshot.LegalActions.Select(a => a.Id).Distinct(StringComparer.Ordinal).Count() !=
            snapshot.LegalActions.Count)
            throw new ArgumentException("同一快照中的合法动作 ID 不能重复。", nameof(snapshot));
    }
}
