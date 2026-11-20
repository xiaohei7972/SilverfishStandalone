using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void RestoreKeepsAllocationCountersAboveTheSavedEntities()
    {
        var simulator = new GameSimulator([
            Hero, Minion("unit", 1, 1), Spell("deploy") with { OnPlay = [new SummonEffect("unit")] }
        ]);
        var initial = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero), Card(11, "deploy", First));
        var saved = new GameContinuation(100, 500, MatchOutcome.InProgress, null);

        var state = Restore(simulator, initial, saved);
        var result = simulator.Apply(state, new(SimulationActionKind.PlayCard, First, new EntityId(11)));
        var unit = result.State.Entities[new EntityId(100)];

        Assert.Equal(500, result.State.Entities[new EntityId(11)].PlayOrder);
        Assert.Equal(501, unit.PlayOrder);
        Assert.Equal(101, result.State.NextEntity);
        Assert.Equal(502, result.State.NextPlayOrder);
        Assert.Throws<ArgumentException>(() => Restore(simulator, initial, saved with { NextEntity = 11 }));
        Assert.Throws<ArgumentException>(() => Restore(simulator, initial, saved with { NextPlayOrder = 2 }));
    }

    [Fact]
    public void RestoresAKnownWinnerAndRejectsAContradictoryOutcome()
    {
        var simulator = new GameSimulator([
            Hero, Spell("finish") with { OnPlay = [new DamageEffect(DamageTarget.EnemyHero, 30)] }
        ]);
        var initial = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero), Card(11, "finish", First));
        var finished = simulator.Apply(initial, new(SimulationActionKind.PlayCard, First, new EntityId(11))).State;
        var saved = new GameContinuation(finished.NextEntity, finished.NextPlayOrder, finished.Outcome, finished.Winner);

        var resumed = Restore(simulator, finished, saved);
        Assert.Equal(MatchOutcome.Won, resumed.Outcome);
        Assert.Equal(First, resumed.Winner);
        Assert.Equal(GameZone.Graveyard, resumed.LocationOf(SecondHero).Zone);
        Assert.Empty(simulator.LegalActions(resumed));
        Assert.Throws<ArgumentException>(() => Restore(simulator, finished, saved with { Winner = Second }));
    }

    private static GameState Restore(GameSimulator simulator, GameState state, GameContinuation continuation) =>
        simulator.RestoreGame(state.Players.Values, state.Entities.Values, state.ActivePlayer, state.Turn,
            state.MinionDeaths, state.RandomState, continuation);
}
