using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void BatchDiscardClearsTheHandBeforeListenersAndResolvesCardsFromLeftToRight()
    {
        var watcher = Minion("watcher", 0, 2) with
        {
            Triggers = [
                new(TriggerTiming.CardDiscarded, TriggerRelation.Friendly, [new GainArmorEffect(1)])
                {
                    Condition = new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Hand, NumberComparison.Equal, 0)
                },
                new(TriggerTiming.AfterMinionSummoned, TriggerRelation.Friendly, [new GainArmorEffect(2)])
            ]
        };
        var simulator = new GameSimulator([
            Hero, watcher, Spell("clear-hand") with { OnPlay = [new DiscardHandEffect()] },
            Minion("emerge", 2, 3) with { WhenDiscarded = [new SummonDiscardedMinionEffect()] },
            Spell("replacement") with { WhenDiscarded = [new DrawEffect(1)] },
            Spell("drawn")
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Zones = Zones((GameZone.Play, [10, 14]), (GameZone.Hand, [11, 13, 12]), (GameZone.Deck, [15]))
            },
            Player(Second, SecondHero),
            Card(11, "clear-hand", First), Card(12, "replacement", First), Card(13, "emerge", First),
            InPlay(14, "watcher", First, 3, 0, 2), Card(15, "drawn", First));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.Equal(new[] { new EntityId(13), new EntityId(12) },
            result.Events.Where(entry => entry.Kind == SimulationEventKind.CardDiscarded).Select(entry => entry.Source!.Value));
        Assert.Equal(new[] {
            SimulationEventKind.CardPlayed, SimulationEventKind.CardDiscarded, SimulationEventKind.CardDiscarded,
            SimulationEventKind.Triggered, SimulationEventKind.ArmorGained, SimulationEventKind.DiscardedEffect,
            SimulationEventKind.Summoned, SimulationEventKind.Triggered, SimulationEventKind.ArmorGained,
            SimulationEventKind.Triggered, SimulationEventKind.ArmorGained, SimulationEventKind.DiscardedEffect,
            SimulationEventKind.CardDrawn
        }, result.Events.Select(entry => entry.Kind));
        Assert.Equal(GameZone.Play, result.State.LocationOf(new(13)).Zone);
        Assert.Equal(2, result.State.Entities[new(13)].Character!.Attack);
        Assert.Equal(root.NextEntity, result.State.NextEntity);
        Assert.Equal(root.RandomState, result.State.RandomState);
        Assert.Equal(4, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(new EntityId(15), Assert.Single(result.State.Players[First].In(GameZone.Hand)));
        Assert.Equal(GameZone.Hand, root.LocationOf(new(13)).Zone);
    }

    [Theory]
    [InlineData(DiscardSelection.Random, 12, 13, 14, 15)]
    [InlineData(DiscardSelection.HighestCost, 13, 14)]
    [InlineData(DiscardSelection.LowestCost, 12)]
    public void DiscardSelectionUsesLiveCostsAndEnumeratesTies(DiscardSelection selection, params int[] discardedIds)
    {
        var site = new SimulationCard(new("site", EntityKind.Location, 2, PrintedDurability: 2))
        {
            Activation = new([new GainArmorEffect(1)])
        };
        var provider = Minion("provider", 0, 1) with
        {
            CostAuras = [
                new(CostAuraRecipient.Friendly, CostAuraTarget.Hand, -1),
                new(CostAuraRecipient.Friendly, CostAuraTarget.Minion, -4)
            ]
        };
        var simulator = new GameSimulator([
            Hero, site, provider, Minion("discounted", 1, 1, cost: 5),
            new(new("mid", EntityKind.Spell, 3)),
            Spell("discard") with { OnPlay = [new DiscardEffect(Selection: selection)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Zones = Zones((GameZone.Play, [10, 16]), (GameZone.Hand, [11, 12, 13, 14, 15]))
            },
            Player(Second, SecondHero), Card(11, "discard", First), Card(12, "discounted", First),
            Card(13, "mid", First), Card(14, "mid", First), Card(15, "site", First), InPlay(16, "provider", First, 3, 0, 1));

        Assert.Equal(1, simulator.EffectiveCost(root, new(15)));
        var play = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(11));
        var outcomes = simulator.ApplyDistribution(root, play);

        Assert.True(outcomes.IsExact);
        Assert.Equal(discardedIds, outcomes.Outcomes.Select(outcome =>
            Assert.Single(outcome.Result.Events, entry => entry.Kind == SimulationEventKind.CardDiscarded).Source!.Value.Value).Order());
        Assert.All(outcomes.Outcomes, entry => Assert.Equal(1.0 / discardedIds.Length, entry.Probability));
    }

    [Fact]
    public void RandomBatchDiscardEnumeratesEachUnorderedSelectionOnce()
    {
        var simulator = new GameSimulator([
            Hero, Spell("select-two") with { OnPlay = [new DiscardEffect(2)] }, Spell("unit")
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 14, 12, 13])) },
            Player(Second, SecondHero), Card(11, "select-two", First), Card(12, "unit", First),
            Card(13, "unit", First), Card(14, "unit", First));

        var distribution = simulator.ApplyDistribution(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.True(distribution.IsExact);
        Assert.Equal(3, distribution.Outcomes.Length);
        Assert.All(distribution.Outcomes, outcome => Assert.Equal(1.0 / 3, outcome.Probability));
        var pairs = distribution.Outcomes.Select(outcome => string.Join(",", outcome.Result.Events
            .Where(entry => entry.Kind == SimulationEventKind.CardDiscarded).Select(entry => entry.Source!.Value)));
        Assert.Equal(new[] { "14,12", "14,13", "12,13" }, pairs);
    }

    [Fact]
    public void DiscardingTheOpponentsHandDoesNotActivateDiscardEffectsFromTheirDeck()
    {
        var simulator = new GameSimulator([
            Hero, Spell("clear-enemy") with { OnPlay = [new DiscardHandEffect(HandRecipient.Opponent)] },
            Spell("own"), Spell("lost") with { WhenDiscarded = [new GainArmorEffect(1)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Hand, [21]), (GameZone.Deck, [22])) },
            Card(11, "clear-enemy", First), Card(12, "own", First), Card(21, "lost", Second), Card(22, "lost", Second));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        var discarded = Assert.Single(result.Events, entry => entry.Kind == SimulationEventKind.CardDiscarded);
        Assert.Equal(Second, discarded.Player);
        Assert.Equal(new EntityId(21), discarded.Source);
        Assert.Equal(1, result.State.Entities[SecondHero].Character!.Armor);
        Assert.Equal(0, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(GameZone.Hand, result.State.LocationOf(new(12)).Zone);
        Assert.Equal(GameZone.Deck, result.State.LocationOf(new(22)).Zone);
    }
}
