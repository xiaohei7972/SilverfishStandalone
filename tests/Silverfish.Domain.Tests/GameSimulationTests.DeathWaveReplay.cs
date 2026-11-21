using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void NewlySummonedDeathWatcherObservesOnlyTheNextDeathWave()
    {
        var onFriendlyDeath = new CardTrigger(TriggerTiming.MinionDied, TriggerRelation.Friendly,
            [new GainArmorEffect(1)]);
        var simulator = new GameSimulator([
            Hero,
            Minion("first", 0, 1) with
            {
                Deathrattle = [new SummonEffect("watcher"), new DamageEffect(DamageTarget.AllMinions, 1)]
            },
            Minion("second", 0, 2),
            Minion("watcher", 0, 3) with { Triggers = [onFriendlyDeath] },
            Spell("sweep") with { OnPlay = [new DamageEffect(DamageTarget.AllMinions, 1)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13])) },
            Player(Second, SecondHero),
            InPlay(11, "first", First, 3, 0, 1), InPlay(12, "second", First, 4, 0, 2),
            Card(13, "sweep", First));
        var action = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(13));

        var result = simulator.Apply(root, action);

        Assert.Equal(new EntityId?[] { new(11), new(12) },
            result.Events.Where(e => e.Kind == SimulationEventKind.Died).Select(e => e.Source));
        var death = Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Triggered &&
            e.Timing == TriggerTiming.MinionDied);
        Assert.Equal(new EntityId(root.NextEntity), death.Source);
        Assert.Equal(new EntityId(12), death.Target);
        Assert.Equal(1, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(2, result.State.Entities[new EntityId(root.NextEntity)].Character!.Health);
        Assert.Equal(result.Events, simulator.Apply(root, action).Events.AsEnumerable());
        Assert.Equal(GameZone.Play, root.LocationOf(new EntityId(11)).Zone);
    }

    [Fact]
    public void ImmediateAndStandardDeathsKeepSeparateObserverSnapshotsInOneWave()
    {
        var onEnemyDeath = new CardTrigger(TriggerTiming.MinionDied, TriggerRelation.Opponent,
            [new GainArmorEffect(1)]);
        var simulator = new GameSimulator([
            Hero,
            Minion("early", 0, 3) with { Triggers = [onEnemyDeath] },
            Minion("late", 0, 3) with { Triggers = [onEnemyDeath] },
            Minion("victim", 0, 2) with { Deathrattle = [new GainArmorEffect(2)] },
            Minion("doomed", 0, 1), Minion("filler", 1, 1),
            Spell("bounce-summon-sweep") with
            {
                Target = TargetRequirement.EnemyMinion,
                OnPlay = [new ReturnMinionEffect(), new SummonEffect("late"),
                    new DamageEffect(DamageTarget.AllMinions, 1)]
            }
        ]);
        var fullHand = Enumerable.Range(30, 10).ToArray();
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21, 22]), (GameZone.Hand, fullHand))
            },
            [
                InPlay(12, "early", First, 5, 0, 3), Card(11, "bounce-summon-sweep", First),
                InPlay(21, "victim", Second, 3, 0, 2), InPlay(22, "doomed", Second, 4, 0, 1),
                .. fullHand.Select(id => Card(id, "filler", Second))
            ]);
        var action = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(11), new EntityId(21));

        var result = simulator.Apply(root, action);
        var late = Assert.Single(result.State.Players[First].In(GameZone.Play),
            id => result.State.Entities[id].CardId == "late");
        Assert.Equal(3, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(2, result.State.Entities[SecondHero].Character!.Armor);
        Assert.Equal(new EntityId?[] { new(12), new(12), late },
            result.Events.Where(e => e.Kind == SimulationEventKind.Triggered &&
                e.Timing == TriggerTiming.MinionDied).Select(e => e.Source));
        Assert.Equal(new EntityId?[] { new(21), new(22) },
            result.Events.Where(e => e.Kind == SimulationEventKind.Died).Select(e => e.Source));
        Assert.Equal(result.Events, simulator.Apply(root, action).Events.AsEnumerable());
        Assert.Equal(0, root.Entities[FirstHero].Character!.Armor);
    }

    [Fact]
    public void HonorableKillAgainstHeroCanCauseADrawBeforeMatchEnds()
    {
        var simulator = new GameSimulator([
            Hero,
            Minion("finisher", 2, 4) with
            {
                Triggers = [new CardTrigger(TriggerTiming.HonorableKill, TriggerRelation.Friendly,
                    [new DamageEffect(DamageTarget.FriendlyHero, 2)])
                {
                    Focus = TriggerFocus.DamageSource,
                    EntityMatch = TriggerEntityMatch.Self
                }]
            }
        ]);
        var initial = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero),
            InPlay(11, "finisher", First, 3, 2, 4));
        var root = simulator.CreateGame(initial.Players.Values,
            initial.Entities.Values.Select(e => e.Id == FirstHero || e.Id == SecondHero
                ? e with { Character = new(0, 30, Damage: 28) } : e),
            initial.ActivePlayer, initial.Turn, initial.MinionDeaths, initial.RandomState);
        var action = new SimulationAction(SimulationActionKind.Attack, First, new EntityId(11), SecondHero);

        var result = simulator.Apply(root, action);

        Assert.Equal(MatchOutcome.Draw, result.State.Outcome);
        Assert.Null(result.State.Winner);
        Assert.Equal(0, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(0, result.State.Entities[SecondHero].Character!.Health);
        Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Triggered &&
            e.Timing == TriggerTiming.HonorableKill);
        Assert.Equal(SimulationEventKind.MatchEnded, result.Events[^1].Kind);
        Assert.Empty(simulator.LegalActions(result.State));
        Assert.Equal(result.Events, simulator.Apply(root, action).Events.AsEnumerable());
        Assert.Equal(MatchOutcome.InProgress, root.Outcome);
    }
}
