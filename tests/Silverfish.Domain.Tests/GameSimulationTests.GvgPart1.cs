using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void GvgSparePartsWorkIncludingUntilNextTurnStealthAndStatSwap()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones(
                    (GameZone.Play, [10, 11, 12]),
                    (GameZone.Hand, [31, 32, 33, 34, 35, 36, 37]),
                    (GameZone.Deck, [40, 41]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Deck, [42, 43]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "GVG_044", First, 3, 3, 4), // Spider Tank 3/4
            InPlay(12, "CS2_231", First, 4, 1, 1), // Wisp 1/1
            InPlay(21, "CS2_182", Second, 5, 4, 5), // Enemy Yeti 4/5
            Card(31, "PART_001", First), // Armor Plating (+1 HP)
            Card(32, "PART_007", First), // Whirling Blades (+1 Atk)
            Card(33, "PART_006", First), // Reversing Switch (swap Atk/HP)
            Card(34, "PART_003", First), // Rusty Horn (Taunt)
            Card(35, "PART_004", First), // Finicky Cloakfield (Stealth until next turn)
            Card(36, "PART_005", First), // Emergency Coolant (Freeze)
            Card(37, "PART_002", First), // Time Rewinder (return to hand)
            Card(40, "CS2_231", First),
            Card(41, "CS2_231", First),
            Card(42, "CS2_231", Second),
            Card(43, "CS2_231", Second)
        ], First);

        // Buff Spider Tank (3/4 -> 3/5 -> 4/5 -> swap -> 5/4) + Taunt + Stealth until next turn
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(31), new(11))).State;
        Assert.Equal(5, s.Entities[new(11)].Character!.Health);
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(32), new(11))).State;
        Assert.Equal(4, s.Entities[new(11)].Character!.Attack);
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(33), new(11))).State;
        Assert.Equal(5, s.Entities[new(11)].Character!.Attack);
        Assert.Equal(4, s.Entities[new(11)].Character!.Health);
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(34), new(11))).State;
        Assert.Contains(s.Entities[new(11)].Enchantments, e => e.GrantedTag == GAME_TAG.TAUNT);
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(35), new(11))).State;
        Assert.Contains(s.Entities[new(11)].Enchantments, e => e.GrantedTag == GAME_TAG.STEALTH);

        // Freeze enemy Yeti (21), return friendly Wisp (12) to hand
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(36), new(21))).State;
        Assert.True(s.Entities[new(21)].Character!.Frozen);
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(37), new(12))).State;
        Assert.Equal(GameZone.Hand, s.LocationOf(new(12)).Zone);

        // Stealth remains during opponent's turn and clears when First's next turn starts
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Contains(s.Entities[new(11)].Enchantments, e => e.GrantedTag == GAME_TAG.STEALTH);
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, Second)).State;
        Assert.DoesNotContain(s.Entities[new(11)].Enchantments, e => e.GrantedTag == GAME_TAG.STEALTH);
    }

    [Fact]
    public void GvgPart1MagePriestWarlockWarriorShamanAndPaladinMechanicsWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones(
                    (GameZone.Play, [10, 11, 12, 13]),
                    (GameZone.Hand, [30, 31, 32, 33, 34, 35, 36, 37, 38, 42]),
                    (GameZone.Deck, [50, 51, 52, 53]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 10, ManaCrystals = 10, MaximumManaCrystals = 10,
                Zones = Zones((GameZone.Play, [20, 21, 22]), (GameZone.Hand, [60]), (GameZone.Deck, [61, 62]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30) with { Character = new(0, 30, Damage: 6) }, // 24 HP
            InPlay(11, "GVG_002", First, 3, 2, 3), // Snowchugger (Mech)
            InPlay(12, "GVG_045t", First, 4, 1, 1), // Friendly Imp (Demon)
            InPlay(13, "GVG_064", First, 5, 3, 2), // Puddlestomper (Murloc)
            InPlay(21, "CS2_182", Second, 6, 4, 5), // Enemy Yeti 4/5
            InPlay(22, "GVG_044", Second, 7, 3, 4), // Enemy Spider Tank 3/4
            Card(30, "GVG_010", First), // Velen's Chosen (+2/+4 & Spell Damage +1)
            Card(31, "GVG_012", First), // Light of the Naaru
            Card(32, "GVG_014", First), // Vol'jin
            Card(33, "GVG_019", First), // Demonheart
            Card(34, "GVG_008", First), // Lightbomb
            Card(35, "GVG_005", First), // Echo of Medivh
            Card(36, "GVG_003", First), // Unstable Portal
            Card(37, "GVG_021", First), // Mal'Ganis
            Card(38, "GVG_052", First), // Crush
            Card(42, "GVG_063", First), // Bolvar Fordragon (in hand)
            Card(50, "GVG_007", First), // Flame Leviathan (WhenDrawn: 2 dmg to non-Mechs)
            Card(51, "CS2_231", First),
            Card(52, "CS2_231", First),
            Card(53, "CS2_231", First),
            Card(60, "CS2_231", Second),
            Card(61, "CS2_231", Second),
            Card(62, "CS2_231", Second)
        ], First);

        // Snowchugger (11) attacks enemy Yeti (21) -> Yeti takes 2 dmg (left at 3 HP) AND is Frozen!
        // Snowchugger (11) takes 4 combat damage and dies -> Bolvar (42) in hand gains +1 Attack!
        var s = simulator.Apply(root, new(SimulationActionKind.Attack, First, new(11), new(21))).State;
        Assert.True(s.Entities[new(21)].Character!.Frozen);
        Assert.Equal(3, s.Entities[new(21)].Character!.Health);
        Assert.Equal(1, s.Entities[new(42)].Enchantments.Sum(e => e.Modifier?.Attack ?? 0));

        // Velen's Chosen (30) on Puddlestomper (13: 3/2 -> 5/6 + Spell Damage +1)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(30), new(13))).State;
        Assert.Equal(5, s.Entities[new(13)].Character!.Attack);
        Assert.Equal(6, s.Entities[new(13)].Character!.Health);

        // Light of the Naaru (31): heals 3 (Spell Damage does not boost healing); SecondHero 24 -> 27 (still damaged -> summons EX1_001!)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(31), SecondHero)).State;
        Assert.Equal(27, s.Entities[SecondHero].Character!.Health);
        Assert.Contains(s.Players[First].In(GameZone.Play), id => s.Entities[id].CardId == "EX1_001");

        // Demonheart (33) on friendly Imp (12) -> +5/+5 (6/6)!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(33), new(12))).State;
        Assert.Equal(6, s.Entities[new(12)].Character!.Attack);
        Assert.Equal(6, s.Entities[new(12)].Character!.Health);

        // Vol'jin (32, 6/2) swaps health with Imp (12, 6/6) -> Vol'jin becomes 6/6 and Imp becomes 6/2 (both undamaged)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(32), new(12), Position: 0)).State;
        Assert.Equal(6, s.Entities[new(32)].Character!.Health);
        Assert.Equal(2, s.Entities[new(12)].Character!.Health);
        Assert.Equal(7, simulator.EffectiveCost(s, new(38)));

        // Puddlestomper (13, 5/6) attacks Yeti (21, 4/3) -> becomes damaged (5/2), so Crush (38) costs 7 - 4 = 3!
        s = simulator.Apply(s, new(SimulationActionKind.Attack, First, new(13), new(21))).State;
        Assert.Equal(3, simulator.EffectiveCost(s, new(38)));
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(38), new(22))).State;
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(22)).Zone);

        // Echo of Medivh (35) copies friendly board minions to hand
        var handBeforeEcho = s.Players[First].In(GameZone.Hand).Length;
        var boardBeforeEcho = s.Players[First].In(GameZone.Play).Count(id => id != FirstHero);
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(35))).State;
        Assert.Equal(handBeforeEcho - 1 + boardBeforeEcho, s.Players[First].In(GameZone.Hand).Length);

        // Mal'Ganis (37) -> hero Immune + friendly Imp (12) gets +2/+2 aura!
        var impAtk = s.Entities[new(12)].Character!.Attack;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(37), Position: 0)).State;
        Assert.Equal(impAtk + 2, s.Entities[new(12)].Character!.Attack);

        // Unstable Portal (36) from root adds a random minion costing 3 less
        var handBeforePortal = root.Players[First].In(GameZone.Hand).Length;
        var sPortal = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(36))).State;
        var portalCard = sPortal.Players[First].In(GameZone.Hand)[^1];
        Assert.Equal(handBeforePortal, sPortal.Players[First].In(GameZone.Hand).Length);
        var portalDef = CompiledCardData.Bind(sPortal.Entities[portalCard].CardId, new()).Definition;
        Assert.Equal(EntityKind.Minion, portalDef.Kind);
        Assert.Equal(-3, Assert.Single(sPortal.Entities[portalCard].CostModifiers).Delta);
        Assert.Equal(Math.Max(0, portalDef.PrintedCost - 3), simulator.EffectiveCost(sPortal, portalCard));

        // End turn -> First Hero takes 0 fatigue/spell? No, Second draws, then Second ends -> First draws Flame Leviathan (50)!
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        var enemyHeroBeforeLeviathan = s.Entities[SecondHero].Character!.Health;
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, Second)).State;
        // Mal'Ganis (37) protects FirstHero (still 30 HP!), SecondHero takes 2 damage!
        Assert.Equal(30, s.Entities[FirstHero].Character!.Health);
        Assert.Equal(enemyHeroBeforeLeviathan - 2, s.Entities[SecondHero].Character!.Health);
    }

    [Fact]
    public void GvgPart1MusterQuartermasterAndBolvarPlayWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [39, 40, 42]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(21, "CS2_182", Second, 3, 4, 5),
            Card(39, "GVG_061", First),
            Card(40, "GVG_060", First),
            Card(42, "GVG_063", First)
        ], First);

        // Muster for Battle (39) + Quartermaster (40)
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(39))).State;
        Assert.NotNull(s.Players[First].EquippedWeapon);
        Assert.Equal(3, s.Players[First].In(GameZone.Play).Count(id => s.Entities[id].CardId == "CS2_101t"));
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(40), Position: 0)).State;
        Assert.All(
            s.Players[First].In(GameZone.Play).Where(id => s.Entities[id].CardId == "CS2_101t"),
            recruit =>
            {
                Assert.Equal(3, s.Entities[recruit].Character!.Attack);
                Assert.Equal(3, s.Entities[recruit].Character!.Health);
            });

        // End turn cycle so Silver Hand Recruits lose summoning sickness, sacrifice one into Yeti (21) -> Bolvar gains +1 Atk in hand & retains it on play!
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, Second)).State;
        var sacrificed = s.Players[First].In(GameZone.Play).First(id => s.Entities[id].CardId == "CS2_101t");
        s = simulator.Apply(s, new(SimulationActionKind.Attack, First, sacrificed, new(21))).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(42), Position: 0)).State;
        Assert.Equal(2, s.Entities[new(42)].Character!.Attack);
    }

    [Fact]
    public void GvgGallywixMalorneGahzrillaCoghammerAndBurrowingMineWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones(
                    (GameZone.Play, [10, 11, 12]),
                    (GameZone.Hand, [30, 31, 32, 33, 34, 35, 36]),
                    (GameZone.Deck, [50, 51, 52, 53]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 10, ManaCrystals = 10, MaximumManaCrystals = 10,
                Zones = Zones(
                    (GameZone.Play, [20, 21]),
                    (GameZone.Hand, [60]),
                    (GameZone.Deck, [61]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "GVG_028", First, 3, 5, 8), // Trade Prince Gallywix
            InPlay(12, "GVG_035", First, 4, 9, 7), // Malorne (Beast)
            InPlay(21, "GVG_049", Second, 5, 6, 9), // Gahz'rilla (Beast)
            Card(30, "GVG_046", First), // King of Beasts (other friendly Beast -> Malorne makes +1 Atk = 2+1=3 or 3 Atk)
            Card(31, "GVG_059", First), // Coghammer
            Card(32, "GVG_056", First), // Iron Juggernaut
            Card(33, "GVG_041", First), // Dark Wispers
            Card(34, "GVG_045", First), // Imp-losion
            Card(35, "PART_007", First), // Whirling Blades (+1 Atk)
            Card(36, "GVG_050", First), // Bouncing Blade
            Card(50, "CS2_231", First),
            Card(51, "CS2_231", First),
            Card(52, "CS2_231", First),
            Card(53, "CS2_231", First),
            Card(60, "PART_001", Second), // Armor Plating spell for Gallywix test
            Card(61, "CS2_231", Second)
        ], First);

        // King of Beasts (30): Malorne (12) is another friendly Beast -> gains +1 Attack
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(30), Position: 0)).State;
        Assert.Equal(simulator.Cards["GVG_046"].Definition.PrintedAttack + 1, simulator.EffectiveAttack(s, s.Entities[new(30)]));

        // Coghammer (31): grants BOTH Divine Shield and Taunt to one random friendly minion!
        var candidatesBefore = s.Players[First].In(GameZone.Play).Where(id => id != FirstHero).ToArray();
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(31))).State;
        Assert.Single(candidatesBefore, id =>
            s.Entities[id].Character!.DivineShield && s.Entities[id].Enchantments.Any(e => e.GrantedTag == GAME_TAG.TAUNT));

        // Gahz'rilla (21): Imp-losion deals 2-4 to Gahz'rilla (double attack!) and summons Imps!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(34), new(21))).State;
        Assert.Equal(12, s.Entities[new(21)].Character!.Attack);
        Assert.Contains(s.Players[First].In(GameZone.Play), id => s.Entities[id].CardId == "GVG_045t");

        // Dark Wispers (33) Choose One: buff Gahz'rilla (+5/+5 & Taunt)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(33), new(21)) { Choice = "GVG_041a" }).State;
        Assert.Contains(s.Entities[new(21)].Enchantments, e => e.GrantedTag == GAME_TAG.TAUNT);
        Assert.Equal(17, s.Entities[new(21)].Character!.Attack);

        // Opponent casts Armor Plating (60) -> Gallywix (11) gives First a copy of PART_001 and Second gets GVG_028t!
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, Second, new(60), new(21))).State;
        Assert.Contains(s.Players[First].In(GameZone.Hand), id => s.Entities[id].CardId == "PART_001");
        var coinId = Assert.Single(s.Players[Second].In(GameZone.Hand), id => s.Entities[id].CardId == "GVG_028t");
        var firstHandCount = s.Players[First].In(GameZone.Hand).Length;
        // Second casts Gallywix's Coin (GVG_028t) -> MUST NOT trigger Gallywix!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, Second, coinId)).State;
        Assert.Equal(firstHandCount, s.Players[First].In(GameZone.Hand).Length);
    }

    [Fact]
    public void GvgMalorneAwakenFelReaverWrenchFeignDeathLightbombRecycleAndOgreWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones(
                    (GameZone.Play, [10, 11, 12, 13, 14, 15, 16]),
                    (GameZone.Hand, [30, 31, 32, 33, 34, 35, 36]),
                    (GameZone.Deck, [50, 51, 52, 53]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 10, ManaCrystals = 10, MaximumManaCrystals = 10,
                Zones = Zones(
                    (GameZone.Play, [20, 21, 22]),
                    (GameZone.Hand, [60]),
                    (GameZone.Deck, [61, 62]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "GVG_035", First, 3, 9, 7) with { Character = new(9, 7, Damage: 6) }, // Malorne at 1 HP
            InPlay(12, "GVG_046", First, 4, 2, 1), // Beast #1 at 1 HP
            InPlay(13, "GVG_049", First, 5, 6, 1), // Beast #2 at 1 HP
            InPlay(14, "GVG_065", First, 6, 4, 4), // Ogre Brute (50% wrong enemy)
            InPlay(15, "GVG_016", First, 7, 8, 8), // Fel Reaver
            InPlay(16, "GVG_056", First, 8, 6, 5), // Iron Juggernaut (Deathrattle: Mine)
            InPlay(21, "CS2_231", Second, 9, 1, 1), // Enemy Wisp 1/1
            InPlay(22, "CS2_182", Second, 10, 4, 5), // Enemy Yeti 4/5 (for Recycle)
            Card(30, "GVG_038", First), // Crackle
            Card(31, "GVG_024", First), // Cogmaster's Wrench (1/3 +2 Atk with Mech 16)
            Card(32, "GVG_026", First), // Feign Death (triggers Iron Juggernaut DR -> Mine in Second deck)
            Card(33, "GVG_031", First), // Recycle (shuffles Yeti 22 into Second deck)
            Card(34, "GVG_008", First), // Lightbomb (kills Malorne 9/1 & Beast2 6/1 & Fel Reaver 8/8 & Iron Juggernaut 6/5!)
            Card(35, "GVG_038", First), // Extra Crackle
            Card(36, "GVG_050", First), // Bouncing Blade
            Card(50, "CS2_231", First),
            Card(51, "CS2_231", First),
            Card(52, "CS2_231", First),
            Card(53, "CS2_231", First),
            Card(60, "CS2_231", Second),
            Card(61, "GVG_056t", Second), // Burrowing Mine on top of Second deck
            Card(62, "CS2_231", Second) // Extra card so Burrowing Mine's cycle draw doesn't fatigue
        ], First);

        // Ogre Brute (14) attacking SecondHero (20) has outcomes hitting SecondHero or Wisp (21) or Yeti (22)
        var ogreOutcomes = simulator.ApplyDistribution(root, new(SimulationActionKind.Attack, First, new(14), SecondHero));
        Assert.True(ogreOutcomes.Outcomes.Length >= 2);
        Assert.Contains(ogreOutcomes.Outcomes, o => o.Result.State.Entities[SecondHero].Character!.Health == 26);
        Assert.Contains(ogreOutcomes.Outcomes, o => o.Result.State.LocationOf(new(21)).Zone == GameZone.Graveyard);

        // Cogmaster's Wrench (31): friendly Mech Iron Juggernaut (16) gives +2 weapon Attack -> 3 Atk
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(31))).State;
        Assert.Equal(3, simulator.EffectiveAttack(s, s.Entities[FirstHero]));

        // Recycle (33): shuffles enemy Yeti (22) into opponent's deck (2 -> 3)!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(33), new(22))).State;
        Assert.Equal(GameZone.Deck, s.LocationOf(new(22)).Zone);
        Assert.Equal(3, s.Players[Second].In(GameZone.Deck).Length);

        // Feign Death (32) from s (branching separately so Malorne's DR doesn't double-schedule): opponent deck grows 3 -> 4!
        var sFeign = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(32))).State;
        Assert.Equal(4, sFeign.Players[Second].In(GameZone.Deck).Length);

        // Kill Malorne (11) with Crackle (30) -> goes Dormant with Remaining = 2 friendly Beast deaths
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(30), new(11))).State;
        Assert.Single(s.Players[First].PendingAwakens);
        Assert.Equal(2, s.Players[First].PendingAwakens[0].Remaining);

        // Lightbomb (34): deals Attack as damage -> Beast #1 (2 atk -> dies), Beast #2 (6 atk -> dies)!
        // Both friendly Beasts die (2 deaths >= 2 Remaining) -> Malorne awakens onto board!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(34))).State;
        Assert.Empty(s.Players[First].PendingAwakens);
        Assert.Contains(s.Players[First].In(GameZone.Play), id => s.Entities[id].CardId == "GVG_035");

        // Bouncing Blade (36) kills a minion when played from root
        var sBlade = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(36))).State;
        var totalBefore = root.Players[First].In(GameZone.Play).Length + root.Players[Second].In(GameZone.Play).Length;
        var totalAfter = sBlade.Players[First].In(GameZone.Play).Length + sBlade.Players[Second].In(GameZone.Play).Length;
        Assert.True(totalAfter < totalBefore);

        // Second draws Burrowing Mine (61 -> 10 dmg to SecondHero!) and plays Wisp (60) -> Fel Reaver (15) mills 3 cards from First deck!
        var sMill = simulator.Apply(root, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(20, sMill.Entities[SecondHero].Character!.Health);
        var firstDeckBefore = sMill.Players[First].In(GameZone.Deck).Length;
        sMill = simulator.Apply(sMill, new(SimulationActionKind.PlayCard, Second, new(60), Position: 0)).State;
        Assert.Equal(firstDeckBefore - 3, sMill.Players[First].In(GameZone.Deck).Length);
    }
}
