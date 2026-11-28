using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Theory]
    [InlineData(5, false)]
    [InlineData(6, true)]
    public void PayingHealthRequiresOneHealthLeftAndUsesTheModifiedCost(int health, bool canPay)
    {
        var payment = new SimulationCard(new("payment", EntityKind.Spell, 7) { AlternateCost = TAG_CARD_ALTERNATE_COST.HEALTH });
        var simulator = new GameSimulator([Hero, payment]);
        var root = simulator.CreateGame([
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero)
        ], [
            InPlay(10, "hero", First, 1, 0, 30) with { Character = new(0, 30, Damage: 30 - health, Armor: 10, DivineShield: true) },
            InPlay(20, "hero", Second, 2, 0, 30),
            Card(11, "payment", First) with { CostModifiers = [new(FirstHero, -2)] }
        ], First);
        var action = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(11));
        Assert.Equal(canPay, simulator.LegalActions(root).Contains(action));
        if (!canPay)
            return;

        var result = simulator.Apply(root, action);
        var hero = result.State.Entities[FirstHero].Character!;
        Assert.Equal(1, hero.Health);
        Assert.Equal(10, hero.Armor);
        Assert.True(hero.DivineShield);
        Assert.Equal(1, result.State.Players[First].Mana);
        Assert.Equal(new[] { SimulationEventKind.AlternateCostPaid, SimulationEventKind.CardPlayed },
            result.Events.Select(entry => entry.Kind));
        Assert.Equal(TAG_CARD_ALTERNATE_COST.HEALTH, result.Events[0].CostType);
        Assert.Equal(5, result.Events[0].Amount);
    }

    [Fact]
    public void AHeroPowerCanSpendAllAvailableArmor()
    {
        var power = new SimulationCard(new("power", EntityKind.HeroPower, 2) { AlternateCost = TAG_CARD_ALTERNATE_COST.ARMOR })
        {
            OnPlay = [new GainArmorEffect(1)]
        };
        var simulator = new GameSimulator([Hero, power]);
        var root = simulator.CreateGame([
            Player(First, FirstHero) with { Mana = 0, ManaCrystals = 0, Zones = Zones((GameZone.Play, [10]), (GameZone.HeroPower, [11])) },
            Player(Second, SecondHero)
        ], [
            InPlay(10, "hero", First, 1, 0, 30) with { Character = new(0, 30, Armor: 2) },
            InPlay(20, "hero", Second, 2, 0, 30), Card(11, "power", First)
        ], First);
        var action = new SimulationAction(SimulationActionKind.UseHeroPower, First, new EntityId(11));

        Assert.Contains(action, simulator.LegalActions(root));
        var result = simulator.Apply(root, action);
        Assert.Equal(1, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(30, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(1, result.State.Players[First].HeroPowerUsesThisTurn);
        Assert.Equal(new[] { SimulationEventKind.AlternateCostPaid, SimulationEventKind.HeroPowerUsed, SimulationEventKind.ArmorGained },
            result.Events.Select(entry => entry.Kind));
        Assert.Equal(TAG_CARD_ALTERNATE_COST.ARMOR, result.Events[0].CostType);
    }

    [Fact]
    public void OnlyEligibleMinionDeathsFundCorpseCostsAndCountersSurviveRestore()
    {
        var spender = new SimulationCard(new("spender", EntityKind.Spell, 1) { AlternateCost = TAG_CARD_ALTERNATE_COST.CORPSES });
        var simulator = new GameSimulator([
            Hero, spender, Minion("unit", 1, 1), Minion("no-corpse", 1, 1) with { LeavesNoCorpse = true },
            Spell("sweep") with { OnPlay = [new DamageEffect(DamageTarget.AllMinions, 1)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 13, 14]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero), Card(11, "sweep", First), Card(12, "spender", First),
            InPlay(13, "unit", First, 3, 1, 1), InPlay(14, "no-corpse", First, 4, 1, 1));
        var spend = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(12));
        Assert.DoesNotContain(spend, simulator.LegalActions(root));

        var funded = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11))).State;
        Assert.Equal(2, funded.MinionDeaths.Count);
        Assert.Equal(1, funded.Players[First].AvailableCorpses);
        var beforePayment = new GameContinuation(funded.NextEntity, funded.NextPlayOrder, funded.Outcome, funded.Winner);
        var oldSave = simulator.RestoreGame(
            funded.Players.Values.Select(player => player with { Corpses = null }), funded.Entities.Values, funded.ActivePlayer,
            funded.Turn, funded.MinionDeaths, funded.RandomState, beforePayment);
        Assert.Equal(1, oldSave.Players[First].AvailableCorpses);

        var result = simulator.Apply(oldSave, spend);
        var player = result.State.Players[First];
        Assert.Equal(1, player.Corpses);
        Assert.Equal(1, player.CorpsesSpentThisGame);
        Assert.Equal(0, player.AvailableCorpses);
        Assert.Equal(1, player.Mana);
        Assert.Equal(TAG_CARD_ALTERNATE_COST.CORPSES, result.Events[0].CostType);
        var continuation = new GameContinuation(result.State.NextEntity, result.State.NextPlayOrder, result.State.Outcome, result.State.Winner);
        Assert.Equal(0, Restore(simulator, result.State, continuation).Players[First].AvailableCorpses);
    }

    [Fact]
    public void DrawTriggerChangesPaymentForTheDrawnEntityAndPreservesAmountModifiers()
    {
        var observer = Minion("observer", 1, 2) with
        {
            Triggers = [new(TriggerTiming.CardDrawn, TriggerRelation.Friendly,
                [new ChangeCostTypeEffect(HandCostTarget.DrawnCard, TAG_CARD_ALTERNATE_COST.HEALTH)])]
        };
        var simulator = new GameSimulator([
            Hero, observer,
            Spell("draw") with { OnPlay = [new DrawEffect(1)] },
            new SimulationCard(new("target", EntityKind.Spell, 3))
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 13]), (GameZone.Hand, [11]), (GameZone.Deck, [12])) },
            Player(Second, SecondHero), Card(11, "draw", First), Card(12, "target", First), InPlay(13, "observer", First, 3, 1, 2));
        var draw = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));
        var change = Assert.Single(draw.Events, entry => entry.Kind == SimulationEventKind.CostTypeChanged);
        Assert.Equal(new EntityId(13), change.Source);
        Assert.Equal(new EntityId(12), change.Target);

        var continuation = new GameContinuation(draw.State.NextEntity, draw.State.NextPlayOrder, draw.State.Outcome, draw.State.Winner);
        var entities = draw.State.Entities.Values.Select(entity => entity.Id.Value == 12
            ? entity with { CostModifiers = [new(FirstHero, -1)] } : entity);
        var restored = simulator.RestoreGame(draw.State.Players.Values, entities, First, draw.State.Turn,
            draw.State.MinionDeaths, draw.State.RandomState, continuation);
        var play = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(12));
        Assert.Contains(play, simulator.LegalActions(restored));

        var result = simulator.Apply(restored, play);
        Assert.Equal(28, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(1, result.State.Players[First].Mana);
        Assert.Equal(TAG_CARD_ALTERNATE_COST.HEALTH, result.Events[0].CostType);
        Assert.Null(result.State.Entities[play.Source!.Value].CostTypeModifiers);
    }

    [Fact]
    public void TemporaryHandPaymentRestoresEarlierPaymentAtEndOfTurn()
    {
        var convert = Spell("convert") with
        {
            OnPlay = [new ChangeCostTypeEffect(HandCostTarget.FriendlyHand, TAG_CARD_ALTERNATE_COST.MANA, EnchantmentDuration.CurrentTurn)
                { CardFilter = new CardFilter { Kind = EntityKind.Spell } }]
        };
        var payment = new SimulationCard(new("payment", EntityKind.Spell, 1) { AlternateCost = TAG_CARD_ALTERNATE_COST.ARMOR });
        var draw = Spell("draw") with { OnPlay = [new DrawEffect(2)] };
        var self = new SimulationCard(new("self", EntityKind.Spell, 2) { AlternateCost = TAG_CARD_ALTERNATE_COST.ARMOR })
        {
            WhenDrawn = [new ChangeCostTypeEffect(HandCostTarget.Source, TAG_CARD_ALTERNATE_COST.HEALTH)]
        };
        var simulator = new GameSimulator([Hero, draw, convert, payment, self, Minion("unit", 1, 1, cost: 1)]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 13, 14]), (GameZone.Deck, [12, 15])) },
            Player(Second, SecondHero), Card(11, "draw", First), Card(12, "self", First), Card(13, "payment", First),
            Card(14, "unit", First), Card(15, "convert", First));
        var afterDraw = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11))).State;
        var before = simulator.LegalActions(afterDraw);
        Assert.Contains(before, action => action.Source == new EntityId(12));
        Assert.DoesNotContain(before, action => action.Source == new EntityId(13));

        var converted = simulator.Apply(afterDraw, new(SimulationActionKind.PlayCard, First, new EntityId(15)));
        var during = simulator.LegalActions(converted.State);
        Assert.DoesNotContain(during, action => action.Source == new EntityId(12));
        Assert.Contains(during, action => action.Source == new EntityId(13));
        Assert.Null(converted.State.Entities[new EntityId(14)].CostTypeModifiers);
        var continuation = new GameContinuation(converted.State.NextEntity, converted.State.NextPlayOrder,
            converted.State.Outcome, converted.State.Winner);
        var ended = simulator.Apply(Restore(simulator, converted.State, continuation), new(SimulationActionKind.EndTurn, First));
        var changes = ended.Events.Where(entry => entry.Kind == SimulationEventKind.CostTypeChanged).ToArray();
        Assert.Equal(new TAG_CARD_ALTERNATE_COST?[] { TAG_CARD_ALTERNATE_COST.ARMOR, TAG_CARD_ALTERNATE_COST.HEALTH },
            changes.Select(entry => entry.CostType));
        Assert.Null(ended.State.Entities[new EntityId(13)].CostTypeModifiers);
        Assert.Equal(TAG_CARD_ALTERNATE_COST.HEALTH, Assert.Single(ended.State.Entities[new EntityId(12)].CostTypeModifiers!.Value).Type);
        Assert.All(changes, change => Assert.Equal(new EntityId(15), change.Source));
    }

    [Fact]
    public void AnAutomaticallyCastSpellRemainsFreeAndItsPaymentChangeDoesNotStayInTheGraveyard()
    {
        var automatic = new SimulationCard(new("automatic", EntityKind.Spell, 3))
        {
            Keywords = CardKeywords.CastsWhenDrawn,
            WhenDrawn = [new ChangeCostTypeEffect(HandCostTarget.Source, TAG_CARD_ALTERNATE_COST.HEALTH)],
            OnPlay = [new GainArmorEffect(2)]
        };
        var simulator = new GameSimulator([Hero, automatic, Spell("replacement"), Spell("draw") with { OnPlay = [new DrawEffect(1)] }]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]), (GameZone.Deck, [12, 13])) },
            Player(Second, SecondHero), Card(11, "draw", First), Card(12, "automatic", First), Card(13, "replacement", First));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));
        Assert.Contains(result.Events, entry => entry.Kind == SimulationEventKind.SpellCastWhenDrawn);
        Assert.DoesNotContain(result.Events, entry => entry.Kind == SimulationEventKind.AlternateCostPaid);
        Assert.Equal(30, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(2, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new EntityId(12)).Zone);
        Assert.Null(result.State.Entities[new EntityId(12)].CostTypeModifiers);
    }
}
