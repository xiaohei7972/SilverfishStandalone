using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void FullHandBounceDoesNotLetALateSummonObserveAnEarlierImmediateDeath()
    {
        var onEnemyDeath = new CardTrigger(TriggerTiming.MinionDied, TriggerRelation.Opponent,
            [new GainArmorEffect(1)]);
        var simulator = new GameSimulator([
            Hero,
            Minion("early-watcher", 0, 3) with { Triggers = [onEnemyDeath] },
            Minion("late-watcher", 0, 3) with { Triggers = [onEnemyDeath] },
            Minion("victim", 0, 2) with { Deathrattle = [new GainArmorEffect(3)] },
            Minion("filler", 1, 1),
            Spell("bounce-then-summon") with
            {
                Target = TargetRequirement.EnemyMinion,
                OnPlay = [new ReturnMinionEffect(), new SummonEffect("late-watcher")]
            }
        ]);
        var fullHand = Enumerable.Range(30, 10).ToArray();
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Hand, fullHand)) },
            [
                Card(11, "bounce-then-summon", First),
                InPlay(12, "early-watcher", First, 3, 0, 3),
                InPlay(21, "victim", Second, 4, 0, 2),
                .. fullHand.Select(id => Card(id, "filler", Second))
            ]);
        var action = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(11), new EntityId(21));

        var result = simulator.Apply(root, action);
        var lateWatcher = Assert.Single(result.State.Players[First].In(GameZone.Play),
            id => result.State.Entities[id].CardId == "late-watcher");

        Assert.Equal(new EntityLocation(Second, GameZone.Graveyard), result.State.LocationOf(new EntityId(21)));
        Assert.Equal(1, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(3, result.State.Entities[SecondHero].Character!.Armor);
        Assert.Equal(new EntityId(12),
            Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Triggered).Source);
        Assert.DoesNotContain(result.Events, e => e.Kind == SimulationEventKind.Triggered && e.Source == lateWatcher);
        Assert.Equal(new EntityId(21),
            Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Deathrattle).Source);
        Assert.Equal(result.Events, simulator.Apply(root, action).Events.AsEnumerable());
        Assert.Equal(GameZone.Play, root.LocationOf(new EntityId(21)).Zone);
        Assert.Equal(0, root.Entities[FirstHero].Character!.Armor);
    }
}
