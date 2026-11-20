using System.Collections.Immutable;
using Silverfish.Core;
using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Tests;

public sealed class SimulationTurnPlannerTests
{
    [Fact]
    public void FindsLethalByRemovingTauntBeforeAttacking()
    {
        var player = new PlayerId(1);
        var opponent = new PlayerId(2);
        var hero = new EntityId(10);
        var enemyHero = new EntityId(20);
        var fighter = new EntityId(11);
        var spell = new EntityId(12);
        var guard = new EntityId(21);
        var simulator = new GameSimulator([
            new(new("hero", EntityKind.Hero, 0, 0, 30)),
            new(new("fighter", EntityKind.Minion, 1, 3, 2)),
            new SimulationCard(new("guard", EntityKind.Minion, 1, 1, 2)) { Keywords = CardKeywords.Taunt },
            new SimulationCard(new("bolt", EntityKind.Spell, 1))
            {
                Target = TargetRequirement.EnemyMinion,
                OnPlay = [new DamageEffect(DamageTarget.Selected, 2)]
            }
        ]);
        var root = simulator.CreateGame([
            new PlayerState(player, hero, Mana: 1, ManaCrystals: 1)
            {
                Zones = ImmutableDictionary<GameZone, ImmutableArray<EntityId>>.Empty
                    .Add(GameZone.Play, [hero, fighter]).Add(GameZone.Hand, [spell])
            },
            new PlayerState(opponent, enemyHero)
            {
                Zones = ImmutableDictionary<GameZone, ImmutableArray<EntityId>>.Empty
                    .Add(GameZone.Play, [enemyHero, guard])
            }
        ], [
            new GameEntity(hero, "hero", player, player) { Character = new(0, 30), PlayOrder = 1 },
            new GameEntity(enemyHero, "hero", opponent, opponent) { Character = new(0, 30, Damage: 27), PlayOrder = 2 },
            new GameEntity(fighter, "fighter", player, player) { Character = new(3, 2), PlayOrder = 3 },
            new GameEntity(guard, "guard", opponent, opponent) { Character = new(1, 2), PlayOrder = 4 },
            new GameEntity(spell, "bolt", player, player)
        ], player);

        var planner = new SimulationTurnPlanner(simulator, SimulationMaterialEvaluator.Evaluate);
        var plan = planner.Plan(root);

        Assert.Equal(new SimulationAction[]
        {
            new(SimulationActionKind.PlayCard, player, spell, guard),
            new(SimulationActionKind.Attack, player, fighter, enemyHero)
        }, plan.Actions);
        Assert.Equal(player, plan.Winner);
        Assert.Equal(GameZone.Hand, root.LocationOf(spell).Zone);
        Assert.Equal(3, root.Entities[enemyHero].Character!.Health);
        Assert.Equal(1, planner.Plan(root, transitionLimit: 1).Transitions);
    }

