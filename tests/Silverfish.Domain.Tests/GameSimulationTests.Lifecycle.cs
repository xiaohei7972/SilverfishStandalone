using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void ShadowstepClearsBattlefieldStateAndDiscountsOnlyTheNextPlayOfTheSameEntity()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var minion = new EntityId(11);
        var shadowstep = new EntityId(13);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11, 12]),
                (GameZone.Hand, [13]), (GameZone.Graveyard, [14])) }, Player(Second, SecondHero),
            InPlay(11, "EX1_007", First, 3, 1, 4) with
            {
                Character = new(1, 4, Damage: 1, AttacksThisTurn: 1), Silenced = true,
                Enchantments = [new(new EntityId(14), new(StatModifierMode.Add, MaximumHealth: 2))]
            }, InPlay(12, "CS2_222", First, 4, 7, 7), Card(13, "EX1_144", First), Card(14, "CS2_092", First));

        var returned = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, shadowstep, minion));
        var inHand = returned.State.Entities[minion];
        Assert.Equal(new EntityLocation(First, GameZone.Hand), returned.State.LocationOf(minion));
        Assert.Null(inHand.Character);
        Assert.False(inHand.Silenced);
        Assert.Empty(inHand.Enchantments);
        Assert.Empty(inHand.AuraGrants);
        Assert.Empty(returned.State.MinionDeaths);
        Assert.Equal(new ManaCostModifier(shadowstep, -2), Assert.Single(inHand.CostModifiers));
        var play = new SimulationAction(SimulationActionKind.PlayCard, First, minion, Position: 0);
        Assert.Contains(play, simulator.LegalActions(returned.State));

        var replayed = simulator.Apply(returned.State, play).State;
        var backInPlay = replayed.Entities[minion];
        Assert.Equal(0, replayed.Players[First].Mana);
        Assert.Empty(backInPlay.CostModifiers);
        Assert.Equal(5, backInPlay.Character!.Health);
        Assert.Equal(0, backInPlay.Character.AttacksThisTurn);
        Assert.True(backInPlay.PlayOrder > root.Entities[minion].PlayOrder);
        Assert.True(root.Entities[minion].Silenced);
    }

    [Fact]
    public void ReturningAStolenMinionUsesItsControllersHandAndKeepsItsOwner()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero,
            Spell("steal") with { Target = TargetRequirement.EnemyMinion, OnPlay = [new TakeControlEffect()] }
        ]);
        var minion = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 2, ManaCrystals = 2, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "steal", First), Card(12, "EX1_049", First), InPlay(21, "CS2_172", Second, 3, 3, 2));

        var stolen = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11), minion)).State;
        var returned = simulator.Apply(stolen, new(SimulationActionKind.PlayCard, First, new EntityId(12), minion, Position: 0)).State;

        Assert.Equal(new EntityLocation(First, GameZone.Hand), returned.LocationOf(minion));
        Assert.Equal(Second, returned.Entities[minion].Owner);
        Assert.Equal(First, returned.Entities[minion].Controller);
        Assert.Empty(returned.MinionDeaths);
    }

    [Fact]
    public void SapIntoAFullHandDestroysTheMinionAndQueuesItsDeathrattle()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero,
            Minion("rattle", 1, 2) with { Deathrattle = [new DamageEffect(DamageTarget.EnemyHero, 2)] }
        ]);
        var minion = new EntityId(21);
        var handIds = Enumerable.Range(22, 10).ToArray();
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 2, ManaCrystals = 2, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Hand, handIds)) },
            [Card(11, "EX1_581", First), InPlay(21, "rattle", Second, 3, 1, 2), .. handIds.Select(id => Card(id, "CS2_231", Second))]);

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11), minion));

        Assert.Equal(10, result.State.Players[Second].In(GameZone.Hand).Length);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(minion).Zone);
        Assert.Equal(new MinionDeath(minion, "rattle", Second, 1), Assert.Single(result.State.MinionDeaths));
        Assert.Equal(28, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(new[] { SimulationEventKind.CardPlayed, SimulationEventKind.Died, SimulationEventKind.Deathrattle,
            SimulationEventKind.Damage }, result.Events.Select(e => e.Kind));
    }

    [Fact]
    public void HexPreservesTheTargetIdAndPositionButReplacesItsAbilitiesAndItsDeathRecord()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero,
            Minion("rattle", 5, 4) with
            {
                Keywords = CardKeywords.DivineShield,
                Deathrattle = [new DamageEffect(DamageTarget.EnemyHero, 6)]
            }
        ]);
        var target = new EntityId(22);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 7, ManaCrystals = 7, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21, 22, 23])) },
            Card(11, "EX1_246", First), Card(12, "CS2_076", First),
            InPlay(21, "CS2_231", Second, 3, 1, 1), InPlay(22, "rattle", Second, 4, 5, 4) with
            {
                Character = new(5, 4, Damage: 1, DivineShield: true)
            }, InPlay(23, "CS2_231", Second, 5, 1, 1));

        var hexed = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11), target));
        var frog = hexed.State.Entities[target];
        Assert.Equal("hexfrog", frog.CardId);
        Assert.Equal(root.Entities[target].PlayOrder, frog.PlayOrder);
        Assert.Equal(root.Players[Second].In(GameZone.Play), hexed.State.Players[Second].In(GameZone.Play));
        Assert.Equal(0, frog.Character!.Attack);
        Assert.Equal(1, frog.Character.Health);
        Assert.True(frog.Character.Exhausted);
        Assert.False(frog.Character.DivineShield);
        Assert.Empty(hexed.State.MinionDeaths);
        Assert.Equal(new[] { SimulationEventKind.CardPlayed, SimulationEventKind.Transformed }, hexed.Events.Select(e => e.Kind));

        var destroyed = simulator.Apply(hexed.State, new(SimulationActionKind.PlayCard, First, new EntityId(12), target));
        Assert.Equal(new MinionDeath(target, "hexfrog", Second, 1), Assert.Single(destroyed.State.MinionDeaths));
        Assert.DoesNotContain(destroyed.Events, e => e.Kind is SimulationEventKind.Deathrattle or SimulationEventKind.Damage);
    }

    [Fact]
    public void ResurrectCreatesFreshCopiesWithoutConsumingTheRecordOrSelectingOtherGraveyardCards()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var deadMinion = new EntityId(11);
        var spell = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(12));
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 6, ManaCrystals = 6, Zones = Zones((GameZone.Play, [10, 11]),
                (GameZone.Hand, [12, 13]), (GameZone.HeroPower, [14]), (GameZone.Graveyard, [15])) },
            Player(Second, SecondHero), InPlay(11, "EX1_007", First, 3, 1, 4) with { Character = new(1, 4, Damage: 3), Silenced = true },
            Card(12, "BRM_017", First), Card(13, "BRM_017", First), Card(14, "HERO_08bp", First), Card(15, "CS2_231", First));
        Assert.DoesNotContain(spell, simulator.LegalActions(root));

        var killed = simulator.Apply(root, new(SimulationActionKind.UseHeroPower, First, new EntityId(14), deadMinion)).State;
        var first = simulator.Apply(killed, spell);
        var second = simulator.Apply(first.State, spell with { Source = new EntityId(13) });

        var firstResurrection = Assert.Single(first.Events, e => e.Kind == SimulationEventKind.Resurrected);
        var secondResurrection = Assert.Single(second.Events, e => e.Kind == SimulationEventKind.Resurrected);
        Assert.Equal(deadMinion, firstResurrection.Source);
        Assert.Equal(deadMinion, secondResurrection.Source);
        Assert.NotEqual(firstResurrection.Target, secondResurrection.Target);
        Assert.Equal(GameZone.Graveyard, second.State.LocationOf(deadMinion).Zone);
        Assert.Same(killed.MinionDeaths, second.State.MinionDeaths);
        foreach (var restored in new[] { firstResurrection.Target!.Value, secondResurrection.Target!.Value })
        {
            var entity = second.State.Entities[restored];
            Assert.Equal("EX1_007", entity.CardId);
            Assert.Equal(4, entity.Character!.Health);
            Assert.False(entity.Silenced);
            Assert.True(entity.Character.Exhausted);
        }
    }

    [Fact]
    public void ResurrectionUsesBranchLocalRandomStateAndHistoricalFormEvenAfterTheCardLeavesTheGraveyard()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var ready = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 2, ManaCrystals = 2,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]), (GameZone.Deck, [12]), (GameZone.Graveyard, [13])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Graveyard, [14])) },
            Card(11, "BRM_017", First), Card(12, "hexfrog", First), Card(13, "EX1_007", First), Card(14, "CS2_231", Second));
        MinionDeath[] deaths = [new(new EntityId(12), "CS2_172", First, 1), new(new EntityId(14), "CS2_231", Second, 1),
            new(new EntityId(13), "EX1_007", First, 1)];
        var root = simulator.CreateGame(ready.Players.Values, ready.Entities.Values, First, minionDeaths: deaths, randomState: 2);
        var otherSeed = simulator.CreateGame(ready.Players.Values, ready.Entities.Values, First, minionDeaths: deaths, randomState: 0);
        var play = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(11));

        var result = simulator.Apply(root, play);
        var duplicate = simulator.Apply(root, play);
        var different = simulator.Apply(otherSeed, play);

        Assert.Equal(result.Events, duplicate.Events.AsEnumerable());
        Assert.Equal(result.State.RandomState, duplicate.State.RandomState);
        Assert.NotEqual(root.RandomState, result.State.RandomState);
        Assert.Equal(2UL, root.RandomState);
        Assert.Equal("CS2_172", result.State.Entities[new EntityId(21)].CardId);
        Assert.Equal("EX1_007", different.State.Entities[new EntityId(21)].CardId);
        Assert.Equal(GameZone.Deck, result.State.LocationOf(new EntityId(12)).Zone);
    }

    [Fact]
    public void ResurrectIsNotPlayableWhenAllSevenMinionSlotsAreOccupied()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var board = Enumerable.Range(11, 7).ToArray();
        var ready = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 2, ManaCrystals = 2,
                Zones = Zones((GameZone.Play, [10, .. board]), (GameZone.Hand, [18]), (GameZone.Graveyard, [19])) },
            Player(Second, SecondHero), [Card(18, "BRM_017", First), Card(19, "CS2_231", First),
                .. board.Select(id => InPlay(id, "CS2_231", First, id - 8, 1, 1))]);
        var full = simulator.CreateGame(ready.Players.Values, ready.Entities.Values, First,
            minionDeaths: [new(new EntityId(19), "CS2_231", First, 1)]);

        Assert.DoesNotContain(simulator.LegalActions(full), action => action.Source == new EntityId(18));
    }

    [Fact]
    public void MindControlOverflowDuringResolutionUsesThePreviousControllersDeathHistory()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero,
            Minion("summon-on-spell", 1, 2) with
            {
                Triggers = [new(TriggerTiming.BeforeSpellResolved, TriggerRelation.Friendly, [new SummonEffect("CS2_231")])]
            }
        ]);
        var target = new EntityId(21);
        var spell = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(17), target);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 9, ManaCrystals = 9,
                Zones = Zones((GameZone.Play, [10, 11, 12, 13, 14, 15, 16]), (GameZone.Hand, [17])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            [InPlay(11, "summon-on-spell", First, 3, 1, 2), InPlay(21, "CS2_231", Second, 10, 1, 1),
             Card(17, "CS1_113", First), .. Enumerable.Range(12, 5).Select(id => InPlay(id, "CS2_231", First, id - 8, 1, 1))]);
        Assert.Contains(spell, simulator.LegalActions(root));

        var result = simulator.Apply(root, spell);
        Assert.Equal(new EntityLocation(Second, GameZone.Graveyard), result.State.LocationOf(target));
        Assert.Equal(new MinionDeath(target, "CS2_231", Second, 1), Assert.Single(result.State.MinionDeaths));
        Assert.DoesNotContain(result.Events, e => e.Kind == SimulationEventKind.ControlChanged);
        Assert.Equal(8, result.State.Players[First].In(GameZone.Play).Length);
    }
}
