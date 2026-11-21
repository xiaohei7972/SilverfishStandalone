using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void CreatedCardsGoToTheRequestedPlayerAndZoneWithFreshEntityIds()
    {
        var simulator = new GameSimulator([Hero, Spell("plain"), Spell("payload"), Spell("place") with
        {
            OnPlay = [
                new CreateCardEffect("payload", CardPlacement.DeckTop),
                new CreateCardEffect("payload", CardPlacement.DeckBottom, CardRecipient.Opponent),
                new CreateCardEffect("payload", Recipient: CardRecipient.Opponent)
            ]
        }]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]), (GameZone.Deck, [21])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Deck, [22, 23])) },
            Card(11, "place", First), Card(21, "plain", First), Card(22, "plain", Second), Card(23, "plain", Second));
        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.Equal(new[] { 24, 21 }, result.State.Players[First].In(GameZone.Deck).Select(id => id.Value));
        Assert.Equal(new[] { 22, 23, 25 }, result.State.Players[Second].In(GameZone.Deck).Select(id => id.Value));
        Assert.Equal(new EntityId(26), Assert.Single(result.State.Players[Second].In(GameZone.Hand)));
        Assert.Equal(new EntityLocation(First, GameZone.Deck), result.State.LocationOf(new(24)));
        Assert.Equal(new EntityLocation(Second, GameZone.Deck), result.State.LocationOf(new(25)));
        Assert.Equal(new EntityLocation(Second, GameZone.Hand), result.State.LocationOf(new(26)));
        Assert.Equal(Second, result.State.Entities[new(26)].Owner);
        Assert.Equal(Second, result.State.Entities[new(26)].Controller);
        Assert.Null(result.State.Entities[new(26)].PlayOrder);
        Assert.Equal(27, result.State.NextEntity);
        Assert.Equal(root.RandomState, result.State.RandomState);
        Assert.Equal(new[] { SimulationEventKind.CardPlayed, SimulationEventKind.CardAddedToDeck,
            SimulationEventKind.CardAddedToDeck, SimulationEventKind.CardGenerated }, result.Events.Select(entry => entry.Kind));
    }

    [Theory]
    [InlineData(CardPlacement.RandomDeckPosition, 1, 3)]
    [InlineData(CardPlacement.ShuffleIntoDeck, 1, 6)]
    [InlineData(CardPlacement.ShuffleIntoDeck, 2, 24)]
    public void RandomInsertionAndDeckShuffleEnumerateTheirDifferentDeckOrders(CardPlacement placement, int count, int permutations)
    {
        var simulator = new GameSimulator([
            Hero, Spell("plain"), Spell("payload"),
            Spell("place") with { OnPlay = [new CreateCardEffect("payload", placement, CardRecipient.Opponent, count)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Deck, [21, 22])) },
            Card(11, "place", First), Card(21, "plain", Second), Card(22, "plain", Second));
        var action = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(11));
        var distribution = simulator.ApplyDistribution(root, action);

        Assert.True(distribution.IsExact);
        Assert.Equal(permutations, distribution.Outcomes.Length);
        var orders = distribution.Outcomes.Select(outcome =>
            outcome.Result.State.Players[Second].In(GameZone.Deck).Select(id => id.Value).ToArray()).ToArray();
        Assert.Equal(permutations, orders.Select(order => string.Join(",", order)).Distinct().Count());
        Assert.All(distribution.Outcomes, outcome =>
        {
            Assert.Equal(1.0 / permutations, outcome.Probability, precision: 10);
            Assert.Equal(count, outcome.Result.Events.Count(entry => entry.Kind == SimulationEventKind.CardAddedToDeck));
            Assert.Equal(placement == CardPlacement.ShuffleIntoDeck ? 1 : 0,
                outcome.Result.Events.Count(entry => entry.Kind == SimulationEventKind.DeckShuffled));
        });
        if (placement == CardPlacement.RandomDeckPosition)
            Assert.All(orders, order => Assert.Equal(new[] { 21, 22 }, order.Where(id => id is 21 or 22)));
        else
            Assert.Contains(orders, order => order.Where(id => id is 21 or 22).SequenceEqual(new[] { 22, 21 }));

        var first = simulator.Apply(root, action);
        var repeated = simulator.Apply(root, action);
        Assert.Equal(first.State.Players[Second].In(GameZone.Deck), repeated.State.Players[Second].In(GameZone.Deck).AsEnumerable());
        Assert.NotEqual(root.RandomState, first.State.RandomState);
        Assert.Equal(new[] { 21, 22 }, root.Players[Second].In(GameZone.Deck).Select(id => id.Value));
    }

    [Fact]
    public void GeneratingToAFullHandDoesNotAllocateExtraEntitiesOrActivateDrawEffects()
    {
        var simulator = new GameSimulator([
            Hero, Spell("plain"), Spell("payload") with { WhenDrawn = [new GainArmorEffect(5)] },
            Spell("place") with { OnPlay = [new CreateCardEffect("payload", Count: 3)] }
        ]);
        var hand = Enumerable.Range(31, 9).ToArray();
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, .. hand])) },
            Player(Second, SecondHero), [Card(11, "place", First), .. hand.Select(id => Card(id, "plain", First))]);
        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.Equal(10, result.State.Players[First].In(GameZone.Hand).Length);
        Assert.Equal(root.NextEntity + 1, result.State.NextEntity);
        Assert.Equal(new EntityId(root.NextEntity), Assert.Single(result.Events, entry => entry.Kind == SimulationEventKind.CardGenerated).Target);
        Assert.Equal(0, result.State.Entities[FirstHero].Character!.Armor);
    }

    [Fact]
    public void NewSpellsAtTheTopCanRunAnAutomaticDrawChain()
    {
        var payload = Spell("payload") with
        {
            Keywords = CardKeywords.CastsWhenDrawn,
            OnPlay = [new DamageEffect(DamageTarget.EnemyHero, 1)]
        };
        var simulator = new GameSimulator([Hero, payload, Spell("plain"), Spell("place") with
        {
            OnPlay = [new CreateCardEffect("payload", CardPlacement.DeckTop, Count: 2), new DrawEffect(1)]
        }]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]), (GameZone.Deck, [21])) },
            Player(Second, SecondHero), Card(11, "place", First), Card(21, "plain", First));
        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.Equal(new EntityId?[] { new(23), new(22) }, result.Events
            .Where(entry => entry.Kind == SimulationEventKind.SpellCastWhenDrawn).Select(entry => entry.Source));
        Assert.Equal(28, result.State.Entities[SecondHero].Character!.Health);
        Assert.Empty(result.State.Players[First].In(GameZone.Deck));
        Assert.Equal(new EntityId(21), Assert.Single(result.State.Players[First].In(GameZone.Hand)));
    }
}
