using Silverfish.ClassicBridge;
using Silverfish.Contracts;
using Xunit;

namespace Silverfish.ClassicBridge.Tests;

public sealed class ClassicSnapshotFormatterTests
{
    [Fact]
    public void ConvertsKnownLimitedSnapshotIntoClassicChineseBoardFixture()
    {
        var result = ClassicSnapshotFormatter.Format(KnownBoard());
        Assert.Contains("水晶： 1 / 1 [我方英雄] 法师 生命值:( 30 + 0", result);
        Assert.Contains("[我方场面] 肯瑞托法师 ( 4 / 3 )", result);
        Assert.Contains("Enemy cards: 0", result);
        Assert.Contains("Enemy deck: 0", result);
    }

    [Fact]
    public void RejectsUnknownOpponentDeckInsteadOfGuessing()
    {
        Assert.Throws<ArgumentException>(() =>
            ClassicSnapshotFormatter.Format(KnownBoard() with { EnemyDeckCount = null }));
    }

    [Fact]
    public void RejectsSecretsBecauseTheirIdentityIsNotRepresented()
    {
        Assert.Throws<NotSupportedException>(() =>
            ClassicSnapshotFormatter.Format(KnownBoard() with { EnemySecretCount = 1 }));
    }

    [Fact]
    public void RejectsExhaustedMinions()
    {
        var snapshot = KnownBoard();
        Assert.Throws<NotSupportedException>(() =>
            ClassicSnapshotFormatter.Format(snapshot with {
                OurBoard = new[] { snapshot.OurBoard[0] with { CanAttack = false } }
            }));
    }

    [Fact]
    public void RejectsMismatchedClassicSyntheticEntityIds()
    {
        var snapshot = KnownBoard();
        Assert.Throws<NotSupportedException>(() =>
            ClassicSnapshotFormatter.Format(snapshot with {
                OurBoard = new[] { snapshot.OurBoard[0] with { EntityId = 999 } }
            }));
    }

    public static GameSnapshot KnownBoard() => new()
    {
        MatchId = "offline-format-test",
        Sequence = 8,
        Phase = GamePhase.Main,
        IsOurTurn = true,
        AvailableMana = 1,
        MaximumMana = 1,
        OurSecretCount = 0,
        EnemySecretCount = 0,
        EnemyHandCount = 0,
        EnemyDeckCount = 0,
        OurHero = new HeroSnapshot { EntityId = 0, ClassNameZh = "法师", Health = 30 },
        EnemyHero = new HeroSnapshot { EntityId = 1, ClassNameZh = "法师", Health = 3 },
        OurBoard = new[]
        {
            new MinionSnapshot
            {
                EntityId = 11, CardId = "EX1_019", CardNameZh = "肯瑞托法师",
                Attack = 4, Health = 3, CanAttack = true
            }
        },
        LegalActions = new CandidateAction[]
        {
            new() { Id = "attack-face", Kind = GameActionKind.Attack, SourceEntityId = 11, TargetEntityId = 1 },
            new() { Id = "end-turn", Kind = GameActionKind.EndTurn }
        }
    };
}
