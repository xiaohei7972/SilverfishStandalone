using Silverfish.Contracts;
using Silverfish.Core;
using Xunit;

namespace Silverfish.Tests;

public sealed class BaselineStrategyEngineTests
{
    [Fact]
    public async Task PrefersDirectLethalInFixture()
    {
        var snapshot = CreateSnapshot();
        var result = await new BaselineStrategyEngine().DecideAsync(snapshot);

        Assert.NotNull(result.ChosenAction);
        Assert.Equal("attack-hero", result.ChosenAction.Id);
        Assert.Equal(snapshot.Sequence, result.SnapshotSequence);
        Assert.True(result.Score >= 10000);
        Assert.Equal("baseline-v1-not-silverfish", result.Engine);
    }

    [Fact]
    public async Task DoesNotActOnOpponentsTurn()
    {
        var result = await new BaselineStrategyEngine().DecideAsync(
            CreateSnapshot() with { IsOurTurn = false });

        Assert.Null(result.ChosenAction);
        Assert.Null(result.Score);
    }

    [Fact]
    public async Task DoesNotActDuringChoicePhase()
    {
        var result = await new BaselineStrategyEngine().DecideAsync(
            CreateSnapshot() with { Phase = GamePhase.Choice });

        Assert.Null(result.ChosenAction);
    }

    [Fact]
    public async Task HandlesMissingActionCandidatesWithoutGuessing()
    {
        var result = await new BaselineStrategyEngine().DecideAsync(
            CreateSnapshot() with { LegalActions = Array.Empty<CandidateAction>() });

        Assert.Null(result.ChosenAction);
        Assert.Empty(result.Alternatives);
    }

    [Fact]
    public void RejectsDuplicateCandidateIds()
    {
        var action = new CandidateAction { Id = "duplicate", Kind = GameActionKind.EndTurn };
        var snapshot = CreateSnapshot() with { LegalActions = new[] { action, action } };

        Assert.Throws<ArgumentException>(() => SnapshotValidator.Validate(snapshot));
    }

    [Fact]
    public void RejectsUnknownSchemaVersion()
    {
        Assert.Throws<ArgumentException>(() =>
            SnapshotValidator.Validate(CreateSnapshot() with { SchemaVersion = 2 }));
    }

    [Fact]
    public async Task DeterministicallyBreaksScoreTiesById()
    {
        var snapshot = CreateSnapshot() with
        {
            LegalActions = new[]
            {
                new CandidateAction { Id = "z", Kind = GameActionKind.PlayCard, ManaCost = 1 },
                new CandidateAction { Id = "a", Kind = GameActionKind.PlayCard, ManaCost = 1 }
            }
        };

        var result = await new BaselineStrategyEngine().DecideAsync(snapshot);
        Assert.Equal("a", result.ChosenAction?.Id);
    }

    private static GameSnapshot CreateSnapshot() => new()
    {
        MatchId = "test-match",
        Sequence = 7,
        Phase = GamePhase.Main,
        IsOurTurn = true,
        AvailableMana = 2,
        OurHero = new HeroSnapshot { EntityId = 1, Health = 20 },
        EnemyHero = new HeroSnapshot { EntityId = 2, Health = 3 },
        OurBoard = new[]
        {
            new MinionSnapshot { EntityId = 101, CardId = "TEST_001", Attack = 3, Health = 2, CanAttack = true }
        },
        LegalActions = new[]
        {
            new CandidateAction { Id = "end-turn", Kind = GameActionKind.EndTurn },
            new CandidateAction
            {
                Id = "attack-hero",
                Kind = GameActionKind.Attack,
                SourceEntityId = 101,
                TargetEntityId = 2
            },
            new CandidateAction { Id = "use-power", Kind = GameActionKind.HeroPower, ManaCost = 2 }
        }
    };
}
