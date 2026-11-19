using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void ArcaneMissilesEnumeratesWeightedPathsAndStopsSelectingAMortallyWoundedMinion()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var spell = new EntityId(11);
        var wisp = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "EX1_277", First), InPlay(21, "CS2_231", Second, 3, 1, 1));
        var action = new SimulationAction(SimulationActionKind.PlayCard, First, spell);

        var outcomes = simulator.ApplyDistribution(root, action);
        Assert.True(outcomes.IsExact);
        Assert.Equal(4, outcomes.Outcomes.Length);
        Assert.Equal(1.0, outcomes.Outcomes.Sum(outcome => outcome.Probability));
        Assert.Equal(0.125, outcomes.Outcomes.Single(outcome => outcome.Result.State.LocationOf(wisp).Zone == GameZone.Play).Probability);
        Assert.Equal(27.875, outcomes.Outcomes.Sum(outcome => outcome.Probability * outcome.Result.State.Entities[SecondHero].Character!.Health));
        Assert.All(outcomes.Outcomes, outcome => Assert.InRange(outcome.Result.Events.Count(e => e.Kind == SimulationEventKind.Damage && e.Target == wisp), 0, 1));
        Assert.Equal(GameZone.Hand, root.LocationOf(spell).Zone);

        var replay = simulator.Apply(root, action);
        Assert.Equal(GameZone.Graveyard, replay.State.LocationOf(wisp).Zone);
        Assert.Equal(28, replay.State.Entities[SecondHero].Character!.Health);
        Assert.NotEqual(root.RandomState, replay.State.RandomState);
    }

    [Fact]
    public void RandomDamageTriggersForOneMissileBeforeTheNextMissileHits()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero,
            Spell("two-missiles") with { OnPlay = [new RandomDamageEffect(RandomCharacterPool.EnemyMinions, 1, Count: 2)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Deck, [22, 23])) },
            Card(11, "two-missiles", First), InPlay(21, "EX1_007", Second, 3, 1, 4), Card(22, "CS2_231", Second), Card(23, "CS2_231", Second));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));
        Assert.Equal(new[] { SimulationEventKind.CardPlayed, SimulationEventKind.Damage, SimulationEventKind.Triggered,
            SimulationEventKind.CardDrawn, SimulationEventKind.Damage, SimulationEventKind.Triggered, SimulationEventKind.CardDrawn },
            result.Events.Select(e => e.Kind));
        Assert.Equal(root.RandomState, result.State.RandomState);
    }

    [Fact]
    public void CleaveHitsDistinctTargetsBeforeDispatchingDamageTriggersAndAcceptsOneTarget()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var spell = new EntityId(11);
        var second = Player(Second, SecondHero);
        var first = Player(First, FirstHero) with { Mana = 2, ManaCrystals = 2, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) };
        var root = Create(simulator, First, first,
            second with { Zones = Zones((GameZone.Play, [20, 21, 22]), (GameZone.Deck, [23])) }, Card(11, "CS2_114", First),
            InPlay(21, "EX1_007", Second, 3, 1, 4), InPlay(22, "CS2_172", Second, 4, 3, 2), Card(23, "CS2_231", Second));

        var action = new SimulationAction(SimulationActionKind.PlayCard, First, spell);
        var outcomes = simulator.ApplyDistribution(root, action);
        Assert.True(outcomes.IsExact);
        Assert.Equal(2, outcomes.Outcomes.Length);
        foreach (var outcome in outcomes.Outcomes)
        {
            Assert.Equal(0.5, outcome.Probability);
            Assert.Equal(new[] { SimulationEventKind.Damage, SimulationEventKind.Damage, SimulationEventKind.Triggered },
                outcome.Result.Events.Skip(1).Take(3).Select(e => e.Kind));
            Assert.Equal(2, outcome.Result.Events.Where(e => e.Kind == SimulationEventKind.Damage).Select(e => e.Target).Distinct().Count());
        }

        var oneTarget = Create(simulator, First, first, second with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "CS2_114", First), InPlay(21, "CS2_172", Second, 3, 3, 2));
        Assert.Single(simulator.Apply(oneTarget, action).Events, e => e.Kind == SimulationEventKind.Damage);
        var noTarget = Create(simulator, First, first, second, Card(11, "CS2_114", First));
        Assert.DoesNotContain(action, simulator.LegalActions(noTarget));
    }

    [Fact]
    public void AnimalCompanionCreatesThreeEquiprobableFormsAndAppliesAuraAndCharge()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var wisp = new EntityId(11);
        var animal = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 3, ManaCrystals = 3, Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12])) },
            Player(Second, SecondHero), InPlay(11, "CS2_231", First, 3, 1, 1), Card(12, "NEW1_031", First));
        var distribution = simulator.ApplyDistribution(root, new(SimulationActionKind.PlayCard, First, new EntityId(12)));
        Assert.True(distribution.IsExact);
        Assert.Equal(new[] { "NEW1_032", "NEW1_033", "NEW1_034" }, distribution.Outcomes.Select(outcome => outcome.Result.State.Entities[animal].CardId));
        Assert.All(distribution.Outcomes, outcome => Assert.Equal(1.0 / 3, outcome.Probability));
        Assert.Equal(2, distribution.Outcomes[1].Result.State.Entities[wisp].Character!.Attack);
        Assert.DoesNotContain(simulator.LegalActions(distribution.Outcomes[0].Result.State), action => action.Source == animal);
        Assert.Contains(simulator.LegalActions(distribution.Outcomes[2].Result.State), action => action.Source == animal && action.Target == SecondHero);
    }

    [Fact]
    public void BrawlMarksTheWholeDeathWaveBeforeResolvingAnyDeathrattle()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero, Minion("rattle", 1, 1) with { Deathrattle = [new SummonEffect("CS2_231")] }
        ]);
        var friendly = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 5, ManaCrystals = 5,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21, 22])) },
            InPlay(11, "rattle", First, 3, 1, 1), Card(12, "EX1_407", First),
            InPlay(21, "CS2_231", Second, 4, 1, 1), InPlay(22, "CS2_231", Second, 5, 1, 1));

        var result = simulator.ApplyDistribution(root, new(SimulationActionKind.PlayCard, First, new EntityId(12)));
        Assert.True(result.IsExact);
        Assert.Equal(3, result.Outcomes.Length);
        Assert.All(result.Outcomes, outcome => Assert.Equal(2, outcome.Result.State.MinionDeaths.Count));
        foreach (var outcome in result.Outcomes.Where(outcome => outcome.Result.State.LocationOf(friendly).Zone == GameZone.Graveyard))
        {
            Assert.Equal(new[] { SimulationEventKind.CardPlayed, SimulationEventKind.Died, SimulationEventKind.Died,
                SimulationEventKind.Deathrattle, SimulationEventKind.Summoned }, outcome.Result.Events.Select(e => e.Kind));
            Assert.Equal(GameZone.Play, outcome.Result.State.LocationOf(new EntityId(23)).Zone);
        }
    }

    [Fact]
    public void DistributionSamplingIsBoundedAndDoesNotUseTheReplaySeed()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "EX1_277", First), InPlay(21, "EX1_007", Second, 3, 1, 4));
        var secondSeed = simulator.CreateGame(root.Players.Values, root.Entities.Values, First, randomState: 55);
        var action = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(11));

        var first = simulator.ApplyDistribution(root, action, workLimit: 1);
        var second = simulator.ApplyDistribution(secondSeed, action, workLimit: 1);
        Assert.False(first.IsExact);
        Assert.Equal(1, first.Resolutions);
        Assert.Equal(1, Assert.Single(first.Outcomes).Probability);
        Assert.Equal(first.Outcomes[0].Result.Events, second.Outcomes[0].Result.Events.AsEnumerable());
    }
}
