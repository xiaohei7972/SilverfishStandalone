using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed class GameSimulationTests
{
    private static readonly PlayerId First = new(1);
    private static readonly PlayerId Second = new(2);
    private static readonly EntityId FirstHero = new(10);
    private static readonly EntityId SecondHero = new(20);
    private static readonly SimulationCard Hero = new(new("hero", EntityKind.Hero, 0, 0, 30));

    [Fact]
    public void DrawPlayAndCombatResolveOnIndependentBranches()
    {
        var simulator = new GameSimulator([
            Hero, Minion("attacker", 3, 2, cost: 2),
            Minion("guard", 2, 1) with
            {
                Keywords = CardKeywords.Taunt,
                Deathrattle = [new DamageEffect(DamageTarget.EnemyHero, 2)]
            }
        ]);
        var root = Create(simulator, Second,
            Player(First, FirstHero) with
            {
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Deck, [12]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "attacker", First, 3, 3, 2), InPlay(21, "guard", Second, 4, 2, 1),
            Card(12, "attacker", First));

        var start = simulator.Apply(root, new(SimulationActionKind.EndTurn, Second));
        Assert.Equal(SimulationEventKind.CardDrawn, start.Events[^1].Kind);
        var played = simulator.Apply(start.State, new(SimulationActionKind.PlayCard, First, new EntityId(12), Position: 0));
        Assert.Equal(new EntityId(12), played.State.Players[First].In(GameZone.Play)[1]);
        Assert.DoesNotContain(simulator.LegalActions(played.State), action =>
            action.Kind == SimulationActionKind.Attack && (action.Source == new EntityId(12) || action.Target == SecondHero));

        var combat = simulator.Apply(played.State, new(SimulationActionKind.Attack, First, new EntityId(11), new EntityId(21)));
        Assert.Equal([
            SimulationEventKind.Attack, SimulationEventKind.Damage, SimulationEventKind.Damage,
            SimulationEventKind.Died, SimulationEventKind.Died, SimulationEventKind.Deathrattle, SimulationEventKind.Damage
        ], combat.Events.Select(e => e.Kind));
        Assert.Equal(28, combat.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(GameZone.Graveyard, combat.State.LocationOf(new EntityId(21)).Zone);
        Assert.Equal(GameZone.Deck, root.LocationOf(new EntityId(12)).Zone);
        Assert.Equal(GameZone.Hand, start.State.LocationOf(new EntityId(12)).Zone);
        Assert.Equal(GameZone.Play, played.State.LocationOf(new EntityId(21)).Zone);
        Assert.Equal(30, played.State.Entities[FirstHero].Character!.Health);
    }

    [Fact]
    public void DeathsResolveInWavesAndUseEntryOrderAcrossPlayers()
    {
        var simulator = new GameSimulator([
            Hero,
            Minion("early", 0, 1) with { Deathrattle = [new DamageEffect(DamageTarget.AllMinions, 1)] },
            Minion("late", 0, 1) with { Deathrattle = [new SummonEffect("token")] },
            Minion("survivor", 0, 2), Minion("token", 0, 1),
            Spell("sweep") with { OnPlay = [new DamageEffect(DamageTarget.AllMinions, 1)] }
        ]);
        var state = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "late", First, 4, 0, 1), InPlay(12, "survivor", First, 5, 0, 2),
            Card(13, "sweep", First), InPlay(21, "early", Second, 3, 0, 1));

        var result = simulator.Apply(state, new(SimulationActionKind.PlayCard, First, new EntityId(13)));

        Assert.Equal(new[] { 21, 11, 12 }, result.Events.Where(e => e.Kind == SimulationEventKind.Died).Select(e => e.Source!.Value.Value));
        Assert.Equal(new[] { 21, 11 }, result.Events.Where(e => e.Kind == SimulationEventKind.Deathrattle).Select(e => e.Source!.Value.Value));
        var survivingToken = Assert.Single(result.State.Players[First].In(GameZone.Play), id => id != FirstHero);
        Assert.Equal("token", result.State.Entities[survivingToken].CardId);
        Assert.Equal(1, result.State.Entities[survivingToken].Character!.Health);
        Assert.Equal(state.NextEntity, survivingToken.Value);
    }

    [Fact]
    public void TakingControlChangesMembershipWithoutChangingIdentityOrEntryOrder()
    {
        var simulator = new GameSimulator([
            Hero, Minion("unit", 3, 2),
            Spell("take") with { Target = TargetRequirement.EnemyMinion, OnPlay = [new TakeControlEffect()] }
        ]);
        var unit = new EntityId(21);
        var state = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "take", First), InPlay(unit.Value, "unit", Second, 3, 3, 2));

        var moved = simulator.Apply(state, new(SimulationActionKind.PlayCard, First, new EntityId(11), unit)).State;

        Assert.Equal(new EntityLocation(First, GameZone.Play), moved.LocationOf(unit));
        Assert.Equal(Second, moved.Entities[unit].Owner);
        Assert.Equal(First, moved.Entities[unit].Controller);
        Assert.Equal(3, moved.Entities[unit].PlayOrder);
        Assert.DoesNotContain(unit, moved.Players[Second].In(GameZone.Play));
        Assert.Contains(unit, state.Players[Second].In(GameZone.Play));
        Assert.DoesNotContain(simulator.LegalActions(moved), action => action.Source == unit);
    }

    [Fact]
    public void FullHandBurnsTopCardWithoutTriggeringFatigue()
    {
        var simulator = new GameSimulator([Hero, Minion("unit", 1, 1)]);
        var hand = Enumerable.Range(21, 10).ToArray();
        var state = Create(simulator, First, Player(First, FirstHero),
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Hand, hand), (GameZone.Deck, [31])) },
            hand.Append(31).Select(id => Card(id, "unit", Second)).ToArray());

        var result = simulator.Apply(state, new(SimulationActionKind.EndTurn, First));

        Assert.Equal(10, result.State.Players[Second].In(GameZone.Hand).Length);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new EntityId(31)).Zone);
        Assert.Equal(0, result.State.Players[Second].Fatigue);
        Assert.Equal(SimulationEventKind.CardBurned, result.Events[^1].Kind);
    }

    [Fact]
    public void SimultaneousHeroDeathsProduceADrawAfterTheDeathPhase()
    {
        var simulator = new GameSimulator([
            Hero, Minion("blast", 1, 1) with { Deathrattle = [new DamageEffect(DamageTarget.EnemyHero, 30)] }
        ]);
        var state = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "blast", First, 3, 1, 1), InPlay(21, "blast", Second, 4, 1, 1));

        var result = simulator.Apply(state, new(SimulationActionKind.Attack, First, new EntityId(11), new EntityId(21)));

        Assert.Equal(MatchOutcome.Draw, result.State.Outcome);
        Assert.Null(result.State.Winner);
        Assert.Empty(simulator.LegalActions(result.State));
        Assert.Equal(SimulationEventKind.MatchEnded, result.Events[^1].Kind);
    }

    [Fact]
    public void RepeatingDeathrattlesReportAResolutionLimitWithoutChangingTheInput()
    {
        var loop = Minion("loop", 1, 1) with
        {
            Deathrattle = [new SummonEffect("loop"), new DamageEffect(DamageTarget.AllMinions, 1)]
        };
        var simulator = new GameSimulator([Hero, loop, Minion("fighter", 1, 2)], resolutionLimit: 12);
        var state = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "fighter", First, 3, 1, 2), InPlay(21, "loop", Second, 4, 1, 1));

        var error = Assert.Throws<InvalidOperationException>(() =>
            simulator.Apply(state, new(SimulationActionKind.Attack, First, new EntityId(11), new EntityId(21))));

        Assert.Contains("resolution budget", error.Message);
        Assert.Equal(GameZone.Play, state.LocationOf(new EntityId(21)).Zone);
        Assert.Equal(4, state.Entities.Count);
    }

    [Fact]
    public void APlayedMinionUsesTheSummonCheckpointsAroundItsBattlecry()
    {
        var simulator = new GameSimulator([
            Hero,
            Minion("watcher", 0, 3) with
            {
                Triggers =
                [
                    new(TriggerTiming.WhenMinionSummoned, TriggerRelation.Opponent,
                        [new DamageEffect(DamageTarget.TriggerSubject, 1)]),
                    new(TriggerTiming.AfterMinionSummoned, TriggerRelation.Opponent,
                        [new DamageEffect(DamageTarget.EnemyHero, 1)])
                ]
            },
            Minion("played", 0, 2) with { OnPlay = [new DamageEffect(DamageTarget.EnemyHero, 3)] }
        ]);
        var source = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "played", First), InPlay(21, "watcher", Second, 3, 0, 3));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, source, Position: 0));

        Assert.Equal(new TriggerTiming?[] { TriggerTiming.WhenMinionSummoned, TriggerTiming.AfterMinionSummoned },
            result.Events.Where(e => e.Kind == SimulationEventKind.Triggered).Select(e => e.Timing));
        Assert.Equal(new EntityId?[] { source, SecondHero, FirstHero },
            result.Events.Where(e => e.Kind == SimulationEventKind.Damage).Select(e => e.Target));
    }

    [Fact]
    public void AMinionKilledDuringEntryStillResolvesItsBattlecryAndSkipsSummonCompletion()
    {
        var simulator = new GameSimulator([
            Hero, Minion("played", 0, 1) with { OnPlay = [new DamageEffect(DamageTarget.EnemyHero, 3)] },
            Minion("watcher", 0, 3) with
            {
                Triggers =
                [
                    new(TriggerTiming.WhenMinionSummoned, TriggerRelation.Opponent,
                        [new DamageEffect(DamageTarget.TriggerSubject, 1)]),
                    new(TriggerTiming.AfterMinionSummoned, TriggerRelation.Opponent,
                        [new DamageEffect(DamageTarget.EnemyHero, 1)])
                ]
            }
        ]);
        var subject = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "played", First), InPlay(21, "watcher", Second, 3, 0, 3));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, subject, Position: 0));

        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(subject).Zone);
        Assert.Equal(27, result.State.Entities[SecondHero].Character!.Health);
        Assert.Equal(30, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(TriggerTiming.WhenMinionSummoned,
            Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Triggered).Timing);
    }

    [Fact]
    public void AListenerCreatedDuringBattlecryCanSeeANewSummonButNotTheOriginalCompletion()
    {
        var simulator = new GameSimulator([
            Hero, Minion("token", 0, 1),
            Minion("listener", 0, 1) with
            {
                Triggers = [new(TriggerTiming.AfterMinionSummoned, TriggerRelation.Friendly,
                    [new DamageEffect(DamageTarget.EnemyHero, 1)])]
            },
            Minion("played", 0, 2) with { OnPlay = [new SummonEffect("listener"), new SummonEffect("token")] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero), Card(11, "played", First));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11), Position: 0));

        var trigger = Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Triggered);
        Assert.Equal(new EntityId(root.NextEntity), trigger.Source);
        Assert.Equal(new EntityId(root.NextEntity + 1), trigger.Target);
        Assert.Equal(29, result.State.Entities[SecondHero].Character!.Health);
    }

    [Fact]
    public void DeathListenersUseTheWaveSnapshotAndShareEntryOrderingWithDeathrattles()
    {
        var deathListener = new CardTrigger(TriggerTiming.MinionDied, TriggerRelation.Friendly,
            [new DamageEffect(DamageTarget.EnemyHero, 1)]);
        var simulator = new GameSimulator([
            Hero,
            Minion("watcher", 0, 3) with { Triggers = [deathListener] },
            Minion("departing", 0, 1) with { Deathrattle = [new SummonEffect("watcher")] },
            Minion("other", 0, 1) with { Triggers = [deathListener] },
            Spell("sweep") with { OnPlay = [new DamageEffect(DamageTarget.AllMinions, 1)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11, 12, 13]), (GameZone.Hand, [14])) },
            Player(Second, SecondHero),
            InPlay(11, "watcher", First, 3, 0, 3), InPlay(12, "departing", First, 4, 0, 1),
            InPlay(13, "other", First, 5, 0, 1), Card(14, "sweep", First));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(14)));

        Assert.Equal(new[] { SimulationEventKind.Triggered, SimulationEventKind.Deathrattle, SimulationEventKind.Triggered },
            result.Events.Where(e => e.Kind is SimulationEventKind.Triggered or SimulationEventKind.Deathrattle).Select(e => e.Kind));
        Assert.All(result.Events.Where(e => e.Kind == SimulationEventKind.Triggered),
            e => Assert.Equal(new EntityId(11), e.Source));
        Assert.Equal(28, result.State.Entities[SecondHero].Character!.Health);
    }

    [Fact]
    public void DeathrattleSummonsUseThePositionAtRemovalWithAnExplicitRightmostOverride()
    {
        var simulator = new GameSimulator([
            Hero, Minion("marker", 0, 3), Minion("replacement", 0, 2), Minion("edge", 0, 2),
            Minion("center", 0, 1) with { Deathrattle = [new SummonEffect("replacement")] },
            Minion("special", 0, 1) with { Deathrattle = [new SummonEffect("edge", SummonPlacement.Rightmost)] },
            Spell("sweep") with { OnPlay = [new DamageEffect(DamageTarget.AllMinions, 1)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11, 12, 13, 14]), (GameZone.Hand, [15])) },
            Player(Second, SecondHero),
            InPlay(11, "special", First, 5, 0, 1), InPlay(12, "marker", First, 3, 0, 3),
            InPlay(13, "center", First, 4, 0, 1), InPlay(14, "marker", First, 6, 0, 3),
            Card(15, "sweep", First));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(15)));

        Assert.Equal(new[] { "hero", "marker", "marker", "replacement", "edge" },
            result.State.Players[First].In(GameZone.Play).Select(id => result.State.Entities[id].CardId));
        Assert.Equal(new EntityId?[] { new(13), new(11) },
            result.Events.Where(e => e.Kind == SimulationEventKind.Deathrattle).Select(e => e.Source));
    }

    [Theory]
    [InlineData(TriggerTiming.TurnStarted)]
    [InlineData(TriggerTiming.TurnEnded)]
    public void LethalAtATurnBoundaryPreventsTheNextDraw(TriggerTiming timing)
    {
        var controller = timing == TriggerTiming.TurnEnded ? First : Second;
        var simulator = new GameSimulator([
            Hero, Minion("watcher", 0, 2) with
            {
                Triggers = [new(timing, TriggerRelation.Friendly, [new DamageEffect(DamageTarget.FriendlyHero, 30)])]
            }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, controller == First ? [10, 11] : [10])) },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, controller == Second ? [20, 11] : [20]), (GameZone.Deck, [21]))
            },
            InPlay(11, "watcher", controller, 3, 0, 2), Card(21, "watcher", Second));

        var result = simulator.Apply(root, new(SimulationActionKind.EndTurn, First));

        Assert.Equal(root.OpponentOf(controller), result.State.Winner);
        Assert.Equal(GameZone.Deck, result.State.LocationOf(new EntityId(21)).Zone);
        Assert.Single(result.Events, e => e.Kind == SimulationEventKind.MatchEnded);
        Assert.Equal(timing == TriggerTiming.TurnEnded ? First : Second, result.State.ActivePlayer);
    }

    [Fact]
    public void NestedSummonsResolveDepthFirstWithoutReenteringAnActiveTrigger()
    {
        var simulator = new GameSimulator([
            Hero, Minion("token", 0, 1), Minion("played", 0, 1),
            Minion("spawner", 0, 3) with { Triggers = [
                new(TriggerTiming.AfterMinionSummoned, TriggerRelation.Friendly,
                    [new SummonEffect("token"), new DamageEffect(DamageTarget.EnemyHero, 2)])
            ] },
            Minion("pinger", 0, 3) with { Triggers = [
                new(TriggerTiming.AfterMinionSummoned, TriggerRelation.Friendly, [new DamageEffect(DamageTarget.EnemyHero, 1)])
            ] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12, 11]), (GameZone.Hand, [13])) },
            Player(Second, SecondHero),
            InPlay(11, "spawner", First, 3, 0, 3), InPlay(12, "pinger", First, 4, 0, 3), Card(13, "played", First));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(13), Position: 2));

        Assert.Equal(new EntityId?[] { new(11), new(12), new(12) },
            result.Events.Where(e => e.Kind == SimulationEventKind.Triggered).Select(e => e.Source));
        Assert.Equal(new int?[] { 1, 2, 1 },
            result.Events.Where(e => e.Kind == SimulationEventKind.Damage).Select(e => e.Amount));
        Assert.Single(result.State.Entities.Values, entity => entity.CardId == "token");
    }

    [Fact]
    public void SpellTriggersSurroundItsEffectAndCleanUpBetweenPhases()
    {
        var simulator = new GameSimulator([
            Hero, Minion("victim", 0, 1),
            Minion("watcher", 0, 3) with
            {
                Triggers =
                [
                    new(TriggerTiming.BeforeSpellResolved, TriggerRelation.Friendly,
                        [new DamageEffect(DamageTarget.AllMinions, 1)]),
                    new(TriggerTiming.AfterSpellResolved, TriggerRelation.Friendly,
                        [new DamageEffect(DamageTarget.EnemyHero, 2)])
                ]
            },
            Spell("rescue") with { Target = TargetRequirement.EnemyMinion, OnPlay = [new TakeControlEffect()] }
        ]);
        var target = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "watcher", First, 3, 0, 3), InPlay(21, "victim", Second, 4, 0, 1),
            Card(12, "rescue", First));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(12), target));

        Assert.Equal(new TriggerTiming?[] { TriggerTiming.BeforeSpellResolved, TriggerTiming.AfterSpellResolved },
            result.Events.Where(e => e.Kind == SimulationEventKind.Triggered).Select(e => e.Timing));
        Assert.Equal(new EntityLocation(Second, GameZone.Graveyard), result.State.LocationOf(target));
        Assert.Equal(28, result.State.Entities[SecondHero].Character!.Health);
        Assert.DoesNotContain(result.Events, e => e.Kind == SimulationEventKind.ControlChanged);
    }

    [Fact]
    public void TargetedMinionIsPlayableWithoutABattlecryTargetButTargetedSpellIsNot()
    {
        var simulator = new GameSimulator([
            Hero,
            Minion("caster", 1, 1) with
            {
                Target = TargetRequirement.EnemyMinion,
                OnPlay = [new DamageEffect(DamageTarget.Selected, 3)]
            },
            Spell("bolt") with
            {
                Target = TargetRequirement.EnemyMinion,
                OnPlay = [new DamageEffect(DamageTarget.Selected, 3)]
            }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero), Card(11, "caster", First), Card(12, "bolt", First));

        var action = Assert.Single(simulator.LegalActions(root), a => a.Kind == SimulationActionKind.PlayCard);
        var result = simulator.Apply(root, action);

        Assert.Equal(new EntityId(11), action.Source);
        Assert.Null(action.Target);
        Assert.Equal(GameZone.Play, result.State.LocationOf(new EntityId(11)).Zone);
        Assert.DoesNotContain(result.Events, e => e.Kind == SimulationEventKind.Damage);
    }

    private static GameState Create(GameSimulator simulator, PlayerId active, PlayerState first, PlayerState second,
        params GameEntity[] cards) => simulator.CreateGame([first, second], [
            InPlay(10, "hero", First, 1, 0, 30), InPlay(20, "hero", Second, 2, 0, 30), ..cards
        ], active);

    private static PlayerState Player(PlayerId id, EntityId hero) =>
        new(id, hero, Mana: 1, ManaCrystals: 1) { Zones = Zones((GameZone.Play, [hero.Value])) };

    private static ImmutableDictionary<GameZone, ImmutableArray<EntityId>> Zones(params (GameZone Zone, int[] Ids)[] zones) =>
        zones.ToImmutableDictionary(zone => zone.Zone, zone => zone.Ids.Select(id => new EntityId(id)).ToImmutableArray());

    private static GameEntity Card(int id, string name, PlayerId player) => new(new EntityId(id), name, player, player);

    private static GameEntity InPlay(int id, string name, PlayerId player, long order, int attack, int health) =>
        Card(id, name, player) with { PlayOrder = order, Character = new(attack, health) };

    private static SimulationCard Minion(string id, int attack, int health, int cost = 0) =>
        new(new CardDefinition(id, EntityKind.Minion, cost, attack, health));

    private static SimulationCard Spell(string id) => new(new CardDefinition(id, EntityKind.Spell, 0));
}
