using System.Text.Json;
using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void KingsElekkDrawsTheRevealedEntityOnlyWhenItWinsTheJoust()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards, Hero,
            new SimulationCard(new CardDefinition("small", EntityKind.Minion, 1, 1, 1)),
            new SimulationCard(new CardDefinition("large", EntityKind.Minion, 9, 1, 1))
        ]);
        var state = Create(simulator, First,
            ExpansionPlayer() with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]), (GameZone.Deck, [12, 13])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Deck, [21])) },
            Card(11, "AT_058", First), Card(12, "small", First), Card(13, "large", First), Card(21, "small", Second));

        var action = new SimulationAction(SimulationActionKind.PlayCard, First, new(11), Position: 0);
        var distribution = simulator.ApplyDistribution(state, action);
        Assert.True(distribution.IsExact);
        var winProbability = distribution.Outcomes.Where(outcome => outcome.Result.State.Players[First].In(GameZone.Hand).Length > 0)
            .Sum(outcome => outcome.Probability);
        Assert.Equal(0.5, winProbability);
        foreach (var outcome in distribution.Outcomes)
        {
            var reveal = outcome.Result.Events.Where(entry => entry.Kind == SimulationEventKind.CardRevealed).ToArray();
            Assert.Equal(2, reveal.Length);
            var hand = outcome.Result.State.Players[First].In(GameZone.Hand);
            if (reveal[0].Target == new EntityId(13))
            {
                Assert.Equal(new EntityId(13), Assert.Single(hand));
                Assert.Equal(new EntityId(12), Assert.Single(outcome.Result.State.Players[First].In(GameZone.Deck)));
            }
            else
            {
                Assert.Empty(hand);
                Assert.Equal(2, outcome.Result.State.Players[First].In(GameZone.Deck).Length);
            }
            Assert.Equal(new EntityId(21), Assert.Single(outcome.Result.State.Players[Second].In(GameZone.Deck)));
        }
    }

    [Fact]
    public void GormokChecksFourOtherMinionsBeforeTheBattlecry()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, Minion("unit", 1, 2)]);
        var first = ExpansionPlayer();
        var second = Player(Second, SecondHero);
        var attack = new SimulationAction(SimulationActionKind.PlayCard, First, new(11), SecondHero, Position: 0);
        var withoutFourth = Create(simulator, First,
            first with { Zones = Zones((GameZone.Play, [10, 12, 13, 14]), (GameZone.Hand, [11])) }, second,
            Card(11, "AT_122", First), InPlay(12, "unit", First, 3, 1, 2),
            InPlay(13, "unit", First, 4, 1, 2), InPlay(14, "unit", First, 5, 1, 2));
        Assert.NotNull(simulator.CheckAction(withoutFourth, attack));
        var withoutTarget = attack with { Target = null };
        Assert.Contains(withoutTarget, simulator.LegalActions(withoutFourth));
        Assert.Equal(30, simulator.Apply(withoutFourth, withoutTarget).State.Entities[SecondHero].Character!.Health);

        var withFourth = Create(simulator, First,
            first with { Zones = Zones((GameZone.Play, [10, 12, 13, 14, 15]), (GameZone.Hand, [11])) }, second,
            Card(11, "AT_122", First), InPlay(12, "unit", First, 3, 1, 2), InPlay(13, "unit", First, 4, 1, 2),
            InPlay(14, "unit", First, 5, 1, 2), InPlay(15, "unit", First, 6, 1, 2));
        Assert.Contains(attack, simulator.LegalActions(withFourth));
        Assert.Equal(ErrorType.REQ_TARGET_TO_PLAY, simulator.CheckAction(withFourth, withoutTarget)!.Type);
        Assert.Equal(26, simulator.Apply(withFourth, attack).State.Entities[SecondHero].Character!.Health);
    }

    [Fact]
    public void GarrisonCommanderAllowsTwoUsesAndInspireFollowsTheHeroPower()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var state = Create(simulator, First,
            ExpansionPlayer() with
            {
                Zones = Zones((GameZone.Play, [10, 12, 13]), (GameZone.HeroPower, [11]), (GameZone.Hand, [14]))
            },
            Player(Second, SecondHero), Card(11, "HERO_01bp", First), InPlay(12, "AT_080", First, 3, 2, 3),
            InPlay(13, "AT_082", First, 4, 1, 2), Card(14, "AT_120", First));
        var power = new SimulationAction(SimulationActionKind.UseHeroPower, First, new(11));
        var first = simulator.Apply(state, power);
        var second = simulator.Apply(first.State, power);
        Assert.Equal(4, second.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(3, second.State.Entities[new(13)].Character!.Attack);
        Assert.Equal(2, second.State.Players[First].HeroPowerUsesThisGame);
        Assert.Equal(8, simulator.EffectiveCost(second.State, new(14)));
        Assert.Equal(ErrorType.REQ_NOT_EXHAUSTED_HERO_POWER, simulator.CheckAction(second.State, power)!.Type);
        Assert.True(first.Events.IndexOf(first.Events.First(e => e.Kind == SimulationEventKind.ArmorGained)) <
            first.Events.IndexOf(first.Events.First(e => e.Timing == TriggerTiming.AfterHeroPowerUsed)));
    }

    [Fact]
    public void FinleyWithBrannResumesTwoDiscoverRoundsFromAStoredHeroPowerOffer()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var state = Create(simulator, First,
            ExpansionPlayer() with
            {
                Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11]), (GameZone.HeroPower, [13]))
            },
            Player(Second, SecondHero), Card(11, "LOE_076", First), InPlay(12, "LOE_077", First, 3, 2, 4),
            Card(13, "HERO_01bp", First));
        var played = simulator.Apply(state, new(SimulationActionKind.PlayCard, First, new(11), Position: 1));
        Assert.Equal(2, played.State.PendingPlayChoice!.RemainingSelections);
        var restored = RestoreExpansionChoice(simulator, played.State);
        var invalid = new SimulationAction(SimulationActionKind.ChooseOption, First, new(11), FirstHero);
        Assert.NotNull(simulator.CheckAction(restored, invalid));
        var first = simulator.Apply(restored, simulator.LegalActions(restored)[0]);
        Assert.Equal(1, first.State.PendingPlayChoice!.RemainingSelections);
        Assert.Empty(first.State.PendingPlayChoice.Options.Intersect(played.State.PendingPlayChoice.Options));
        var second = simulator.Apply(first.State, simulator.LegalActions(first.State)[0]);
        Assert.Null(second.State.PendingPlayChoice);
        Assert.Equal(played.State.Players[First].Mana, second.State.Players[First].Mana);
        Assert.Single(second.State.Players[First].In(GameZone.HeroPower));
        Assert.DoesNotContain(second.Events, entry => entry.Kind == SimulationEventKind.CardPlayed);
    }

    [Fact]
    public void FandralOffersCombinedRavenIdolAndResumesOnTheSpellRound()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero], randomMinionPool: ["AT_101", "OG_141", "OG_142"]);
        var state = Create(simulator, First,
            ExpansionPlayer() with { Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero), Card(11, "LOE_115", First), InPlay(12, "OG_044", First, 3, 3, 5));
        var play = Assert.Single(simulator.LegalActions(state).Where(action => action.Source == new EntityId(11)));
        Assert.Equal("both", play.Choice);
        Assert.NotNull(simulator.CheckAction(state, play with { Choice = "LOE_115a" }));
        var pending = simulator.Apply(state, play).State;
        Assert.All(pending.PendingPlayChoice!.Options, id => Assert.Contains(pending.Entities[id].CardId, simulator.RandomMinionPool!.Value));
        var spells = simulator.Apply(pending, simulator.LegalActions(pending)[0]).State;
        Assert.Equal(1, spells.PendingPlayChoice!.DiscoverRound);
        var restored = RestoreExpansionChoice(simulator, spells);
        var done = simulator.Apply(restored, simulator.LegalActions(restored)[0]).State;
        Assert.Null(done.PendingPlayChoice);
        Assert.Equal(GameZone.Graveyard, done.LocationOf(new(11)).Zone);
        Assert.Equal(2, done.Players[First].In(GameZone.Hand).Length);
    }

    [Fact]
    public void CthunCultistGrowthSurvivesSaveAndIsAppliedOnEntry()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var state = Create(simulator, First,
            new PlayerState(First, FirstHero, Mana: 20, ManaCrystals: 20)
            {
                MaximumManaCrystals = 20,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12, 13]))
            },
            Player(Second, SecondHero), Card(11, "OG_293", First), Card(12, "OG_301", First), Card(13, "OG_280", First));
        var cultist = simulator.Apply(state, new(SimulationActionKind.PlayCard, First, new(11), Position: 0)).State;
        var players = JsonSerializer.Deserialize<PlayerState[]>(JsonSerializer.Serialize(cultist.Players.Values))!;
        var restored = simulator.RestoreGame(players, cultist.Entities.Values, cultist.ActivePlayer, cultist.Turn,
            cultist.MinionDeaths, cultist.RandomState, new(cultist.NextEntity, cultist.NextPlayOrder, cultist.Outcome, cultist.Winner));

        var shield = simulator.Apply(restored, new(SimulationActionKind.PlayCard, First, new(12), Position: 1));
        Assert.Equal(10, shield.State.Entities[FirstHero].Character!.Armor);
        var cthun = simulator.Apply(restored, new(SimulationActionKind.PlayCard, First, new(13), Position: 1));
        Assert.Equal(10, cthun.State.Entities[new(13)].Character!.Attack);
        Assert.Equal(10, cthun.State.Entities[new(13)].Character!.Health);
        Assert.Equal(20, cthun.State.Entities[SecondHero].Character!.Health);
    }

    [Fact]
    public void DjinniCastsTheSelectedSpellBranchOnItself()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var state = Create(simulator, First,
            ExpansionPlayer() with { Zones = Zones((GameZone.Play, [10, 12, 13]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero), Card(11, "EX1_155", First),
            InPlay(12, "LOE_053", First, 3, 4, 6), InPlay(13, "AT_101", First, 4, 5, 6));
        var play = new SimulationAction(SimulationActionKind.PlayCard, First, new(11), new(13)) { Choice = "attack" };
        var result = simulator.Apply(state, play);
        Assert.Equal(9, result.State.Entities[new(13)].Character!.Attack);
        Assert.Equal(8, result.State.Entities[new(12)].Character!.Attack);
        Assert.Contains(result.Events, entry => entry.Kind == SimulationEventKind.SpellCastAutomatically && entry.Target == new EntityId(12));
    }

    [Fact]
    public void ConvertSetsTheCostAfterTheCopiedCardsOwnReduction()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var state = Create(simulator, First,
            ExpansionPlayer() with
            {
                HeroPowerUsesThisGame = 4,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "AT_015", First), InPlay(21, "AT_120", Second, 3, 8, 8));
        var result = simulator.Apply(state, new(SimulationActionKind.PlayCard, First, new(11), new(21)));
        var copy = Assert.Single(result.State.Players[First].In(GameZone.Hand));
        Assert.Equal("AT_120", result.State.Entities[copy].CardId);
        Assert.Equal(1, simulator.EffectiveCost(result.State, copy));
    }

    [Fact]
    public void SeaWitchSetsBaseCostBeforeFrostGiantsReduction()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var state = Create(simulator, First,
            ExpansionPlayer() with
            {
                HeroPowerUsesThisGame = 4,
                Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11]))
            },
            Player(Second, SecondHero), Card(11, "AT_120", First), InPlay(12, "LOE_038", First, 3, 5, 5));
        Assert.Equal(1, simulator.EffectiveCost(state, new(11)));
    }

    private static PlayerState ExpansionPlayer() => new(First, FirstHero, Mana: 10, ManaCrystals: 10)
    {
        Zones = Zones((GameZone.Play, [10]))
    };

    private static GameState RestoreExpansionChoice(GameSimulator simulator, GameState state)
    {
        var continuation = new GameContinuation(state.NextEntity, state.NextPlayOrder, state.Outcome, state.Winner)
        {
            PendingPlayChoice = state.PendingPlayChoice
        };
        return simulator.RestoreGame(state.Players.Values, state.Entities.Values, state.ActivePlayer, state.Turn,
            state.MinionDeaths, state.RandomState,
            JsonSerializer.Deserialize<GameContinuation>(JsonSerializer.Serialize(continuation))!);
    }
}
