using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void NaxxBatch2EchoingOozeMadScientistDeathlordWebspinnerKelThuzadAndTwinBossesWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones(
                    (GameZone.Play, [10]),
                    (GameZone.Hand, [11, 12, 13, 14, 15, 16]),
                    (GameZone.Deck, [31, 30]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 10, ManaCrystals = 10, MaximumManaCrystals = 10,
                Zones = Zones(
                    (GameZone.Play, [20, 21]),
                    (GameZone.Deck, [40, 41]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(21, "CS2_231", Second, 3, 10, 30), // Large enemy minion for trading
            Card(11, "FP1_003", First), // Echoing Ooze
            Card(12, "CS2_092", First), // Blessing of Kings (+4/+4)
            Card(13, "FP1_004", First), // Mad Scientist
            Card(14, "FP1_009", First), // Deathlord
            Card(15, "FP1_011", First), // Webspinner
            Card(16, "FP1_013", First), // Kel'Thuzad
            Card(30, "FP1_020", First), // Avenge (Secret in First's deck)
            Card(31, "CS2_231", First), // Wisp (drawn on turn start)
            Card(40, "CS2_182", Second), // Chillwind Yeti (minions in Second's deck)
            Card(41, "CS2_231", Second)
        ], First);

        // Play Echoing Ooze (11) and buff it with Blessing of Kings (+4/+4 -> 5/6)
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(11), Position: 0)).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(12), new(11))).State;
        Assert.Equal(5, s.Entities[new(11)].Character!.Attack);
        Assert.Equal(6, s.Entities[new(11)].Character!.Health);

        // Play Mad Scientist (13), Deathlord (14), Webspinner (15), Kel'Thuzad (16)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(13), Position: 1)).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14), Position: 2)).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15), Position: 3)).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16), Position: 4)).State;

        // End First's turn -> Echoing Ooze copies itself (also 5/6!)
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        var oozes = s.Players[First].In(GameZone.Play).Where(id => s.Entities[id].CardId == "FP1_003").ToArray();
        Assert.Equal(2, oozes.Length);
        Assert.All(oozes, id =>
        {
            Assert.Equal(5, s.Entities[id].Character!.Attack);
            Assert.Equal(6, s.Entities[id].Character!.Health);
        });

        // End Second's turn -> Echoing Ooze does NOT copy again on subsequent turns
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, Second)).State;
        Assert.Equal(2, s.Players[First].In(GameZone.Play).Count(id => s.Entities[id].CardId == "FP1_003"));

        // Trade Mad Scientist (13) into 21 -> Mad Scientist dies, puts Avenge (30) into Secret zone!
        s = simulator.Apply(s, new(SimulationActionKind.Attack, First, new(13), new(21))).State;
        Assert.Contains(new EntityId(30), s.Players[First].In(GameZone.Secret));

        // Trade Deathlord (14) into 21 -> Opponent recruits a minion from deck into play!
        var enemyBoardBefore = s.Players[Second].In(GameZone.Play).Length;
        s = simulator.Apply(s, new(SimulationActionKind.Attack, First, new(14), new(21))).State;
        Assert.Equal(enemyBoardBefore + 1, s.Players[Second].In(GameZone.Play).Length);

        // Trade Webspinner (15) into 21 -> First gets a random Beast in hand!
        var handBefore = s.Players[First].In(GameZone.Hand).Length;
        s = simulator.Apply(s, new(SimulationActionKind.Attack, First, new(15), new(21))).State;
        Assert.Equal(handBefore + 1, s.Players[First].In(GameZone.Hand).Length);
        var generatedBeastId = s.Players[First].In(GameZone.Hand)[^1];
        var generatedBeastDef = CompiledCardData.Bind(s.Entities[generatedBeastId].CardId, new()).Definition;
        Assert.Equal(EntityKind.Minion, generatedBeastDef.Kind);
        var generatedTypes = generatedBeastDef.MinionTypes.GetValueOrDefault();
        Assert.True(generatedTypes.HasFlag(MinionType.Beast) || generatedTypes.HasFlag(MinionType.All));

        // End First's turn -> Kel'Thuzad resummons Mad Scientist, Deathlord, Webspinner that died this turn!
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Contains(s.Players[First].In(GameZone.Play), id => s.Entities[id].CardId == "FP1_004");
        Assert.Contains(s.Players[First].In(GameZone.Play), id => s.Entities[id].CardId == "FP1_009");
        Assert.Contains(s.Players[First].In(GameZone.Play), id => s.Entities[id].CardId == "FP1_011");

        // Stalagg & Feugen: first death does not summon Thaddius; second death summons 11/11 Thaddius!
        var twinRoot = simulator.CreateGame(
        [
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11, 12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "FP1_014", First, 3, 7, 4), // Stalagg
            InPlay(12, "FP1_015", First, 4, 4, 7), // Feugen
            InPlay(21, "CS2_231", Second, 5, 10, 30)
        ], First);
        var afterFirstTwin = simulator.Apply(twinRoot, new(SimulationActionKind.Attack, First, new(11), new(21))).State;
        Assert.DoesNotContain(afterFirstTwin.Players[First].In(GameZone.Play), id => afterFirstTwin.Entities[id].CardId == "FP1_014t");
        var afterSecondTwin = simulator.Apply(afterFirstTwin, new(SimulationActionKind.Attack, First, new(12), new(21))).State;
        var thaddiusId = Assert.Single(afterSecondTwin.Players[First].In(GameZone.Play), id => afterSecondTwin.Entities[id].CardId == "FP1_014t");
        Assert.Equal(11, afterSecondTwin.Entities[thaddiusId].Character!.Attack);
        Assert.Equal(11, afterSecondTwin.Entities[thaddiusId].Character!.Health);
    }

    [Fact]
    public void NaxxBatch2WailingSoulWeblordDuplicatePoisonSeedsVoidcallerReincarnateAmbusherUndertakerAndBaronWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones(
                    (GameZone.Play, [10, 11]),
                    (GameZone.Hand, [12, 13, 14, 15, 16, 17, 18, 19]),
                    (GameZone.Deck, [30]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 10, ManaCrystals = 10, MaximumManaCrystals = 10,
                Zones = Zones(
                    (GameZone.Play, [20, 21]),
                    (GameZone.Hand, [22, 23]),
                    (GameZone.Deck, [40]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "FP1_028", First, 3, 1, 2), // Undertaker
            Card(12, "FP1_007", First), // Nerubian Egg (Deathrattle -> triggers Undertaker!)
            Card(13, "FP1_031", First), // Baron Rivendare
            Card(14, "FP1_025", First), // Reincarnate
            Card(15, "FP1_017", First), // Nerub'ar Weblord
            Card(16, "FP1_016", First), // Wailing Soul
            Card(17, "FP1_018", First), // Duplicate
            Card(18, "FP1_022", First), // Voidcaller
            Card(19, "CS2_064", First), // Dread Infernal (Demon in hand)
            InPlay(21, "CS2_231", Second, 4, 10, 10),
            Card(22, "CS2_117", Second), // Earthen Ring Farseer (Battlecry, base cost 3 -> becomes 5 with Weblord)
            Card(23, "CS2_182", Second), // Chillwind Yeti (no Battlecry, stays 4)
            Card(30, "CS2_231", First),
            Card(40, "CS2_231", Second)
        ], First);

        // Summon Nerubian Egg (12) -> Undertaker (11) gains +1/+1 (2/3)
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(12), Position: 0)).State;
        Assert.Equal(2, s.Entities[new(11)].Character!.Attack);
        Assert.Equal(3, s.Entities[new(11)].Character!.Health);

        // Play Baron Rivendare (13, Legendary)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(13), Position: 2)).State;

        // Verify Reincarnate (14) legal targets include Egg (12) and Undertaker (11) but NOT Legendary Baron (13)
        var reincarnateLegals = simulator.LegalActions(s)
            .Where(a => a.Kind == SimulationActionKind.PlayCard && a.Source == new EntityId(14))
            .Select(a => a.Target!.Value)
            .ToHashSet();
        Assert.Contains(new EntityId(12), reincarnateLegals);
        Assert.DoesNotContain(new EntityId(13), reincarnateLegals);

        // Cast Reincarnate on Nerubian Egg (12) while Baron (13) is in play -> destroys Egg (summons TWO 4/4 Nerubians) + resummons Egg!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14), new(12))).State;
        Assert.Equal(2, s.Players[First].In(GameZone.Play).Count(id => s.Entities[id].CardId == "FP1_014t" || s.Entities[id].CardId == "FP1_007t"));
        Assert.Equal(2, s.Players[First].In(GameZone.Play).Count(id => s.Entities[id].CardId == "FP1_007t"));
        Assert.Contains(s.Players[First].In(GameZone.Play), id => s.Entities[id].CardId == "FP1_007");

        // Play Nerub'ar Weblord (15) -> Enemy Earthen Ring Farseer (22) costs 3+2=5; Yeti (23) stays 4
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15), Position: 0)).State;
        Assert.Equal(5, simulator.EffectiveCost(s, new(22)));
        Assert.Equal(4, simulator.EffectiveCost(s, new(23)));

        // Play Wailing Soul (16) -> silences other friendly minions (Undertaker, Baron, Weblord, etc.)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16), Position: 0)).State;
        Assert.True(s.Entities[new(11)].Silenced);
        Assert.True(s.Entities[new(13)].Silenced);
        Assert.True(s.Entities[new(15)].Silenced);
        Assert.False(s.Entities[new(16)].Silenced);
        // Silenced Weblord no longer increases Earthen Ring Farseer's cost
        Assert.Equal(3, simulator.EffectiveCost(s, new(22)));

        // Test Voidcaller + Duplicate + Anub'ar Ambusher + Poison Seeds in clean board
        var sub = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones(
                    (GameZone.Play, [10, 11, 12]),
                    (GameZone.Hand, [13, 14, 15]),
                    (GameZone.Deck, [30]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 10, ManaCrystals = 10, MaximumManaCrystals = 10,
                Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Deck, [40]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "FP1_022", First, 3, 3, 4), // Voidcaller
            InPlay(12, "FP1_026", First, 4, 5, 5), // Anub'ar Ambusher
            Card(13, "FP1_018", First), // Duplicate
            Card(14, "CS2_064", First), // Dread Infernal (Demon in hand)
            Card(15, "FP1_019", First), // Poison Seeds
            InPlay(21, "CS2_231", Second, 5, 10, 20),
            Card(30, "CS2_231", First),
            Card(40, "CS2_231", Second)
        ], First);

        // Play Duplicate (13)
        sub = simulator.Apply(sub, new(SimulationActionKind.PlayCard, First, new(13))).State;
        // End First's turn -> Second attacks Voidcaller (11) -> Duplicate adds 2 FP1_022 to First's hand AND Voidcaller summons Dread Infernal (14)!
        sub = simulator.Apply(sub, new(SimulationActionKind.EndTurn, First)).State;
        sub = simulator.Apply(sub, new(SimulationActionKind.Attack, Second, new(21), new(11))).State;
        Assert.Equal(2, sub.Players[First].In(GameZone.Hand).Count(id => sub.Entities[id].CardId == "FP1_022"));
        Assert.Contains(new EntityId(14), sub.Players[First].In(GameZone.Play));

        // End Second's turn -> First attacks Ambusher (12) into 21 -> Ambusher dies and bounces Dread Infernal (14) to hand!
        sub = simulator.Apply(sub, new(SimulationActionKind.EndTurn, Second)).State;
        sub = simulator.Apply(sub, new(SimulationActionKind.Attack, First, new(12), new(21))).State;
        Assert.Contains(new EntityId(14), sub.Players[First].In(GameZone.Hand));

        // Cast Poison Seeds (15) -> destroys enemy 21 (summons 1 Treant for Second) and any friendly minions -> equal Treants!
        var seedRoot = simulator.CreateGame(
        [
            Player(First, FirstHero) with { Mana = 10, ManaCrystals = 10, Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "CS2_182", First, 3, 4, 5),
            InPlay(12, "CS2_231", First, 4, 1, 1),
            InPlay(21, "CS2_182", Second, 5, 4, 5),
            Card(13, "FP1_019", First)
        ], First);
        var afterSeeds = simulator.Apply(seedRoot, new(SimulationActionKind.PlayCard, First, new(13))).State;
        Assert.Equal(2, afterSeeds.Players[First].In(GameZone.Play).Count(id => afterSeeds.Entities[id].CardId == "FP1_019t"));
        Assert.Equal(1, afterSeeds.Players[Second].In(GameZone.Play).Count(id => afterSeeds.Entities[id].CardId == "FP1_019t"));
    }
}
