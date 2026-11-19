using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void CombatCommitsBothHitsBeforeSourceAndTargetListenersShareTheFirstQueue()
    {
        var simulator = new GameSimulator([
            Hero,
            Minion("attacker", 4, 2) with
            {
                Triggers =
                [
                    new(TriggerTiming.Damage, TriggerRelation.Friendly, [new DrawEffect(1)])
                    {
                        Focus = TriggerFocus.DamageSource, EntityMatch = TriggerEntityMatch.Self
                    },
                    new(TriggerTiming.Damage, TriggerRelation.Friendly, [new GainArmorEffect(2)])
                    {
                        EntityMatch = TriggerEntityMatch.Self
                    },
                    new(TriggerTiming.Damage, TriggerRelation.Friendly, [new GainArmorEffect(100)])
                    {
                        EntityMatch = TriggerEntityMatch.Self, RequireLivingEntity = true
                    }
                ]
            },
            Minion("defender", 3, 1) with
            {
                Triggers =
                [
                    new(TriggerTiming.Damage, TriggerRelation.Friendly, [new GainArmorEffect(1)])
                    {
                        EntityMatch = TriggerEntityMatch.Self
                    }
                ]
            }
        ]);
        var attacker = new EntityId(11);
        var defender = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Deck, [12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "attacker", First, 3, 4, 2), InPlay(21, "defender", Second, 4, 3, 1),
            Card(12, "attacker", First));

        var result = simulator.Apply(root, new(SimulationActionKind.Attack, First, attacker, defender));

        Assert.Equal(new EntityId?[] { defender, attacker }, result.Events
            .Skip(1).Take(2).Where(e => e.Kind == SimulationEventKind.Damage).Select(e => e.Target));
        Assert.Equal(new EntityId?[] { attacker, defender, attacker }, result.Events
            .Where(e => e.Kind == SimulationEventKind.Triggered).Select(e => e.Source));
        Assert.Equal(2, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(1, result.State.Entities[SecondHero].Character!.Armor);
        Assert.Equal(GameZone.Hand, result.State.LocationOf(new EntityId(12)).Zone);
        Assert.Equal(GameZone.Play, root.LocationOf(attacker).Zone);
    }

    [Fact]
    public void EnemyAreaDamageUsesEntryOrderAndDivineShieldDoesNotCauseADamageEvent()
    {
        var simulator = new GameSimulator([
            Hero, Minion("unit", 0, 1),
            Minion("shield", 0, 1) with { Keywords = CardKeywords.DivineShield },
            Minion("watcher", 0, 1) with
            {
                Triggers =
                [
                    new(TriggerTiming.Damage, TriggerRelation.Friendly, [new GainArmorEffect(1)])
                    {
                        EntityKind = EntityKind.Minion
                    }
                ]
            },
            Spell("sweep") with { OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 1)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 23, 21, 22])) },
            InPlay(11, "unit", First, 3, 0, 1), Card(12, "sweep", First),
            InPlay(21, "watcher", Second, 4, 0, 1), InPlay(22, "unit", Second, 5, 0, 1),
            InPlay(23, "shield", Second, 6, 0, 1) with { Character = new(0, 1, DivineShield: true) });

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(12)));

        Assert.Equal(new EntityId?[] { new(21), new(22) },
            result.Events.Skip(1).Take(2).Where(e => e.Kind == SimulationEventKind.Damage).Select(e => e.Target));
        Assert.Equal(SimulationEventKind.ShieldBroken, result.Events[3].Kind);
        Assert.Equal(2, result.State.Entities[SecondHero].Character!.Armor);
        Assert.False(result.State.Entities[new EntityId(23)].Character!.DivineShield);
        Assert.Equal(1, result.State.Entities[new EntityId(11)].Character!.Health);
    }

    [Fact]
    public void AnArmorOnlyHitProducesADamageOccurrence()
    {
        var simulator = new GameSimulator([
            Hero, Spell("ping") with { OnPlay = [new DamageEffect(DamageTarget.EnemyHero, 1)] },
            Minion("watcher", 0, 1) with
            {
                Triggers = [new(TriggerTiming.Damage, TriggerRelation.Friendly, [new GainArmorEffect(2)])
                {
                    EntityKind = EntityKind.Hero
                }]
            }
        ]);
        var root = simulator.CreateGame([
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) }
        ], [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30) with { Character = new(0, 30, Armor: 3) },
            Card(11, "ping", First), InPlay(21, "watcher", Second, 3, 0, 1)
        ], First);

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.Equal(SimulationEventKind.Damage, result.Events[1].Kind);
        Assert.Equal(1, Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Triggered).Amount);
        Assert.Equal(30, result.State.Entities[SecondHero].Character!.Health);
        Assert.Equal(4, result.State.Entities[SecondHero].Character!.Armor);
    }

    [Fact]
    public void ADrawTriggerCanFillTheHandBeforeTheSecondDrawAttempt()
    {
        var simulator = new GameSimulator([
            Hero, Minion("unit", 0, 1), Spell("draw-two") with { OnPlay = [new DrawEffect(2)] },
            Minion("watcher", 0, 1) with
            {
                Triggers = [new(TriggerTiming.CardDrawn, TriggerRelation.Friendly, [new DrawEffect(1)])]
            }
        ]);
        var hand = Enumerable.Range(40, 8).ToArray();
        var extras = hand.Concat([30, 31, 32]).Select(id => Card(id, "unit", First));
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11, ..hand]), (GameZone.Deck, [30, 31, 32]))
            },
            Player(Second, SecondHero),
            [InPlay(12, "watcher", First, 3, 0, 1), Card(11, "draw-two", First), ..extras]);

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.Equal(new[]
        {
            SimulationEventKind.CardPlayed, SimulationEventKind.CardDrawn, SimulationEventKind.Triggered,
            SimulationEventKind.CardDrawn, SimulationEventKind.CardBurned
        }, result.Events.Select(e => e.Kind));
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new EntityId(32)).Zone);
        Assert.Equal(10, result.State.Players[First].In(GameZone.Hand).Length);
    }

    [Fact]
    public void TheDrawnCardsOwnEffectRunsBeforeObserversAndFatigueDispatchesDamageOnly()
    {
        var simulator = new GameSimulator([
            Hero, Spell("draw-two") with { OnPlay = [new DrawEffect(2)] },
            Minion("drawn", 0, 1) with { WhenDrawn = [new DamageEffect(DamageTarget.FriendlyHero, 2)] },
            Minion("observer", 0, 1) with
            {
                Triggers = [
                    new(TriggerTiming.CardDrawn, TriggerRelation.Friendly, [new GainArmorEffect(3)]),
                    new(TriggerTiming.Damage, TriggerRelation.Friendly, [new GainArmorEffect(1)]) { EntityKind = EntityKind.Hero }
                ]
            }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11]), (GameZone.Deck, [13]))
            },
            Player(Second, SecondHero),
            InPlay(12, "observer", First, 3, 0, 1), Card(11, "draw-two", First), Card(13, "drawn", First));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.Equal(new TriggerTiming?[] { TriggerTiming.Damage, TriggerTiming.CardDrawn, TriggerTiming.Damage },
            result.Events.Where(e => e.Kind == SimulationEventKind.Triggered).Select(e => e.Timing));
        Assert.Single(result.Events, e => e.Kind == SimulationEventKind.DrawnEffect);
        Assert.Equal(28, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(4, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(1, result.State.Players[First].Fatigue);
    }

    [Fact]
    public void AListenerSummonedByTheDrawnCardSeesTheNextDrawOnly()
    {
        var simulator = new GameSimulator([
            Hero, Spell("draw-two") with { OnPlay = [new DrawEffect(2)] },
            Minion("first", 0, 1) with { WhenDrawn = [new SummonEffect("observer")] },
            Minion("observer", 0, 1) with
            {
                Triggers = [new(TriggerTiming.CardDrawn, TriggerRelation.Friendly, [new GainArmorEffect(1)])]
            }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]), (GameZone.Deck, [12, 13])) },
            Player(Second, SecondHero), Card(11, "draw-two", First), Card(12, "first", First), Card(13, "observer", First));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        var trigger = Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Triggered);
        Assert.Equal(new EntityId(13), trigger.Target);
        Assert.Equal(1, result.State.Entities[FirstHero].Character!.Armor);
    }
}
