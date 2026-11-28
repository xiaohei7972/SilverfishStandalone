using System.Text.Json;
using Xunit;
using Silverfish.Domain;
using Silverfish.Simulation;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void JadeGolemCountScalesAndSurvivesSaveRestore()
    {
        var jadeSpell = Spell("jade-test") with { OnPlay = [new SummonJadeGolemEffect()] };
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, jadeSpell]);
        var initial = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 10,
                ManaCrystals = 10,
                MaximumManaCrystals = 10,
                Zones = Zones((GameZone.Play, [FirstHero.Value]), (GameZone.Hand, [11, 12, 13]))
            },
            Player(Second, SecondHero),
            Card(11, "jade-test", First), Card(12, "jade-test", First), Card(13, "jade-test", First));

        var first = simulator.Apply(initial, new(SimulationActionKind.PlayCard, First, new EntityId(11))).State;
        Assert.Equal(1, first.Players[First].JadeGolemsSummoned);
        AssertJadeGolem(first, 1);

        var second = simulator.Apply(first, new(SimulationActionKind.PlayCard, First, new EntityId(12))).State;
        Assert.Equal(2, second.Players[First].JadeGolemsSummoned);
        AssertJadeGolem(second, 2);

        var savedPlayers = JsonSerializer.Deserialize<PlayerState[]>(JsonSerializer.Serialize(second.Players.Values))!;
        var savedEntities = JsonSerializer.Deserialize<GameEntity[]>(JsonSerializer.Serialize(second.Entities.Values))!;
        var continuation = new GameContinuation(second.NextEntity, second.NextPlayOrder, second.Outcome, second.Winner);
        var restored = simulator.RestoreGame(savedPlayers, savedEntities, second.ActivePlayer, second.Turn,
            second.MinionDeaths, second.RandomState, continuation);

        var third = simulator.Apply(restored, new(SimulationActionKind.PlayCard, First, new EntityId(13))).State;
        Assert.Equal(3, third.Players[First].JadeGolemsSummoned);
        AssertJadeGolem(third, 3);
    }

    [Fact]
    public void JadeGolemCountDoesNotIncreaseWhenTheBoardIsFull()
    {
        var jadeSpell = Spell("jade-test") with { OnPlay = [new SummonJadeGolemEffect()] };
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, jadeSpell]);
        var minions = Enumerable.Range(30, 7)
            .Select(id => InPlay(id, "CFM_712_t01", First, id, 1, 1)).ToArray();
        var initial = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 10,
                ManaCrystals = 10,
                MaximumManaCrystals = 10,
                Zones = Zones((GameZone.Play, [FirstHero.Value, .. minions.Select(minion => minion.Id.Value)]),
                    (GameZone.Hand, [11]))
            },
            Player(Second, SecondHero),
            [Card(11, "jade-test", First), .. minions]);

        var result = simulator.Apply(initial, new(SimulationActionKind.PlayCard, First, new EntityId(11))).State;

        Assert.Equal(0, result.Players[First].JadeGolemsSummoned);
        Assert.Equal(7, result.Players[First].In(GameZone.Play)
            .Count(id => result.Entities[id].CardId == "CFM_712_t01"));
    }

    private static void AssertJadeGolem(GameState state, int expected)
    {
        var golem = state.Players[First].In(GameZone.Play)
            .Select(id => state.Entities[id])
            .Single(entity => entity.CardId == "CFM_712_t01" &&
                entity.Character is { Attack: var attack, MaximumHealth: var health } &&
                attack == expected && health == expected);
        Assert.Equal(expected, golem.Character!.Health);
    }
}
