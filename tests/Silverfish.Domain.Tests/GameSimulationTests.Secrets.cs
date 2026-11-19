using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void CounterspellCancelsOverloadAndOtherSpellTriggersButCostsMana()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero, Minion("watcher", 1, 2) with
            {
                Triggers =
                [
                    new(TriggerTiming.BeforeSpellResolved, TriggerRelation.Friendly, [new GainArmorEffect(1)]),
                    new(TriggerTiming.AfterSpellResolved, TriggerRelation.Friendly, [new GainArmorEffect(2)])
                ]
            }
        ]);
        var counter = new EntityId(21);
        var firstBolt = new EntityId(12);
        var secondBolt = new EntityId(13);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 2, ManaCrystals = 2, Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12, 13])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Secret, [21])) },
            InPlay(11, "watcher", First, 3, 1, 2), Card(12, "EX1_238", First), Card(13, "EX1_238", First), Armed(21, "EX1_287", Second, 4));

        var cancelled = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, firstBolt, SecondHero));
        Assert.Equal(1, cancelled.State.Players[First].Mana);
        Assert.Equal(0, cancelled.State.Players[First].OverloadOwed);
        Assert.Equal(0, cancelled.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(30, cancelled.State.Entities[SecondHero].Character!.Health);
        Assert.Equal(GameZone.Graveyard, cancelled.State.LocationOf(firstBolt).Zone);
        Assert.Equal(GameZone.Graveyard, cancelled.State.LocationOf(counter).Zone);
        Assert.Equal(new[] { SimulationEventKind.CardPlayed, SimulationEventKind.SecretRevealed, SimulationEventKind.SpellCountered },
            cancelled.Events.Select(e => e.Kind));

        var resolved = simulator.Apply(cancelled.State, new(SimulationActionKind.PlayCard, First, secondBolt, SecondHero)).State;
        Assert.Equal(1, resolved.Players[First].OverloadOwed);
        Assert.Equal(3, resolved.Entities[FirstHero].Character!.Armor);
        Assert.Equal(27, resolved.Entities[SecondHero].Character!.Health);
        Assert.Equal(GameZone.Secret, root.LocationOf(counter).Zone);
    }

    [Fact]
    public void CounterspellCanCounterAnOpposingSecretBeforeItArms()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var trap = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 2, ManaCrystals = 2, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Secret, [21])) },
            Card(11, "EX1_610", First), Armed(21, "EX1_287", Second, 3));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, trap));
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(trap).Zone);
        Assert.Empty(result.State.Players[First].In(GameZone.Secret));
        Assert.DoesNotContain(result.Events, e => e.Kind == SimulationEventKind.SecretArmed);
        Assert.Equal(trap, Assert.Single(result.Events, e => e.Kind == SimulationEventKind.SpellCountered).Target);
    }

    [Fact]
    public void SecretZoneAllowsFiveDifferentSecretsAndRejectsDuplicatePlays()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var duplicate = new EntityId(15);
        var fifth = new EntityId(16);
        var sixth = new EntityId(17);
        var first = Player(First, FirstHero) with { Mana = 10, ManaCrystals = 10,
            Zones = Zones((GameZone.Play, [10]), (GameZone.Secret, [11, 12, 13, 14]), (GameZone.Hand, [15, 16, 17])) };
        var second = Player(Second, SecondHero);
        var cards = new[] { Armed(11, "EX1_287", First, 3), Armed(12, "EX1_610", First, 4), Armed(13, "EX1_611", First, 5),
            Armed(14, "EX1_130", First, 6), Card(15, "EX1_610", First), Card(16, "EX1_289", First), Card(17, "EX1_609", First) };
        var root = Create(simulator, First, first, second, cards);
        Assert.DoesNotContain(simulator.LegalActions(root), action => action.Source == duplicate);
        Assert.Throws<ArgumentException>(() => Create(simulator, First, first, second, [.. cards.Take(3), Armed(14, "EX1_610", First, 6), .. cards.Skip(4)]));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, fifth));
        Assert.Equal(5, result.State.Players[First].In(GameZone.Secret).Length);
        Assert.Equal(7, result.State.Entities[fifth].PlayOrder);
        Assert.Equal(new[] { SimulationEventKind.CardPlayed, SimulationEventKind.SecretArmed }, result.Events.Select(e => e.Kind));
        Assert.DoesNotContain(simulator.LegalActions(result.State), action => action.Source == sixth);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void FreezingAndExplosiveTrapRespectPlayOrderAndRecheckForALivingAttacker(bool freezingFirst)
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var wisp = new EntityId(11);
        var freezing = new EntityId(21);
        var explosive = new EntityId(22);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Secret, freezingFirst ? [21, 22] : [22, 21])) },
            InPlay(11, "CS2_231", First, 3, 1, 1), Armed(21, "EX1_611", Second, freezingFirst ? 4 : 5),
            Armed(22, "EX1_610", Second, freezingFirst ? 5 : 4));

        var result = simulator.Apply(root, new(SimulationActionKind.Attack, First, wisp, SecondHero));
        var revealed = result.Events.Where(e => e.Kind == SimulationEventKind.SecretRevealed).Select(e => e.Source);
        Assert.Equal(freezingFirst ? new EntityId?[] { freezing, explosive } : [explosive], revealed);
        Assert.Equal(28, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(30, result.State.Entities[SecondHero].Character!.Health);
        Assert.Equal(freezingFirst ? GameZone.Hand : GameZone.Graveyard, result.State.LocationOf(wisp).Zone);
        Assert.Equal(freezingFirst ? GameZone.Graveyard : GameZone.Secret, result.State.LocationOf(freezing).Zone);
        Assert.Contains(result.Events, e => e.Kind == SimulationEventKind.AttackCancelled);
        if (freezingFirst)
        {
            Assert.Equal(new ManaCostModifier(freezing, 2), Assert.Single(result.State.Entities[wisp].CostModifiers));
            Assert.DoesNotContain(simulator.LegalActions(result.State), action => action.Source == wisp);
        }
    }

    [Fact]
    public void NobleSacrificeRedirectsTheSameAttackAndAllowsSnakeTrapInTheNextProposal()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var attacker = new EntityId(11);
        var snakes = new EntityId(21);
        var noble = new EntityId(22);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Secret, [21, 22])) },
            InPlay(11, "CS2_033", First, 3, 3, 6), Armed(21, "EX1_554", Second, 4), Armed(22, "EX1_130", Second, 5));

        var result = simulator.Apply(root, new(SimulationActionKind.Attack, First, attacker, SecondHero));
        var defender = Assert.Single(result.State.Entities.Values, entity => entity.CardId == "EX1_130a");
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(defender.Id).Zone);
        Assert.Equal(defender.Id, Assert.Single(result.Events, e => e.Kind == SimulationEventKind.AttackRedirected).Target);
        Assert.Equal(4, result.State.Entities[attacker].Character!.Health);
        Assert.Equal(30, result.State.Entities[SecondHero].Character!.Health);
        Assert.Equal(3, result.State.Players[Second].In(GameZone.Play).Count(id => result.State.Entities[id].CardId == "EX1_554t"));
        Assert.Equal(new EntityId?[] { noble, snakes }, result.Events.Where(e => e.Kind == SimulationEventKind.SecretRevealed).Select(e => e.Source));
    }

    [Fact]
    public void NobleSacrificeRemainsArmedWhenTheBoardIsFull()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var board = Enumerable.Range(22, 7).ToArray();
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, .. board]), (GameZone.Secret, [21])) },
            [InPlay(11, "CS2_231", First, 3, 1, 1), Armed(21, "EX1_130", Second, 4),
             .. board.Select(id => InPlay(id, "CS2_231", Second, id - 17, 1, 1))]);

        var result = simulator.Apply(root, new(SimulationActionKind.Attack, First, new EntityId(11), SecondHero));
        Assert.Equal(GameZone.Secret, result.State.LocationOf(new EntityId(21)).Zone);
        Assert.Equal(29, result.State.Entities[SecondHero].Character!.Health);
        Assert.DoesNotContain(result.Events, e => e.Kind == SimulationEventKind.SecretRevealed);
    }

    [Fact]
    public void SnipeIgnoresEffectSummonsAndRunsAfterThePlayedMinionsBattlecry()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero, Spell("summon-body") with { OnPlay = [new SummonEffect("body")] },
            Minion("body", 1, 2) with { OnPlay = [new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, MaximumHealth: 5))] }
        ]);
        var snipe = new EntityId(21);
        var body = new EntityId(12);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Secret, [21])) },
            Card(11, "summon-body", First), Card(12, "body", First), Armed(21, "EX1_609", Second, 3));

        var summoned = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11))).State;
        Assert.Equal(GameZone.Secret, summoned.LocationOf(snipe).Zone);
        var played = simulator.Apply(summoned, new(SimulationActionKind.PlayCard, First, body, Position: 0));
        Assert.Equal(1, played.State.Entities[body].Character!.Health);
        Assert.Equal(GameZone.Graveyard, played.State.LocationOf(snipe).Zone);
        Assert.Equal(new[] { SimulationEventKind.CardPlayed, SimulationEventKind.Summoned, SimulationEventKind.EnchantmentApplied,
            SimulationEventKind.SecretRevealed, SimulationEventKind.Damage }, played.Events.Select(e => e.Kind));
    }

    [Fact]
    public void TruesilverCanHealTheAttackingHeroBetweenExplosiveTrapAndCombat()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var weapon = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { EquippedWeapon = weapon, Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Secret, [21])) },
            Weapon(11, "CS2_097", First, 3, 4, 2), Armed(21, "EX1_610", Second, 4));
        var atTwoHealth = simulator.CreateGame(root.Players.Values,
            root.Entities.Values.Select(entity => entity.Id == FirstHero ? entity with { Character = new(0, 30, Damage: 28) } : entity), First);

        var result = simulator.Apply(atTwoHealth, new(SimulationActionKind.Attack, First, FirstHero, SecondHero));
        Assert.Equal(3, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(26, result.State.Entities[SecondHero].Character!.Health);
        Assert.Equal(1, result.State.Entities[weapon].Weapon!.Durability);
        Assert.Equal(MatchOutcome.InProgress, result.State.Outcome);
    }

    [Fact]
    public void IceBarrierGrantsArmorBeforeCombatAndIgnoresDirectSpellDamage()
    {
        var simulator = new GameSimulator(StarterCardCatalog.Cards.Add(Hero));
        var barrier = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Secret, [21])) },
            InPlay(11, "CS2_231", First, 3, 1, 1), Card(12, "EX1_238", First), Armed(21, "EX1_289", Second, 4));

        var spell = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(12), SecondHero)).State;
        Assert.Equal(27, spell.Entities[SecondHero].Character!.Health);
        Assert.Equal(GameZone.Secret, spell.LocationOf(barrier).Zone);

        var attack = simulator.Apply(spell, new(SimulationActionKind.Attack, First, new EntityId(11), SecondHero));
        Assert.Equal(27, attack.State.Entities[SecondHero].Character!.Health);
        Assert.Equal(7, attack.State.Entities[SecondHero].Character!.Armor);
        Assert.Equal(new[] { SimulationEventKind.Attack, SimulationEventKind.SecretRevealed, SimulationEventKind.ArmorGained,
            SimulationEventKind.Damage }, attack.Events.Select(e => e.Kind));
    }

    private static GameEntity Armed(int id, string cardId, PlayerId controller, long order) =>
        Card(id, cardId, controller) with { PlayOrder = order };
}
