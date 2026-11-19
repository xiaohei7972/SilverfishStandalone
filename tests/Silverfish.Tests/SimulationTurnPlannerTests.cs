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
}
