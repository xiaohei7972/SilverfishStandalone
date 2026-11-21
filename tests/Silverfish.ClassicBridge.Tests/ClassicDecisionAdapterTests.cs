using Silverfish.ClassicBridge;
using Silverfish.Contracts;
using HREngine.Bots;
using Xunit;
using ClassicAction = HREngine.Bots.Action;

namespace Silverfish.ClassicBridge.Tests;

public class ClassicDecisionAdapterTests
{
    [Fact]
    public void EndTurnMatchesOnlyKnownLegalAction()
    {
        var snapshot = BaseSnapshot() with
        {
            LegalActions = new[] { new CandidateAction { Id = "end", Kind = GameActionKind.EndTurn } }
        };
        var move = new ClassicAction(actionEnum.endturn, null!, null!, 0, null!, 0, 0);
        var decision = ClassicDecisionAdapter.Match(snapshot, move, -1);
        Assert.Equal("end", decision.ChosenAction?.Id);
        Assert.Equal("classic-silverfish", decision.Engine);
        Assert.Equal(31, decision.SnapshotSequence);
    }

    [Fact]
    public void AmbiguousCandidatesAreRejected()
    {
        var snapshot = BaseSnapshot() with
        {
            LegalActions = new[]
            {
                new CandidateAction { Id = "end-1", Kind = GameActionKind.EndTurn },
                new CandidateAction { Id = "end-2", Kind = GameActionKind.EndTurn }
            }
        };
        var move = new ClassicAction(actionEnum.endturn, null!, null!, 0, null!, 0, 0);
        Assert.Null(ClassicDecisionAdapter.Match(snapshot, move, 0).ChosenAction);
    }

    [Fact]
    public void UnknownActionDoesNotProduceRecommendation()
    {
        var snapshot = BaseSnapshot();
        var move = new ClassicAction(actionEnum.attackWithMinion, null!, null!, 0, null!, 0, 0);
        Assert.Null(ClassicDecisionAdapter.Match(snapshot, move, 0).ChosenAction);
    }

    [Fact]
    public void DoesNotRecommendOnOpponentsTurn()
    {
        var snapshot = BaseSnapshot() with { IsOurTurn = false };
        var move = new ClassicAction(actionEnum.endturn, null!, null!, 0, null!, 0, 0);
        Assert.Null(ClassicDecisionAdapter.Match(snapshot, move, 0).ChosenAction);
    }

    private static GameSnapshot BaseSnapshot() => new()
    {
        MatchId = "offline-classic-board",
        Sequence = 31,
        Phase = GamePhase.Main,
        IsOurTurn = true,
        OurHero = new HeroSnapshot { EntityId = 1, Health = 30 },
        EnemyHero = new HeroSnapshot { EntityId = 2, Health = 3 }
    };
}
