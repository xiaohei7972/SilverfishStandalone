using System.Text.Json;
using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void BookWyrmChecksDragonAndEffectiveAttackBeforePlay()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var first = KarazhanPlayer(First, FirstHero);
        var second = KarazhanPlayer(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21, 22])) };
        var target = new SimulationAction(SimulationActionKind.PlayCard, First, new(11), new(21), 0);
        var noDragon = Board(false);
        Assert.Contains(target with { Target = null }, simulator.LegalActions(noDragon));
        Assert.NotNull(simulator.CheckAction(noDragon, target));

        var withDragon = Board(true);
        Assert.Contains(target, simulator.LegalActions(withDragon));
        Assert.Equal(ErrorType.REQ_TARGET_MAX_ATTACK, simulator.CheckAction(withDragon, target with { Target = new(22) })!.Type);
        Assert.Equal(ErrorType.REQ_TARGET_TO_PLAY, simulator.CheckAction(withDragon, target with { Target = null })!.Type);
        Assert.Equal(GameZone.Graveyard, simulator.Apply(withDragon, target).State.LocationOf(new(21)).Zone);

        GameState Board(bool dragon) => Create(simulator, First,
            first with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) }, second,
            Card(11, "KAR_033", First), Card(12, dragon ? "AT_017" : "KAR_300", First),
            InPlay(21, "KAR_011", Second, 3, 3, 2), InPlay(22, "AT_101", Second, 4, 5, 6));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CatTrickWaitsForTheOpposingSpellToResolve(bool countered)
    {
        var spell = Spell("sweep") with { OnPlay = [new DamageEffect(DamageTarget.AllMinions, 4)] };
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, spell]);
        var cards = new List<GameEntity>
        {
            Card(11, "sweep", First), Card(21, "KAR_004", Second) with { PlayOrder = 4 },
            InPlay(22, "KAR_011", Second, 3, 3, 2)
        };
        if (countered)
            cards.Add(Card(23, "EX1_287", Second) with { PlayOrder = 5 });
        var state = Create(simulator, First,
            KarazhanPlayer(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            KarazhanPlayer(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 22]), (GameZone.Secret, countered ? [21, 23] : [21]))
            }, [.. cards]);
        var result = simulator.Apply(state, new(SimulationActionKind.PlayCard, First, new(11)));
        if (countered)
        {
            Assert.Equal(GameZone.Secret, result.State.LocationOf(new(21)).Zone);
            Assert.Equal(GameZone.Play, result.State.LocationOf(new(22)).Zone);
        }
        else
        {
            var reveal = Assert.Single(result.Events, entry => entry.Kind == SimulationEventKind.SecretRevealed);
            var death = Assert.Single(result.Events, entry => entry.Kind == SimulationEventKind.Died);
            Assert.True(result.Events.IndexOf(death) < result.Events.IndexOf(reveal));
            var cat = Assert.Single(result.State.Entities.Values, entity => entity.CardId == "KAR_004a");
            Assert.Equal(GameZone.Play, result.State.LocationOf(cat.Id).Zone);
            Assert.Equal(2, cat.Character!.Health);
        }
    }

    [Fact]
    public void AtieshUsesTheDiscountedSpellCostAfterRestoringDiscover()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero], randomMinionPool: ["CS2_231", "KAR_011"]);
        var weapon = new EntityId(12);
        var spell = new EntityId(11);
        var state = Create(simulator, First,
            KarazhanPlayer(First, FirstHero) with
            {
                EquippedWeapon = weapon,
                Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11, 13]))
            }, KarazhanPlayer(Second, SecondHero),
            Card(11, "LOE_115", First) with { CostModifiers = [new(FirstHero, -1)] },
            Card(12, "KAR_097t", First) with { Weapon = new(1, 1, 3), PlayOrder = 3 },
            Card(13, "KAR_711", First));
        var played = simulator.Apply(state, new(SimulationActionKind.PlayCard, First, spell) { Choice = "LOE_115b" });
        Assert.Equal(0, played.State.PendingPlayChoice!.PlayedCardCost);
        Assert.Equal(1, played.State.Entities[weapon].Weapon!.Durability);
        var restored = RestoreKarazhanState(simulator, played.State);
        var result = simulator.Apply(restored, simulator.LegalActions(restored)[0]);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(weapon).Zone);
        Assert.Contains(result.State.Players[First].In(GameZone.Play), id => result.State.Entities[id].CardId == "CS2_231");
        Assert.DoesNotContain(result.State.Players[First].In(GameZone.Play), id => result.State.Entities[id].CardId == "KAR_011");
        Assert.Equal(11, simulator.EffectiveCost(result.State, new(13)));
    }

    [Fact]
    public void AtieshKeepsItsDurabilityWithNoRoomForTheSummon()
    {
        var spell = Spell("test-spell");
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, spell], randomMinionPool: ["CS2_231"]);
        var weapon = new EntityId(12);
        var minions = Enumerable.Range(13, 7).Select(id => InPlay(id, "KAR_011", First, id, 3, 2)).ToArray();
        var player = KarazhanPlayer(First, FirstHero) with
        {
            EquippedWeapon = weapon,
            Zones = Zones((GameZone.Play, [10, 12, .. minions.Select(minion => minion.Id.Value)]), (GameZone.Hand, [11]))
        };
        var state = Create(simulator, First, player, KarazhanPlayer(Second, SecondHero),
            [Card(11, "test-spell", First), Card(12, "KAR_097t", First) with { Weapon = new(1, 2, 3), PlayOrder = 3 }, .. minions]);
        var result = simulator.Apply(state, new(SimulationActionKind.PlayCard, First, new(11)));
        Assert.Equal(2, result.State.Entities[weapon].Weapon!.Durability);
        Assert.DoesNotContain(result.Events, entry => entry.Kind == SimulationEventKind.Summoned);
    }

    [Fact]
    public void IvoryKnightHealsForTheChosenSpellsPrintedCostAfterRestore()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var state = simulator.CreateGame(
        [
            KarazhanPlayer(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            KarazhanPlayer(Second, SecondHero),
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30) with { Character = new(0, 30, Damage: 20) },
            InPlay(20, "hero", Second, 2, 0, 30), Card(11, "KAR_057", First),
        ], First);

        var played = simulator.Apply(state, new(SimulationActionKind.PlayCard, First, new(11), Position: 0)).State;
        Assert.Equal(10, played.Entities[FirstHero].Character!.Health);
        var restored = RestoreKarazhanState(simulator, played);
        Assert.Equal(simulator.LegalActions(played).ToArray(), simulator.LegalActions(restored).ToArray());
        var choice = simulator.LegalActions(restored).First(move =>
            move.Target is EntityId option && CompiledCardData.Bind(restored.Entities[option].CardId, new()).Definition.PrintedCost > 0);
        var cardId = restored.Entities[choice.Target!.Value].CardId;
        var cost = CompiledCardData.Bind(cardId, new()).Definition.PrintedCost;
        var result = simulator.Apply(restored, choice).State;
        Assert.Null(result.PendingPlayChoice);
        Assert.Equal(Math.Min(30, 10 + cost), result.Entities[FirstHero].Character!.Health);
        Assert.Contains(result.Players[First].In(GameZone.Hand), id => result.Entities[id].CardId == cardId);
    }

    [Fact]
    public void FirelandsPortalCanDealSixDamageToAFriendlyMinionAndSummonASixCostMinion()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero], randomMinionPool: ["KAR_041"]);
        var state = Create(simulator, First,
            KarazhanPlayer(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11])) },
            KarazhanPlayer(Second, SecondHero), Card(11, "KAR_076", First), InPlay(12, "KAR_011", First, 3, 3, 2));
        var play = new SimulationAction(SimulationActionKind.PlayCard, First, new(11), new(12));
        Assert.Contains(play, simulator.LegalActions(state));
        Assert.Equal(ErrorType.REQ_TARGET_TO_PLAY, simulator.CheckAction(state, play with { Target = null })!.Type);
        var result = simulator.Apply(state, play);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new(12)).Zone);
        Assert.Equal(6, Assert.Single(result.Events, entry => entry.Kind == SimulationEventKind.Damage).Amount);
        Assert.Contains(result.State.Players[First].In(GameZone.Play), id => result.State.Entities[id].CardId == "KAR_041");
    }

    [Fact]
    public void MedivhsValetRequiresATargetOnlyWhileControllingASecret()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var play = new SimulationAction(SimulationActionKind.PlayCard, First, new(11), FirstHero, 0);
        var withoutSecret = Board(false);
        Assert.Contains(play with { Target = null }, simulator.LegalActions(withoutSecret));
        Assert.NotNull(simulator.CheckAction(withoutSecret, play));

        var withSecret = Board(true);
        Assert.Contains(play, simulator.LegalActions(withSecret));
        Assert.Equal(ErrorType.REQ_TARGET_TO_PLAY, simulator.CheckAction(withSecret, play with { Target = null })!.Type);
        Assert.Equal(27, simulator.Apply(withSecret, play).State.Entities[FirstHero].Character!.Health);

        GameState Board(bool secret) => Create(simulator, First,
            KarazhanPlayer(First, FirstHero) with
            {
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]), (GameZone.Secret, secret ? [12] : []))
            }, KarazhanPlayer(Second, SecondHero), secret
                ? [Card(11, "KAR_092", First), Card(12, "EX1_287", First) with { PlayOrder = 3 }]
                : [Card(11, "KAR_092", First)]);
    }

    [Fact]
    public void MoatLurkerResummonsTheCapturedCardForItsPreviousControllerAfterRestore()
    {
        var destroy = Spell("destroy") with { Target = TargetRequirement.Minion, OnPlay = [new DestroyMinionEffect()] };
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, destroy]);
        var state = Create(simulator, First,
            KarazhanPlayer(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12, 13])) },
            KarazhanPlayer(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Hand, [22, 23])) },
            Card(11, "KAR_041", First), Card(12, "destroy", First), Card(13, "KAR_013", First),
            InPlay(21, "KAR_011", Second, 3, 6, 7), Card(22, "CS1_113", Second), Card(23, "destroy", Second));
        var captured = simulator.Apply(state, new(SimulationActionKind.PlayCard, First, new(11), new(21), 0)).State;
        Assert.Equal(GameZone.Graveyard, captured.LocationOf(new(21)).Zone);
        var restored = RestoreKarazhanState(simulator, captured);
        var kill = new SimulationAction(SimulationActionKind.PlayCard, First, new(12), new(11));
        var result = simulator.Apply(restored, kill).State;
        var returned = Assert.Single(result.Players[Second].In(GameZone.Play).Where(id => id != SecondHero));
        Assert.NotEqual(new EntityId(21), returned);
        Assert.Equal("KAR_011", result.Entities[returned].CardId);
        Assert.Equal(2, result.Entities[returned].Character!.Health);

        var silenced = simulator.Apply(restored, new(SimulationActionKind.PlayCard, First, new(13), new(11))).State;
        Assert.Single(simulator.Apply(silenced, kill).State.Players[Second].In(GameZone.Play));

        var enemyTurn = simulator.Apply(restored, new(SimulationActionKind.EndTurn, First)).State;
        var stolen = simulator.Apply(enemyTurn, new(SimulationActionKind.PlayCard, Second, new(22), new(11))).State;
        Assert.Equal(Second, stolen.Entities[new(11)].Controller);
        var afterControlChanged = simulator.Apply(stolen, new(SimulationActionKind.PlayCard, Second, new(23), new(11))).State;
        Assert.Contains(afterControlChanged.Players[Second].In(GameZone.Play), id => afterControlChanged.Entities[id].CardId == "KAR_011");
    }

    [Fact]
    public void BarnesCreatesAOneOneWithoutPlayingTheOriginal()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var state = Create(simulator, First,
            KarazhanPlayer(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 13]), (GameZone.Hand, [11]), (GameZone.Deck, [12])) },
            KarazhanPlayer(Second, SecondHero), Card(11, "KAR_114", First), Card(12, "KAR_030a", First),
            InPlay(13, "KAR_011", First, 3, 3, 2));
        var result = simulator.Apply(state, new(SimulationActionKind.PlayCard, First, new(11), Position: 0)).State;
        Assert.Equal(new EntityId(12), Assert.Single(result.Players[First].In(GameZone.Deck)));
        var summoned = Assert.Single(result.Players[First].In(GameZone.Play), id => result.Entities[id].CardId == "KAR_030a");
        Assert.Equal(new EntityId[] { new(11), summoned, new(13) },
            result.Players[First].In(GameZone.Play).Where(id => id != FirstHero));
        Assert.DoesNotContain(result.Players[First].In(GameZone.Play), id => result.Entities[id].CardId == "KAR_030");
        Assert.Equal("KAR_030a", result.Entities[summoned].CardId);
        Assert.Equal(1, result.Entities[summoned].Character!.Attack);
        Assert.Equal(1, result.Entities[summoned].Character!.Health);
        Assert.Equal("KAR_114e", Assert.Single(result.Entities[summoned].Enchantments).CardId);
    }

    [Fact]
    public void PrinceAddsDistinctClassLegalLegendariesOnlyAtOpening()
    {
        string[] pool = ["KAR_096", "KAR_044", "KAR_061", "KAR_097", "KAR_114", "OG_133", "OG_280", "EX1_016", "AT_009"];
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero], randomMinionPool: pool);
        PlayerState[] players =
        [
            KarazhanPlayer(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12]), (GameZone.Deck, [13])) },
            KarazhanPlayer(Second, SecondHero)
        ];
        GameEntity[] cards =
        [
            InPlay(10, "hero", First, 1, 0, 30), InPlay(20, "hero", Second, 2, 0, 30),
            Card(11, "KAR_096", First), Card(12, "OG_280", First), Card(13, "KAR_044", First)
        ];
        var snapshot = simulator.CreateGame(players, cards, First);
        Assert.Single(snapshot.Players[First].In(GameZone.Deck));
        var opened = simulator.CreateGame(players, cards, First, resolveStartOfGame: true);
        var ids = opened.Players[First].In(GameZone.Deck).Select(id => opened.Entities[id].CardId).ToArray();
        Assert.Equal(6, ids.Length);
        Assert.Equal(6, ids.Distinct().Count());
        Assert.DoesNotContain("OG_280", ids);
        Assert.DoesNotContain("AT_009", ids);
        Assert.Equal(opened.Players[First].In(GameZone.Deck).ToArray(),
            RestoreKarazhanState(simulator, opened).Players[First].In(GameZone.Deck).ToArray());
    }

    [Fact]
    public void FoolsBaneAllowsRepeatedMinionAttacksUntilItBreaks()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, Minion("training", 0, 20)]);
        var state = Create(simulator, First,
            KarazhanPlayer(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])), EquippedWeapon = new(11) },
            KarazhanPlayer(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "KAR_028", First) with { Weapon = new(3, 2, 4), PlayOrder = 3 },
            InPlay(21, "training", Second, 4, 0, 20));
        var attack = new SimulationAction(SimulationActionKind.Attack, First, FirstHero, new(21));
        Assert.NotNull(simulator.CheckAction(state, attack with { Target = SecondHero }));
        state = simulator.Apply(state, attack).State;
        Assert.Contains(attack, simulator.LegalActions(state));
        state = simulator.Apply(state, attack).State;
        Assert.Equal(14, state.Entities[new(21)].Character!.Health);
        Assert.Null(state.Players[First].EquippedWeapon);
        Assert.DoesNotContain(attack, simulator.LegalActions(state));
    }

    [Fact]
    public void ZoobotUsesDifferentRecipientsForOverlappingTypes()
    {
        var hybrid = Minion("hybrid", 1, 2);
        hybrid = hybrid with { Definition = hybrid.Definition with { MinionTypes = MinionType.Beast | MinionType.Dragon } };
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, hybrid]);
        var state = Create(simulator, First,
            KarazhanPlayer(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12, 13, 14]), (GameZone.Hand, [11])) },
            KarazhanPlayer(Second, SecondHero), Card(11, "KAR_095", First), InPlay(12, "hybrid", First, 3, 1, 2),
            InPlay(13, "KAR_300", First, 4, 2, 2), InPlay(14, "OG_006a", First, 5, 1, 1));
        var result = simulator.Apply(state, new(SimulationActionKind.PlayCard, First, new(11), Position: 0)).State;
        foreach (var id in new[] { 12, 13, 14 })
            Assert.Equal("KAR_095e", Assert.Single(result.Entities[new(id)].Enchantments).CardId);
    }

    private static PlayerState KarazhanPlayer(PlayerId id, EntityId hero) => new(id, hero, Mana: 10, ManaCrystals: 10)
    {
        Zones = Zones((GameZone.Play, [hero.Value]))
    };

    private static GameState RestoreKarazhanState(GameSimulator simulator, GameState state)
    {
        var continuation = new GameContinuation(state.NextEntity, state.NextPlayOrder, state.Outcome, state.Winner)
        {
            PendingPlayChoice = state.PendingPlayChoice
        };
        var options = new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };
        return simulator.RestoreGame(
            JsonSerializer.Deserialize<PlayerState[]>(JsonSerializer.Serialize(state.Players.Values, options), options)!,
            JsonSerializer.Deserialize<GameEntity[]>(JsonSerializer.Serialize(state.Entities.Values, options), options)!,
            state.ActivePlayer, state.Turn, state.MinionDeaths, state.RandomState,
            JsonSerializer.Deserialize<GameContinuation>(JsonSerializer.Serialize(continuation, options), options)!);
    }
}
