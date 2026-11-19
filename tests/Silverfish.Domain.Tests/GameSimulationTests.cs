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
