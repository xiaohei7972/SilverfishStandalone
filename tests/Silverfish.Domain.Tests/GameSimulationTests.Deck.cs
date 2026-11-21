using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void FilteredDrawEnumeratesEligibleCardsAndPreservesTheRestOfTheDeckOrder()
    {
        var filter = new CardFilter { Kind = EntityKind.Spell, SpellSchool = SpellSchool.Fire, MaximumPrintedCost = 3 };
        var warm = new SimulationCard(new("warm", EntityKind.Spell, 2) { SpellSchool = SpellSchool.Fire })
        {
            WhenDrawn = [new GainArmorEffect(1)]
        };
        var simulator = new GameSimulator([
            Hero, Spell("lookup") with { OnPlay = [new DrawMatchingEffect(filter)] }, warm,
            new(new("expensive", EntityKind.Spell, 4) { SpellSchool = SpellSchool.Fire }), Spell("plain")
        ]);
        var order = new[] { 21, 22, 23, 24, 25 };
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]), (GameZone.Deck, order)) },
            Player(Second, SecondHero), Card(11, "lookup", First), Card(21, "plain", First), Card(22, "warm", First),
            Card(23, "expensive", First), Card(24, "plain", First), Card(25, "warm", First));
        var action = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(11));

        var distribution = simulator.ApplyDistribution(root, action);
        Assert.True(distribution.IsExact);
        Assert.Equal(2, distribution.Outcomes.Length);
        Assert.Equal(new[] { 22, 25 }, distribution.Outcomes.Select(outcome =>
            Assert.Single(outcome.Result.State.Players[First].In(GameZone.Hand)).Value));
        Assert.All(distribution.Outcomes, outcome =>
        {
            var drawn = Assert.Single(outcome.Result.State.Players[First].In(GameZone.Hand));
            Assert.Equal(0.5, outcome.Probability);
            Assert.Equal(order.Where(id => id != drawn.Value), outcome.Result.State.Players[First].In(GameZone.Deck).Select(id => id.Value));
            Assert.Equal(1, outcome.Result.State.Entities[FirstHero].Character!.Armor);
            Assert.Equal(new[] { SimulationEventKind.CardPlayed, SimulationEventKind.CardDrawn,
                SimulationEventKind.DrawnEffect, SimulationEventKind.ArmorGained }, outcome.Result.Events.Select(entry => entry.Kind));
        });
    }

    [Fact]
    public void FilteredDrawBurnsTheEligibleCardAtHandLimitAndStopsWithoutFatigue()
    {
        var simulator = new GameSimulator([
            Hero, Spell("lookup") with { OnPlay = [new DrawMatchingEffect(new() { Kind = EntityKind.Minion }, Count: 5)] },
            Minion("unit", 1, 1) with { WhenDrawn = [new GainArmorEffect(1)] }, Spell("plain")
        ]);
        var hand = Enumerable.Range(31, 9).ToArray();
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, .. hand]), (GameZone.Deck, [21, 22])) },
            Player(Second, SecondHero),
            [Card(11, "lookup", First), Card(21, "unit", First), Card(22, "unit", First), ..hand.Select(id => Card(id, "plain", First))]);

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));
        Assert.Equal(new[] { SimulationEventKind.CardPlayed, SimulationEventKind.CardDrawn, SimulationEventKind.DrawnEffect,
            SimulationEventKind.ArmorGained, SimulationEventKind.CardBurned }, result.Events.Select(entry => entry.Kind));
        Assert.Empty(result.State.Players[First].In(GameZone.Deck));
        Assert.Equal(10, result.State.Players[First].In(GameZone.Hand).Length);
        Assert.Equal(0, result.State.Players[First].Fatigue);
        Assert.Equal(1, result.State.Entities[FirstHero].Character!.Armor);
    }

    [Fact]
    public void RecruitmentMovesTheExistingEntityAndChecksTheDeckAgainAfterSummonListeners()
    {
        var filter = new CardFilter { Kind = EntityKind.Minion, MinionType = MinionType.Beast, MaximumPrintedCost = 2 };
        var beast = new SimulationCard(new("beast", EntityKind.Minion, 2, 2, 3) { MinionTypes = MinionType.Beast })
        {
            OnPlay = [new GainArmorEffect(5)],
            WhenDrawn = [new GainArmorEffect(1)]
        };
        var watcher = Minion("watcher", 0, 2) with
        {
            Triggers = [new(TriggerTiming.AfterMinionSummoned, TriggerRelation.Friendly, [new DrawMatchingEffect(filter)])]
        };
        var simulator = new GameSimulator([
            Hero, beast, watcher, Spell("plain"),
            Spell("search") with { OnPlay = [new RecruitEffect(2) { CardFilter = filter }] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11]), (GameZone.Deck, [21, 22, 23])) },
            Player(Second, SecondHero), Card(11, "search", First), InPlay(12, "watcher", First, 3, 0, 2),
            Card(21, "plain", First), Card(22, "beast", First), Card(23, "beast", First));

        var distribution = simulator.ApplyDistribution(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));
        Assert.True(distribution.IsExact);
        Assert.Equal(2, distribution.Outcomes.Length);
        Assert.All(distribution.Outcomes, outcome =>
        {
            var result = outcome.Result;
            var recruited = Assert.Single(result.Events, entry => entry.Kind == SimulationEventKind.Recruited).Source!.Value;
            var drawn = Assert.Single(result.State.Players[First].In(GameZone.Hand));
            Assert.NotEqual(recruited, drawn);
            Assert.Equal(GameZone.Play, result.State.LocationOf(recruited).Zone);
            Assert.Equal(root.NextEntity, result.State.NextEntity);
            Assert.Equal(root.NextPlayOrder + 1, result.State.Entities[recruited].PlayOrder);
            Assert.Equal(1, result.State.Entities[FirstHero].Character!.Armor);
            Assert.Equal(new EntityId(21), Assert.Single(result.State.Players[First].In(GameZone.Deck)));
            Assert.Equal(new[] { SimulationEventKind.CardPlayed, SimulationEventKind.Summoned, SimulationEventKind.Recruited,
                SimulationEventKind.Triggered, SimulationEventKind.CardDrawn, SimulationEventKind.DrawnEffect,
                SimulationEventKind.ArmorGained }, result.Events.Select(entry => entry.Kind));
        });
    }

    [Fact]
    public void RecruitmentStopsOnceTheSharedBoardIsFull()
    {
        var site = new SimulationCard(new("site", EntityKind.Location, 0, PrintedDurability: 1))
        {
            Activation = new([new GainArmorEffect(1)])
        };
        var simulator = new GameSimulator([
            Hero, site, Minion("unit", 1, 2), Spell("search") with { OnPlay = [new RecruitEffect(2)] }
        ]);
        var fillers = Enumerable.Range(12, 5).ToArray();
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Zones = Zones((GameZone.Play, [10, 17, ..fillers]), (GameZone.Hand, [11]), (GameZone.Deck, [21, 22]))
            },
            Player(Second, SecondHero),
            [Card(11, "search", First), Card(17, "site", First) with { PlayOrder = 3, Location = new(1) },
             Card(21, "unit", First), Card(22, "unit", First), ..fillers.Select((id, i) => InPlay(id, "unit", First, 4 + i, 1, 2))]);
        var distribution = simulator.ApplyDistribution(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.True(distribution.IsExact);
        Assert.Equal(2, distribution.Outcomes.Length);
        Assert.All(distribution.Outcomes, outcome =>
        {
            Assert.Single(outcome.Result.Events, entry => entry.Kind == SimulationEventKind.Recruited);
            Assert.Single(outcome.Result.State.Players[First].In(GameZone.Deck));
            Assert.Equal(8, outcome.Result.State.Players[First].In(GameZone.Play).Length);
        });
    }
}
