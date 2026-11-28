using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    private static (GameSimulator Simulator, GameState State) DamageListenerScenario(
        CardEffect damage, bool twoTargets = true)
    {
        var summoner = Minion("on-damage-summoner", 0, 6) with
        {
            Triggers = [new(TriggerTiming.Damage, TriggerRelation.Friendly,
                [new SummonEffect("late-damage-watcher")])
            {
                EntityMatch = TriggerEntityMatch.Self
            }]
        };
        var watcher = Minion("late-damage-watcher", 0, 5) with
        {
            Triggers = [new(TriggerTiming.Damage, TriggerRelation.Friendly,
                [new GainArmorEffect(1)])
            {
                EntityKind = EntityKind.Minion
            }]
        };
        var simulator = new GameSimulator([
            Hero, summoner, watcher, Minion("second-victim", 0, 6),
            Spell("damage-batch") with { OnPlay = [damage] }
        ]);
        var cards = new List<GameEntity> {
            Card(11, "damage-batch", First),
            InPlay(21, "on-damage-summoner", Second, 3, 0, 6)
        };
        if (twoTargets)
            cards.Add(InPlay(22, "second-victim", Second, 4, 0, 6));
        var state = Create(simulator, First,
            Player(First, FirstHero) with {
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]))
            },
            Player(Second, SecondHero) with {
                Zones = Zones((GameZone.Play, twoTargets ? [20, 21, 22] : [20, 21]))
            },
            cards.ToArray());
        return (simulator, state);
    }

    [Fact]
    public void AreaDamageDoesNotNotifyAListenerSummonedAfterTheEntireDamageBatch()
    {
        var (simulator, root) = DamageListenerScenario(
            new DamageEffect(DamageTarget.EnemyMinions, 1));
        var result = simulator.Apply(root,
            new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.Equal(new[] { 21, 22 }, result.Events.Where(e => e.Kind == SimulationEventKind.Damage)
            .Select(e => e.Target!.Value.Value));
        var watcher = Assert.Single(result.State.Players[Second].In(GameZone.Play),
            id => result.State.Entities[id].CardId == "late-damage-watcher");
        Assert.Contains(result.Events, e => e.Kind == SimulationEventKind.Summoned && e.Source == watcher);
        Assert.DoesNotContain(result.Events, e => e.Kind == SimulationEventKind.Triggered && e.Source == watcher);
        Assert.Equal(0, result.State.Entities[SecondHero].Character!.Armor);
        Assert.Equal(GameZone.Play, root.LocationOf(new EntityId(21)).Zone);
    }

    [Fact]
    public void DistinctRandomBatchUsesEventTimeListenersInEveryExactBranch()
    {
        var (simulator, root) = DamageListenerScenario(new RandomDamageEffect(
            RandomCharacterPool.EnemyMinions, 1, Count: 2, Mode: RandomDamageMode.DistinctBatch));
        var distribution = simulator.ApplyDistribution(root,
            new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.True(distribution.IsExact);
        Assert.Equal(2, distribution.Outcomes.Length);
        Assert.All(distribution.Outcomes, outcome => {
            Assert.Equal(0.5, outcome.Probability);
            Assert.Equal(2, outcome.Result.Events.Count(e => e.Kind == SimulationEventKind.Damage));
            Assert.Equal(0, outcome.Result.State.Entities[SecondHero].Character!.Armor);
            var watcher = Assert.Single(outcome.Result.State.Players[Second].In(GameZone.Play),
                id => outcome.Result.State.Entities[id].CardId == "late-damage-watcher");
            Assert.DoesNotContain(outcome.Result.Events,
                e => e.Kind == SimulationEventKind.Triggered && e.Source == watcher);
        });
        Assert.Equal(0, root.Entities[SecondHero].Character!.Armor);
    }

    [Fact]
    public void SequentialRandomHitsAllowNewListenersToObserveLaterActualDamage()
    {
        var (simulator, root) = DamageListenerScenario(new RandomDamageEffect(
            RandomCharacterPool.EnemyMinions, 1, Count: 2, Mode: RandomDamageMode.Sequential),
            twoTargets: false);
        var distribution = simulator.ApplyDistribution(root,
            new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.True(distribution.IsExact);
        Assert.Equal(2, distribution.Outcomes.Length);
        Assert.All(distribution.Outcomes, outcome => {
            Assert.Equal(2, outcome.Result.Events.Count(e => e.Kind == SimulationEventKind.Damage));
            Assert.Equal(1, outcome.Result.State.Entities[SecondHero].Character!.Armor);
            Assert.Contains(outcome.Result.Events,
                e => e.Kind == SimulationEventKind.Triggered && e.Source is EntityId source &&
                     outcome.Result.State.Entities[source].CardId == "late-damage-watcher");
        });
        Assert.Equal(0, root.Entities[SecondHero].Character!.Armor);
    }
}
