using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void AnUnsupportedAlternateCostIsNotOfferedForManaPayment()
    {
        var alternate = new SimulationCard(new("alternate", EntityKind.Spell, 0) { AlternateCost = TAG_CARD_ALTERNATE_COST.ALT_TAVERN_COIN });
        var simulator = new GameSimulator([Hero, alternate, Spell("normal")]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero), Card(11, "alternate", First), Card(12, "normal", First));
        var legal = simulator.LegalActions(root).Where(action => action.Kind == SimulationActionKind.PlayCard);
        Assert.Equal(new EntityId(12), Assert.Single(legal).Source);
        var error = Assert.Throws<InvalidOperationException>(() =>
            simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11))));
        Assert.Contains("替代费用", error.Message);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(11)]
    public void TemporaryManaCanExceedTheCapIsSpentFirstAndExpiresAtEndTurn(int cap)
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var first = Player(First, FirstHero) with { Mana = cap, ManaCrystals = cap, MaximumManaCrystals = cap,
            Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12, 13])) };
        var root = Create(simulator, First, first, Player(Second, SecondHero), Card(11, "GAME_005", First),
            Card(12, "EX1_169", First), Card(13, "CS2_091", First));

        var coin = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11))).State;
        var more = simulator.Apply(coin, new(SimulationActionKind.PlayCard, First, new EntityId(12))).State;
        Assert.Equal(cap + 2, more.Players[First].Mana);
        Assert.Equal(2, more.Players[First].TemporaryMana);

        var paid = simulator.Apply(more, new(SimulationActionKind.PlayCard, First, new EntityId(13))).State;
        Assert.Equal(cap + 1, paid.Players[First].Mana);
        Assert.Equal(0, paid.Players[First].ManaSpent);
        Assert.Equal(1, paid.Players[First].TemporaryMana);
        var ended = simulator.Apply(paid, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(cap, ended.Players[First].Mana);
        Assert.Equal(0, ended.Players[First].TemporaryMana);
        Assert.Equal(cap, root.Players[First].Mana);
    }

    [Fact]
    public void CoinsFirstOffsetOverloadDebtAndUnlockingRetainsTemporaryMana()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero, Spell("free-unlock") with { OnPlay = [new UnlockOverloadEffect()] }
        ]);
        var root = Create(simulator, First, Player(First, FirstHero),
            Player(Second, SecondHero) with { OverloadOwed = 4, Zones = Zones((GameZone.Play, [20]), (GameZone.Hand, [21, 22, 23, 24, 25])) },
            Card(21, "GAME_005", Second), Card(22, "GAME_005", Second), Card(23, "GAME_005", Second),
            Card(24, "SCH_427", Second), Card(25, "free-unlock", Second));
        var overloaded = simulator.Apply(root, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(4, overloaded.Players[Second].OverloadLocked);
        Assert.Equal(2, overloaded.Players[Second].ManaCrystals);

        var firstCoin = simulator.Apply(overloaded, new(SimulationActionKind.PlayCard, Second, new EntityId(21))).State;
        var secondCoin = simulator.Apply(firstCoin, new(SimulationActionKind.PlayCard, Second, new EntityId(22))).State;
        var thirdCoin = simulator.Apply(secondCoin, new(SimulationActionKind.PlayCard, Second, new EntityId(23))).State;
        Assert.Equal(0, firstCoin.Players[Second].Mana);
        Assert.Equal(0, secondCoin.Players[Second].Mana);
        Assert.Equal(1, thirdCoin.Players[Second].Mana);

        var bloomed = simulator.Apply(thirdCoin, new(SimulationActionKind.PlayCard, Second, new EntityId(24)));
        Assert.Equal(2, bloomed.State.Players[Second].OverloadOwed);
        Assert.DoesNotContain(bloomed.Events, e => e.Kind == SimulationEventKind.ManaRefreshed);
        var unlocked = simulator.Apply(bloomed.State, new(SimulationActionKind.PlayCard, Second, new EntityId(25))).State;
        Assert.Equal(0, unlocked.Players[Second].OverloadOwed);
        Assert.Equal(0, unlocked.Players[Second].OverloadLocked);
        Assert.Equal(3, unlocked.Players[Second].TemporaryMana);
        Assert.Equal(5, unlocked.Players[Second].Mana);
    }

    [Fact]
    public void LightningBloomRefreshesOnlyRemainingEmptyUnlockedCrystals()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var root = Create(simulator, First,
            Player(First, FirstHero) with { ManaCrystals = 6, ManaSpent = 2, OverloadLocked = 2,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero), Card(11, "GAME_005", First), Card(12, "SCH_427", First));
        var withCoin = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11))).State;
        var bloomed = simulator.Apply(withCoin, new(SimulationActionKind.PlayCard, First, new EntityId(12)));

        Assert.Equal(4, bloomed.State.Players[First].Mana);
        Assert.Equal(2, bloomed.State.Players[First].TemporaryMana);
        Assert.Equal(2, bloomed.State.Players[First].ManaSpent);
        Assert.Equal(2, bloomed.State.Players[First].OverloadLocked);
        Assert.Equal(2, bloomed.State.Players[First].OverloadOwed);
        Assert.Equal(1, Assert.Single(bloomed.Events, e => e.Kind == SimulationEventKind.ManaRefreshed).Amount);
    }

    [Fact]
    public void EmptyRampAtTheCapGeneratesExcessManaWithoutDrawingItFromTheDeck()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 9, ManaCrystals = 9,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12]), (GameZone.Deck, [13])) }, Player(Second, SecondHero),
            Card(11, "BT_130", First), Card(12, "CS2_013", First), Card(13, "CS2_231", First));

        var partial = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));
        Assert.Equal(10, partial.State.Players[First].ManaCrystals);
        Assert.Equal(5, partial.State.Players[First].Mana);
        Assert.DoesNotContain(partial.Events, e => e.Kind == SimulationEventKind.CardGenerated);

        var capped = simulator.Apply(partial.State, new(SimulationActionKind.PlayCard, First, new EntityId(12)));
        var added = Assert.Single(capped.Events, e => e.Kind == SimulationEventKind.CardGenerated);
        var excess = added.Target!.Value;
        Assert.Equal("CS2_013t", capped.State.Entities[excess].CardId);
        Assert.Equal(GameZone.Hand, capped.State.LocationOf(excess).Zone);
        Assert.Equal(GameZone.Deck, capped.State.LocationOf(new EntityId(13)).Zone);
        Assert.DoesNotContain(capped.Events, e => e.Kind == SimulationEventKind.CardDrawn);

        var drawn = simulator.Apply(capped.State, new(SimulationActionKind.PlayCard, First, excess));
        Assert.Equal(new EntityId(13), Assert.Single(drawn.Events, e => e.Kind == SimulationEventKind.CardDrawn).Source);
    }

    [Fact]
    public void TemporaryManaDoesNotCountTowardThePermanentCrystalCapForWildGrowth()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var root = Create(simulator, First,
            Player(First, FirstHero) with { ManaCrystals = 9, ManaSpent = 0, TemporaryMana = 3,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) }, Player(Second, SecondHero), Card(11, "CS2_013", First));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));
        Assert.Equal(10, result.State.Players[First].ManaCrystals);
        Assert.Equal(10, result.State.Players[First].Mana);
        Assert.Equal(1, result.State.Players[First].TemporaryMana);
        Assert.DoesNotContain(result.Events, e => e.Kind == SimulationEventKind.CardGenerated);
    }

    [Fact]
    public void BiologyProjectGrantsFilledCrystalsToBothPlayersAndRefillsSpaceAtTheCap()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 9, ManaCrystals = 9,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Mana = 2, ManaCrystals = 5 }, Card(11, "BOT_054", First));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));
        Assert.Equal(10, result.State.Players[First].ManaCrystals);
        Assert.Equal(10, result.State.Players[First].Mana);
        Assert.Equal(7, result.State.Players[Second].ManaCrystals);
        Assert.Equal(4, result.State.Players[Second].Mana);
        Assert.Equal(new PlayerId?[] { First, Second }, result.Events.Where(e => e.Kind == SimulationEventKind.ManaCrystalsGained).Select(e => e.Player));
    }

    [Fact]
    public void DarnassusAspirantCanLoseACrystalEvenWhenItsBattlecryWasAtTheCap()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero,
            Spell("erase") with { Target = TargetRequirement.FriendlyMinion, OnPlay = [new DestroyMinionEffect()] }
        ]);
        var minion = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 10, ManaCrystals = 10,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) }, Player(Second, SecondHero),
            Card(11, "AT_038", First), Card(12, "erase", First));

        var played = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, minion, Position: 0));
        Assert.DoesNotContain(played.Events, e => e.Kind is SimulationEventKind.ManaCrystalsGained or SimulationEventKind.CardGenerated);
        var killed = simulator.Apply(played.State, new(SimulationActionKind.PlayCard, First, new EntityId(12), minion));
        Assert.Equal(9, killed.State.Players[First].ManaCrystals);
        Assert.Equal(8, killed.State.Players[First].Mana);
        Assert.Equal(1, Assert.Single(killed.Events, e => e.Kind == SimulationEventKind.ManaCrystalsDestroyed).Amount);
    }

    [Fact]
    public void NourishRequiresAnExplicitChoiceAndTheTwoBranchesKeepTheSameRoot()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var spell = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 5, ManaCrystals = 5,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]), (GameZone.Deck, [12, 13, 14])) }, Player(Second, SecondHero),
            Card(11, "EX1_164", First), Card(12, "CS2_231", First), Card(13, "CS2_231", First), Card(14, "CS2_231", First));
        var play = new SimulationAction(SimulationActionKind.PlayCard, First, spell);
        Assert.Equal(new[] { "gainCrystals", "draw" }, simulator.LegalActions(root).Where(action => action.Source == spell).Select(action => action.Choice));
        Assert.Throws<InvalidOperationException>(() => simulator.Apply(root, play));
        Assert.Throws<InvalidOperationException>(() => simulator.Apply(root, play with { Choice = "other" }));

        var gain = simulator.Apply(root, play with { Choice = "gainCrystals" }).State;
        var draw = simulator.Apply(root, play with { Choice = "draw" }).State;
        Assert.Equal(7, gain.Players[First].ManaCrystals);
        Assert.Equal(2, gain.Players[First].Mana);
        Assert.Empty(gain.Players[First].In(GameZone.Hand));
        Assert.Equal(5, draw.Players[First].ManaCrystals);
        Assert.Equal(0, draw.Players[First].Mana);
        Assert.Equal(3, draw.Players[First].In(GameZone.Hand).Length);
        Assert.Equal(GameZone.Hand, root.LocationOf(spell).Zone);
    }

    [Fact]
    public void TwigsDeathrattleRefreshesUnlockedCrystalsWithoutChangingThePermanentPool()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var weapon = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 1, ManaCrystals = 5, OverloadLocked = 2, EquippedWeapon = weapon,
                Zones = Zones((GameZone.Play, [10, 11])) }, Player(Second, SecondHero), Weapon(11, "LOOT_392", First, 3, 1, 1));

        var result = simulator.Apply(root, new(SimulationActionKind.Attack, First, FirstHero, SecondHero));
        Assert.Equal(5, result.State.Players[First].ManaCrystals);
        Assert.Equal(2, result.State.Players[First].OverloadLocked);
        Assert.Equal(3, result.State.Players[First].Mana);
        Assert.Equal(2, Assert.Single(result.Events, e => e.Kind == SimulationEventKind.ManaRefreshed).Amount);
    }
}
