using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void RushRespectsTauntAndCanAttackTheHeroFromTheNextTurn()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var commander = new EntityId(11);
        var taunt = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 4, ManaCrystals = 4,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]), (GameZone.Deck, [12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21, 22]), (GameZone.Deck, [23])) },
            Card(11, "GIL_803", First), Card(12, "CS2_231", First), InPlay(21, "hexfrog", Second, 3, 0, 1),
            InPlay(22, "CS2_231", Second, 4, 1, 1), Card(23, "CS2_231", Second));

        var entered = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, commander, Position: 0)).State;
        var attack = Assert.Single(simulator.LegalActions(entered), action => action.Kind == SimulationActionKind.Attack);
        Assert.Equal(new SimulationAction(SimulationActionKind.Attack, First, commander, taunt), attack);
        Assert.Equal(5, entered.Entities[commander].Character!.Attack);

        var traded = simulator.Apply(entered, attack).State;
        Assert.DoesNotContain(simulator.LegalActions(traded), action => action.Kind == SimulationActionKind.Attack);
        var next = simulator.Apply(traded, new(SimulationActionKind.EndTurn, First)).State;
        var ready = simulator.Apply(next, new(SimulationActionKind.EndTurn, Second)).State;
        Assert.Equal(2, ready.Entities[commander].Character!.Attack);
        Assert.Contains(new SimulationAction(SimulationActionKind.Attack, First, commander, SecondHero), simulator.LegalActions(ready));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void FrozenSummoningSickRushMinionThawsOnlyIfItHadAMinionToAttack(bool enemyHasMinion)
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var commander = new EntityId(11);
        var first = Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) };
        var second = Player(Second, SecondHero);
        var minion = InPlay(11, "GIL_803", First, 3, 2, 5) with { Character = new(2, 5, Exhausted: true) { Frozen = true } };
        var root = enemyHasMinion
            ? Create(simulator, First, first, second with { Zones = Zones((GameZone.Play, [20, 21])) },
                minion, InPlay(21, "CS2_231", Second, 4, 1, 1))
            : Create(simulator, First, first, second, minion);

        Assert.DoesNotContain(simulator.LegalActions(root), action => action.Source == commander);
        var ended = simulator.Apply(root, new(SimulationActionKind.EndTurn, First));
        Assert.Equal(!enemyHasMinion, ended.State.Entities[commander].Character!.Frozen);
        Assert.Equal(enemyHasMinion, ended.Events.Any(e => e.Kind == SimulationEventKind.Thawed && e.Target == commander));
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, false)]
    public void WindfuryOnlyThawsThatTurnIfAnAttackWasLeft(int attacksSpent, bool thawsImmediately)
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var harpy = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 2, ManaCrystals = 2,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12]), (GameZone.Deck, [13])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Deck, [21, 22])) },
            InPlay(11, "EX1_033", First, 3, 4, 5) with { Character = new(4, 5, AttacksThisTurn: attacksSpent) },
            Card(12, "CS2_024", First), Card(13, "CS2_231", First), Card(21, "CS2_231", Second), Card(22, "CS2_231", Second));

        var frozen = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(12), harpy)).State;
        Assert.True(frozen.Entities[harpy].Character!.Frozen);
        Assert.DoesNotContain(simulator.LegalActions(frozen), action => action.Kind == SimulationActionKind.Attack);
        var ended = simulator.Apply(frozen, new(SimulationActionKind.EndTurn, First));
        Assert.Equal(!thawsImmediately, ended.State.Entities[harpy].Character!.Frozen);

        var next = simulator.Apply(ended.State, new(SimulationActionKind.EndTurn, Second)).State;
        Assert.Equal(thawsImmediately, simulator.LegalActions(next).Any(action => action.Source == harpy && action.Kind == SimulationActionKind.Attack));
        var cleaned = simulator.Apply(next, new(SimulationActionKind.EndTurn, First)).State;
        Assert.False(cleaned.Entities[harpy].Character!.Frozen);
    }

    [Fact]
    public void AHeroFrozenOnTheOpponentsTurnCanUseItsPowerAndThawsEvenWithoutAttack()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 2, ManaCrystals = 2, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.HeroPower, [21]), (GameZone.Deck, [22])) },
            Card(11, "CS2_024", First), Card(21, "HERO_01bp", Second), Card(22, "CS2_231", Second));

        var frozen = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11), SecondHero)).State;
        var secondTurn = simulator.Apply(frozen, new(SimulationActionKind.EndTurn, First)).State;
        Assert.True(secondTurn.Entities[SecondHero].Character!.Frozen);
        var armored = simulator.Apply(secondTurn, new(SimulationActionKind.UseHeroPower, Second, new EntityId(21))).State;
        Assert.Equal(2, armored.Entities[SecondHero].Character!.Armor);
        var ended = simulator.Apply(armored, new(SimulationActionKind.EndTurn, Second));
        Assert.Contains(ended.Events, e => e.Kind == SimulationEventKind.Thawed && e.Target == SecondHero);
        Assert.False(ended.State.Entities[SecondHero].Character!.Frozen);
    }

    [Fact]
    public void BlizzardFreezesAShieldedMinionAfterDamageAndSilenceClearsFreeze()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero,
            Minion("shield", 1, 3) with { Keywords = CardKeywords.DivineShield }
        ]);
        var shield = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 6, ManaCrystals = 6, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21, 22])) },
            Card(11, "CS2_028", First), Card(12, "EX1_332", First),
            InPlay(21, "shield", Second, 3, 1, 3) with { Character = new(1, 3, DivineShield: true) },
            InPlay(22, "CS2_033", Second, 4, 3, 6));

        var frozen = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));
        Assert.True(frozen.State.Entities[shield].Character!.Frozen);
        Assert.False(frozen.State.Entities[shield].Character!.DivineShield);
        Assert.Equal(3, frozen.State.Entities[shield].Character!.Health);
        Assert.Equal(4, frozen.State.Entities[new EntityId(22)].Character!.Health);
        Assert.Equal(new[] { SimulationEventKind.CardPlayed, SimulationEventKind.ShieldBroken, SimulationEventKind.Damage,
            SimulationEventKind.Frozen, SimulationEventKind.Frozen }, frozen.Events.Select(e => e.Kind));

        var silenced = simulator.Apply(frozen.State, new(SimulationActionKind.PlayCard, First, new EntityId(12), shield)).State;
        Assert.False(silenced.Entities[shield].Character!.Frozen);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WaterElementalFreezesOnlyWhenItsHitDealsDamageEvenIfItDiesInCombat(bool hasShield)
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, Minion("defender", 6, 5)]);
        var elemental = new EntityId(11);
        var defender = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "CS2_033", First, 3, 3, 6),
            InPlay(21, "defender", Second, 4, 6, 5) with { Character = new(6, 5, DivineShield: hasShield) });

        var result = simulator.Apply(root, new(SimulationActionKind.Attack, First, elemental, defender));
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(elemental).Zone);
        Assert.Equal(!hasShield, result.State.Entities[defender].Character!.Frozen);
        Assert.Equal(!hasShield, result.Events.Any(e => e.Kind == SimulationEventKind.Frozen && e.Target == defender));
    }

    [Fact]
    public void DoomhammerChangesTheAttackLimitWithoutResettingTheAttackCounter()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var axe = new EntityId(11);
        var hammer = new EntityId(12);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 5, ManaCrystals = 5, EquippedWeapon = axe,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12])) }, Player(Second, SecondHero),
            Weapon(11, "CS2_106", First, 3, 3, 2), Card(12, "EX1_567", First));
        var attack = new SimulationAction(SimulationActionKind.Attack, First, FirstHero, SecondHero);

        var spent = simulator.Apply(root, attack).State;
        Assert.DoesNotContain(attack, simulator.LegalActions(spent));
        var equipped = simulator.Apply(spent, new(SimulationActionKind.PlayCard, First, hammer));
        Assert.Equal(new[] { SimulationEventKind.CardPlayed, SimulationEventKind.OverloadScheduled,
            SimulationEventKind.WeaponEquipped, SimulationEventKind.WeaponDestroyed }, equipped.Events.Select(e => e.Kind));
        Assert.Equal(2, equipped.State.Players[First].OverloadOwed);
        Assert.Contains(attack, simulator.LegalActions(equipped.State));

        var twice = simulator.Apply(equipped.State, attack).State;
        Assert.Equal(25, twice.Entities[SecondHero].Character!.Health);
        Assert.Equal(7, twice.Entities[hammer].Weapon!.Durability);
        Assert.DoesNotContain(attack, simulator.LegalActions(twice));
    }

    [Fact]
    public void OverloadStacksCarriesToTheOwnersTurnAndLavaShockClearsBothBuckets()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 5, ManaCrystals = 5,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12, 13, 14]), (GameZone.Deck, [15, 16])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Deck, [21, 22])) },
            Card(11, "EX1_238", First), Card(12, "EX1_241", First), Card(13, "EX1_238", First), Card(14, "BRM_011", First),
            Card(15, "CS2_231", First), Card(16, "CS2_231", First), Card(21, "CS2_231", Second), Card(22, "CS2_231", Second));

        var bolt = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11), SecondHero)).State;
        var burst = simulator.Apply(bolt, new(SimulationActionKind.PlayCard, First, new EntityId(12), SecondHero)).State;
        Assert.Equal(1, burst.Players[First].Mana);
        Assert.Equal(3, burst.Players[First].OverloadOwed);
        var opponent = simulator.Apply(burst, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(2, opponent.Players[Second].Mana);
        var locked = simulator.Apply(opponent, new(SimulationActionKind.EndTurn, Second)).State;
        Assert.Equal(6, locked.Players[First].ManaCrystals);
        Assert.Equal(3, locked.Players[First].Mana);
        Assert.Equal(3, locked.Players[First].OverloadLocked);
        Assert.Equal(0, locked.Players[First].OverloadOwed);

        var extraBolt = simulator.Apply(locked, new(SimulationActionKind.PlayCard, First, new EntityId(13), SecondHero)).State;
        var shock = simulator.Apply(extraBolt, new(SimulationActionKind.PlayCard, First, new EntityId(14), SecondHero));
        Assert.Equal(3, shock.State.Players[First].Mana);
        Assert.Equal(0, shock.State.Players[First].OverloadLocked);
        Assert.Equal(0, shock.State.Players[First].OverloadOwed);
        Assert.Equal(3, Assert.Single(shock.Events, e => e.Kind == SimulationEventKind.OverloadUnlocked).Amount);
        Assert.Equal(0, root.Players[First].OverloadOwed);
    }

    [Fact]
    public void AnOverloadLargerThanTheManaPoolStillLetsAFreeEffectUnlockTheFullPool()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero, Spell("free-unlock") with { OnPlay = [new UnlockOverloadEffect()] }
        ]);
        var free = new EntityId(21);
        var priced = new EntityId(22);
        var root = Create(simulator, First, Player(First, FirstHero),
            Player(Second, SecondHero) with { OverloadOwed = 9, Zones = Zones((GameZone.Play, [20]), (GameZone.Hand, [21, 22])) },
            Card(21, "free-unlock", Second), Card(22, "CS2_172", Second));

        var locked = simulator.Apply(root, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(0, locked.Players[Second].Mana);
        Assert.Equal(9, locked.Players[Second].OverloadLocked);
        Assert.DoesNotContain(simulator.LegalActions(locked), action => action.Source == priced);

        var unlocked = simulator.Apply(locked, new(SimulationActionKind.PlayCard, Second, free)).State;
        Assert.Equal(2, unlocked.Players[Second].Mana);
        Assert.Contains(simulator.LegalActions(unlocked), action => action.Source == priced);
    }
}