    [Fact]
    public void PlansWithMissileProbabilitiesWithoutReadingTheReplaySeed()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(new(new("hero", EntityKind.Hero, 0, PrintedHealth: 30))));
        var first = new PlayerId(1);
        var firstHero = new EntityId(10);
        var second = new PlayerId(2);
        var secondHero = new EntityId(20);
        var spell = new EntityId(11);
        var shield = new EntityId(21);
        var draw = new EntityId(22);
        var players = new[]
        {
            new PlayerState(first, firstHero, Mana: 1, ManaCrystals: 1)
            {
                Zones = ImmutableDictionary<GameZone, ImmutableArray<EntityId>>.Empty.Add(GameZone.Play, [firstHero]).Add(GameZone.Hand, [spell])
            },
            new PlayerState(second, secondHero)
            {
                Zones = ImmutableDictionary<GameZone, ImmutableArray<EntityId>>.Empty.Add(GameZone.Play, [secondHero, shield]).Add(GameZone.Deck, [draw])
            }
        };
        var entities = new[]
        {
            new GameEntity(firstHero, "hero", first, first) { PlayOrder = 1, Character = new(0, 30) },
            new GameEntity(secondHero, "hero", second, second) { PlayOrder = 2, Character = new(0, 30, Damage: 29) },
            new GameEntity(shield, "EX1_033", second, second) { PlayOrder = 3, Character = new(4, 5) },
            new GameEntity(draw, "CS2_231", second, second), new GameEntity(spell, "EX1_277", first, first)
        };
        var planner = new SimulationTurnPlanner(simulator, SimulationMaterialEvaluator.Evaluate);
        var root = simulator.CreateGame(players, entities, first);
        var otherSeed = simulator.CreateGame(players, entities, first, randomState: 99);
        var plan = planner.Plan(root);
        var rerun = planner.Plan(otherSeed);

        Assert.Equal(new SimulationAction(SimulationActionKind.PlayCard, first, spell), Assert.Single(plan.Actions));
        Assert.Equal(0.875, plan.WinProbability);
        Assert.Equal(plan.WinProbability, rerun.WinProbability);
        Assert.Equal(plan.Actions, rerun.Actions.AsEnumerable());
        Assert.Equal(MatchOutcome.InProgress, plan.Outcome);
        Assert.Null(plan.Winner);
        Assert.True(plan.ReplanAfterLastAction);
        Assert.False(plan.UsesSamples);
        Assert.InRange(plan.Transitions, 2, 512);
    }

    [Fact]
    public void ForecastsPostSummonLethalButWaitsForTheActualCompanionBeforeIssuingTheAttack()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(new(new("hero", EntityKind.Hero, 0, PrintedHealth: 30))));
        var first = new PlayerId(1);
        var firstHero = new EntityId(10);
        var second = new PlayerId(2);
        var secondHero = new EntityId(20);
        var spell = new EntityId(11);
        var root = simulator.CreateGame([
            new PlayerState(first, firstHero, Mana: 3, ManaCrystals: 3)
            {
                Zones = ImmutableDictionary<GameZone, ImmutableArray<EntityId>>.Empty.Add(GameZone.Play, [firstHero]).Add(GameZone.Hand, [spell])
            },
            new PlayerState(second, secondHero)
            {
                Zones = ImmutableDictionary<GameZone, ImmutableArray<EntityId>>.Empty.Add(GameZone.Play, [secondHero])
            }
        ], [
            new GameEntity(firstHero, "hero", first, first) { PlayOrder = 1, Character = new(0, 30) },
            new GameEntity(secondHero, "hero", second, second) { PlayOrder = 2, Character = new(0, 30, Damage: 26) },
            new GameEntity(spell, "NEW1_031", first, first)
        ], first);
        var planner = new SimulationTurnPlanner(simulator, SimulationMaterialEvaluator.Evaluate);

        var onePly = planner.Plan(root, depth: 1);
        var twoPly = planner.Plan(root, depth: 2, transitionLimit: 32);
        Assert.Equal(0, onePly.WinProbability);
        Assert.Equal(1.0 / 3, twoPly.WinProbability);
        Assert.Equal(new SimulationAction(SimulationActionKind.PlayCard, first, spell), Assert.Single(twoPly.Actions));
        Assert.True(twoPly.ReplanAfterLastAction);
        Assert.InRange(twoPly.Transitions, 2, 32);
    }

    [Fact]
    public void PlannerCanSelectTheDrawChoiceThatEnablesLethal()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, new(new("hero", EntityKind.Hero, 0, PrintedHealth: 30)),
            new SimulationCard(new("test-bolt", EntityKind.Spell, 0)) { OnPlay = [new DamageEffect(DamageTarget.EnemyHero, 1)] }
        ]);
        var first = new PlayerId(1);
        var second = new PlayerId(2);
        var firstHero = new EntityId(10);
        var secondHero = new EntityId(20);
        var nourish = new EntityId(11);
        var bolt = new EntityId(12);
        var nextDraw = new EntityId(21);
        var root = simulator.CreateGame([
            new PlayerState(first, firstHero, Mana: 5, ManaCrystals: 5)
            {
                Zones = ImmutableDictionary<GameZone, ImmutableArray<EntityId>>.Empty.Add(GameZone.Play, [firstHero])
                    .Add(GameZone.Hand, [nourish]).Add(GameZone.Deck, [bolt, new EntityId(13), new EntityId(14)])
            },
            new PlayerState(second, secondHero)
            {
                Zones = ImmutableDictionary<GameZone, ImmutableArray<EntityId>>.Empty.Add(GameZone.Play, [secondHero])
                    .Add(GameZone.Deck, [nextDraw])
            }
        ], [
            new GameEntity(firstHero, "hero", first, first) { PlayOrder = 1, Character = new(0, 30) },
            new GameEntity(secondHero, "hero", second, second) { PlayOrder = 2, Character = new(0, 30, Damage: 29) },
            new GameEntity(nourish, "EX1_164", first, first), new GameEntity(bolt, "test-bolt", first, first),
            new GameEntity(new EntityId(13), "CS2_231", first, first), new GameEntity(new EntityId(14), "CS2_231", first, first),
            new GameEntity(nextDraw, "CS2_231", second, second)
        ], first);

        var plan = new SimulationTurnPlanner(simulator, SimulationMaterialEvaluator.Evaluate).Plan(root, depth: 2);
        Assert.Equal(new SimulationAction[]
        {
            new(SimulationActionKind.PlayCard, first, nourish) { Choice = "draw" },
            new(SimulationActionKind.PlayCard, first, bolt)
        }, plan.Actions);
        Assert.Equal(first, plan.Winner);
    }
}
