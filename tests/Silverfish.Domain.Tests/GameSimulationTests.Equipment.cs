using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void AWeaponContributesToItsHerosAttackAndTheHeroRemainsTheDamageSource()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var axe = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 4, ManaCrystals = 4,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero), Card(11, "CS2_106", First), Card(12, "CS2_105", First));
        var armed = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, axe)).State;
        var ready = simulator.Apply(armed, new(SimulationActionKind.PlayCard, First, new EntityId(12))).State;
        var result = simulator.Apply(ready, new(SimulationActionKind.Attack, First, FirstHero, SecondHero));

        Assert.Equal(axe, result.State.Players[First].EquippedWeapon);
        Assert.Equal(1, result.State.Entities[axe].Weapon!.Durability);
        Assert.Equal(23, result.State.Entities[SecondHero].Character!.Health);
        Assert.Equal(FirstHero, Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Damage).Source);
        Assert.DoesNotContain(simulator.LegalActions(result.State), a => a.Kind == SimulationActionKind.Attack);
        Assert.Equal(2, armed.Entities[axe].Weapon!.Durability);
        Assert.Null(root.Entities[axe].Weapon);
    }

    [Fact]
    public void EquippedWeaponsAreExcludedAsTargetsAndDoNotRetaliateOnTheOpponentsTurn()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var enemyWeapon = new EntityId(21);
        var minion = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 2, ManaCrystals = 2,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.HeroPower, [12])) },
            Player(Second, SecondHero) with { EquippedWeapon = enemyWeapon, Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "CS2_172", First, 3, 3, 2), Card(12, "HERO_08bp", First), Weapon(21, "CS2_106", Second, 4, 3, 2));

        Assert.DoesNotContain(simulator.LegalActions(root), a => a.Target == enemyWeapon);
        Assert.Equal(0, simulator.EffectiveAttack(root, root.Entities[SecondHero]));
        var struck = simulator.Apply(root, new(SimulationActionKind.Attack, First, minion, SecondHero)).State;
        Assert.Equal(2, struck.Entities[minion].Character!.Health);
        Assert.Equal(2, struck.Entities[enemyWeapon].Weapon!.Durability);

        var playedPower = simulator.Apply(struck, new(SimulationActionKind.UseHeroPower, First, new EntityId(12), SecondHero));
        Assert.Equal(26, playedPower.State.Entities[SecondHero].Character!.Health);
        Assert.Equal(0, playedPower.State.Players[First].Mana);
        Assert.Equal(1, playedPower.State.Players[First].HeroPowerUsesThisTurn);
        Assert.Equal(new[] { SimulationEventKind.HeroPowerUsed, SimulationEventKind.Damage }, playedPower.Events.Select(e => e.Kind));
    }

    [Theory]
    [InlineData(true, 2)]
    [InlineData(false, 1)]
    public void AWeaponAndMinionDestroyedInTheSameCombatResolveDeathrattlesInEntryOrder(bool weaponFirst, int tokenHealth)
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero,
            Minion("victim", 1, 4) with { Deathrattle = [new SummonEffect("token")] }, Minion("token", 1, 2)
        ]);
        var weapon = new EntityId(11);
        var victim = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { EquippedWeapon = weapon, Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Weapon(11, "FP1_021", First, weaponFirst ? 3 : 4, 4, 1),
            InPlay(21, "victim", Second, weaponFirst ? 4 : 3, 1, 4));

        var result = simulator.Apply(root, new(SimulationActionKind.Attack, First, FirstHero, victim));
        Assert.Equal(29, result.State.Entities[FirstHero].Character!.Health);
        Assert.Null(result.State.Players[First].EquippedWeapon);
        Assert.Equal(0, result.State.Entities[weapon].Weapon!.Durability);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(weapon).Zone);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(victim).Zone);
        var token = Assert.Single(result.State.Entities.Values, entity => entity.CardId == "token");
        Assert.Equal(tokenHealth, token.Character!.Health);
        EntityId?[] expected = weaponFirst ? [weapon, victim] : [victim, weapon];
        Assert.Equal(expected, result.Events.Where(e => e.Kind == SimulationEventKind.Deathrattle).Select(e => e.Source));
    }

    [Fact]
    public void PlayingAWeaponResolvesItsBattlecryBeforeTheReplacedWeaponDies()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero,
            new SimulationCard(new("battlecry-weapon", EntityKind.Weapon, 2, 2, PrintedDurability: 2))
            {
                Target = TargetRequirement.EnemyMinion, OnPlay = [new DamageEffect(DamageTarget.Selected, 3)]
            }
        ]);
        var old = new EntityId(11);
        var next = new EntityId(12);
        var target = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 2, ManaCrystals = 2, EquippedWeapon = old,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Weapon(11, "FP1_021", First, 3, 4, 1), Card(12, "battlecry-weapon", First),
            InPlay(21, "EX1_402", Second, 4, 1, 4));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, next, target));

        Assert.Equal(next, result.State.Players[First].EquippedWeapon);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(old).Zone);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(target).Zone);
        Assert.Equal(new EntityId?[] { next, old }, result.Events.Where(e => e.Kind == SimulationEventKind.Damage).Select(e => e.Source));
        Assert.Equal(new[] { 3, 1 }, result.Events.Where(e => e.Kind == SimulationEventKind.Damage).Select(e => e.Amount!.Value));
    }

    [Fact]
    public void TruesilverChampionHealsBeforeRetaliation()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var weapon = new EntityId(11);
        var minion = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { EquippedWeapon = weapon, Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Weapon(11, "CS2_097", First, 3, 4, 2), InPlay(21, "CS2_172", Second, 4, 3, 2));
        var damaged = simulator.CreateGame(root.Players.Values,
            root.Entities.Values.Select(e => e.Id == FirstHero ? e with { Character = e.Character! with { Damage = 28 } } : e), First);

        var result = simulator.Apply(damaged, new(SimulationActionKind.Attack, First, FirstHero, minion));
        Assert.Equal(MatchOutcome.InProgress, result.State.Outcome);
        Assert.Equal(2, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(new[] { SimulationEventKind.Attack, SimulationEventKind.Triggered, SimulationEventKind.Healed,
            SimulationEventKind.Damage, SimulationEventKind.Damage, SimulationEventKind.WeaponDurabilityLost,
            SimulationEventKind.Died }, result.Events.Select(e => e.Kind));
    }

    [Fact]
    public void DaggerMasteryCanReplaceAWeaponAndThePowerReadiesOnItsOwnersNextTurn()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var old = new EntityId(11);
        var power = new SimulationAction(SimulationActionKind.UseHeroPower, First, new EntityId(12));
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 4, ManaCrystals = 4, EquippedWeapon = old,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.HeroPower, [12]), (GameZone.Deck, [13])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Deck, [21])) },
            Weapon(11, "FP1_021", First, 3, 4, 2), Card(12, "HERO_03bp", First), Card(13, "CS2_231", First),
            Card(21, "CS2_231", Second));

        var result = simulator.Apply(root, power);
        var dagger = Assert.IsType<EntityId>(result.State.Players[First].EquippedWeapon);
        Assert.NotEqual(old, dagger);
        Assert.Equal("CS2_082", result.State.Entities[dagger].CardId);
        Assert.Equal(new[] { SimulationEventKind.HeroPowerUsed, SimulationEventKind.WeaponEquipped,
            SimulationEventKind.WeaponDestroyed, SimulationEventKind.Deathrattle }, result.Events.Select(e => e.Kind));
        Assert.DoesNotContain(power, simulator.LegalActions(result.State));
        var secondTurn = simulator.Apply(result.State, new(SimulationActionKind.EndTurn, First)).State;
        var firstTurn = simulator.Apply(secondTurn, new(SimulationActionKind.EndTurn, Second)).State;

        Assert.Contains(power, simulator.LegalActions(firstTurn));
        Assert.Contains(new SimulationAction(SimulationActionKind.Attack, First, FirstHero, SecondHero),
            simulator.LegalActions(firstTurn));
    }

    [Fact]
    public void AcidicSwampOozeDestroysTheOpponentsWeaponAndCanDieToItsDeathrattle()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero, Minion("on-summon", 1, 3) with
            {
                Triggers = [new(TriggerTiming.WhenMinionSummoned, TriggerRelation.Opponent,
                    [new DamageEffect(DamageTarget.TriggerSubject, 1)])]
            }
        ]);
        var ooze = new EntityId(11);
        var weapon = new EntityId(22);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 2, ManaCrystals = 2, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { EquippedWeapon = weapon, Zones = Zones((GameZone.Play, [20, 21, 22])) },
            Card(11, "EX1_066", First), InPlay(21, "on-summon", Second, 3, 1, 3), Weapon(22, "FP1_021", Second, 4, 4, 2));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, ooze, Position: 0));
        Assert.Null(result.State.Players[Second].EquippedWeapon);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(weapon).Zone);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(ooze).Zone);
        Assert.Equal(2, result.State.Entities[weapon].Weapon!.Durability);
        Assert.Equal(1, result.Events.Count(e => e.Kind == SimulationEventKind.WeaponDestroyed));
    }

    [Fact]
    public void AnInitialWeaponMustBeTheControllersEquippedEntity()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var first = Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) };
        var second = Player(Second, SecondHero);
        var weapon = Weapon(11, "CS2_091", First, 3, 1, 4);

        Assert.Throws<ArgumentException>(() => Create(simulator, First, first, second, weapon));
        Assert.Throws<ArgumentException>(() => Create(simulator, First, first with { EquippedWeapon = weapon.Id }, second,
            weapon with { Weapon = weapon.Weapon! with { Durability = 0 } }));
    }

    private static GameEntity Weapon(int id, string cardId, PlayerId controller, int order, int attack, int durability) =>
        Card(id, cardId, controller) with { PlayOrder = order, Weapon = new(attack, durability, Math.Max(durability, 2)) };
}
