using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void WitchyLackeyUsesTheEligibleCostPoolAndPreservesEntityIdentity()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero, Minion("base", 1, 3, cost: 1),
            Minion("two-a", 2, 2, cost: 2), Minion("two-b", 1, 4, cost: 2),
            Minion("off-pool", 3, 3, cost: 2), Minion("wrong-cost", 4, 4, cost: 3)
        ], randomMinionPool: ["two-b", "wrong-cost", "two-a", "two-b"]);
        var source = new EntityId(11);
        var target = new EntityId(12);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "DAL_615", First),
            InPlay(12, "base", First, 3, 1, 3) with { Character = new(1, 3, Damage: 1), Stealthed = true },
            InPlay(21, "base", Second, 4, 1, 3));
        var play = new SimulationAction(SimulationActionKind.PlayCard, First, source, target, Position: 0);
        Assert.Equal([target], simulator.LegalActions(root).Where(action => action.Source == source).Select(action => action.Target).Distinct());
        Assert.Equal(ErrorType.REQ_FRIENDLY_TARGET, simulator.CheckAction(root, play with { Target = new EntityId(21) })?.Type);

        var outcomes = simulator.ApplyDistribution(root, play);
        Assert.True(outcomes.IsExact);
        Assert.Equal(["two-a", "two-b"], outcomes.Outcomes.Select(group => group.Result.State.Entities[target].CardId));
        Assert.All(outcomes.Outcomes, outcome =>
        {
            Assert.Equal(0.5, outcome.Probability);
            var transformed = outcome.Result.State.Entities[target];
            Assert.Equal(3, transformed.PlayOrder);
            Assert.Equal(First, transformed.Controller);
            Assert.Equal(0, transformed.Character!.Damage);
            Assert.Null(transformed.Stealthed);
            Assert.Equal(target, Assert.Single(outcome.Result.Events, change => change.Kind == SimulationEventKind.Transformed).Target);
            Assert.Empty(outcome.Result.State.MinionDeaths);
            Assert.DoesNotContain(outcome.Result.Events, change =>
                change.Kind == SimulationEventKind.Summoned && change.Source == target);
        });
    }

    [Theory]
    [InlineData("REV_923", 6, "seven", 7)]
    [InlineData("REV_923", 7, "seven", 7)]
    [InlineData("REV_798", 10, "ten", 10)]
    [InlineData("REV_798", 12, "twelve", 12)]
    [InlineData("REV_798", 13, "base", 13)]
    public void MuckPoolsUsesTheNearestAllowedCostAndPreservesOverTenCosts(string cardId, int originalCost,
        string expected, int expectedCost)
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero, Minion("base", 1, 3, cost: originalCost),
            Minion("seven", 2, 2, cost: 7), Minion("ten", 3, 3, cost: 10), Minion("twelve", 4, 4, cost: 12)
        ], randomMinionPool: ["seven", "ten", "twelve"]);
        var source = new EntityId(11);
        var target = new EntityId(12);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11, 12])) },
            Player(Second, SecondHero),
            Card(11, cardId, First) with { PlayOrder = 3, Location = new(3) },
            InPlay(12, "base", First, 4, 1, 3) with { Character = new(1, 3, Damage: 1) });
        var use = new SimulationAction(SimulationActionKind.ActivateLocation, First, source, target);

        Assert.Equal(use, Assert.Single(simulator.LegalActions(root), action => action.Source == source));
        var applied = simulator.Apply(root, use);
        var changed = applied.State.Entities[target];
        Assert.Equal(expected, changed.CardId);
        Assert.Equal(expectedCost, simulator.Cards[changed.CardId].Definition.PrintedCost);
        Assert.Equal(0, changed.Character!.Damage);
        Assert.Equal(new LocationState(2, 2), applied.State.Entities[source].Location);
    }

    [Fact]
    public void DefaultEvolvePoolUsesCompiledCardCostsAndCanRestoreTheResult()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, Minion("base", 1, 3, cost: 9)]);
        var source = new EntityId(11);
        var target = new EntityId(12);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero), Card(11, "DAL_615", First), InPlay(12, "base", First, 3, 1, 3));
        var transformed = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, source, target, Position: 0)).State;
        var cardId = transformed.Entities[target].CardId;

        Assert.Equal(10, CompiledCardData.Bind(cardId, new()).Definition.PrintedCost);
        Assert.Equal(target, Assert.Single(transformed.Players[First].In(GameZone.Play), id => id == target));
        var fresh = new GameSimulator(simulator.Cards.Values);
        var restored = fresh.RestoreGame(transformed.Players.Values, transformed.Entities.Values, transformed.ActivePlayer,
            transformed.Turn, transformed.MinionDeaths, transformed.RandomState,
            new(transformed.NextEntity, transformed.NextPlayOrder, transformed.Outcome, transformed.Winner));
        Assert.Equal(cardId, restored.Entities[target].CardId);
    }
}
