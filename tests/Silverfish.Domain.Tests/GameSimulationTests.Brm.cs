using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void BrmDamageTriggersMinionDiscountAndDragonsWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, CompiledCardData.Bind("HERO_08", new())]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones(
                    (GameZone.Play, [10, 11, 12, 13, 14]),
                    (GameZone.Hand, [15, 16, 17, 18, 19, 50, 51, 52, 53]),
                    (GameZone.Deck, [30, 31, 32]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21, 22, 23]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30) with { Character = new(0, 30, Damage: 15) }, // 15 HP for Drakonid Crusher!
            InPlay(11, "BRM_006", First, 3, 2, 4), // Imp Gang Boss (Demon)
            InPlay(12, "BRM_016", First, 4, 2, 6), // Axe Flinger
            InPlay(13, "BRM_019", First, 5, 3, 3), // Grim Patron
            InPlay(14, "BRM_022", First, 6, 0, 2), // Dragon Egg
            Card(15, "BRM_004", First), // Twilight Whelp (Dragon in hand -> 2/3)
            Card(16, "BRM_033", First), // Blackwing Technician (Dragon in hand -> 3/5)
            Card(17, "BRM_024", First), // Drakonid Crusher (Dragon! Enemy <= 15 HP -> 9/9)
            Card(18, "BRM_034", First), // Blackwing Corruptor (Dragon in hand -> deals 5 damage)
            Card(19, "BRM_029", First), // Rend Blackhand (Dragon in hand -> destroys Legendary minion)
            Card(50, "BRM_005", First), // Demonwrath (2 dmg to non-Demons)
            Card(51, "BRM_008", First), // Dark Iron Skulker (2 dmg to undamaged enemies)
            Card(52, "BRM_001", First), // Solemn Vigil (cost 5 - minions died)
            Card(53, "BRM_009", First), // Volcanic Lumberer (cost 9 - minions died)
            InPlay(21, "CS2_231", Second, 7, 1, 1), // Enemy Wisp (undamaged -> dies to Skulker)
            InPlay(22, "EX1_572", Second, 8, 4, 12) with { Character = new(4, 12, Damage: 1) }, // Damaged Legendary Ysera (skipped by Skulker, destroyed by Rend!)
            InPlay(23, "BRM_025", Second, 9, 6, 4), // Undamaged enemy Volcanic Drake
            Card(30, "CS2_231", First),
            Card(31, "CS2_231", First),
            Card(32, "CS2_231", First)
        ], First);

        // Dark Iron Skulker (51): deals 2 to undamaged enemy Wisp (21 -> dies!) & Volcanic Drake (23: 4->2), skips damaged Ysera (22: stays 11 HP)!
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(51), Position: 4)).State;
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(21)).Zone);
        Assert.Equal(11, s.Entities[new(22)].Character!.Health);
        Assert.Equal(2, s.Entities[new(23)].Character!.Health);

        // 1 minion died this turn -> Solemn Vigil (52) costs 5-1=4, Volcanic Lumberer (53) costs 9-1=8!
        Assert.Equal(4, simulator.EffectiveCost(s, new(52)));
        Assert.Equal(8, simulator.EffectiveCost(s, new(53)));

        // Cast Demonwrath (50): 2 damage to all non-Demon minions ->
        // Imp Gang Boss (11, Demon) takes 0 dmg!
        // Axe Flinger (12) takes 2 -> deals 2 to Enemy Hero (15 -> 13)!
        // Grim Patron (13, 3->1 HP) survives -> summons another Grim Patron!
        // Dragon Egg (14, 2->0 HP) takes 2 and dies -> still triggers summon of 2/1 Whelp (BRM_022t)!
        // Enemy Volcanic Drake (23, 2->0 HP) dies!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(50))).State;
        Assert.Equal(0, s.Entities[new(11)].Character!.Damage);
        Assert.Equal(13, s.Entities[SecondHero].Character!.Health);
        Assert.Equal(2, s.Players[First].In(GameZone.Play).Count(id => s.Entities[id].CardId == "BRM_019"));
        Assert.Contains(s.Players[First].In(GameZone.Play), id => s.Entities[id].CardId == "BRM_022t");
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(23)).Zone);

        // Now 3 minions died this turn (21, 14, 23) -> Solemn Vigil costs 5-3=2! Cast Solemn Vigil (52) to draw 2.
        Assert.Equal(2, simulator.EffectiveCost(s, new(52)));
        var handBefore = s.Players[First].In(GameZone.Hand).Length;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(52))).State;
        Assert.Equal(handBefore + 1, s.Players[First].In(GameZone.Hand).Length); // -1 played +2 drawn

        // Twilight Whelp (15) gains +2 Health (2/3) while holding Dragon 17
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15), Position: 0)).State;
        Assert.Equal(3, s.Entities[new(15)].Character!.Health);

        // Test Blackwing Technician (16), Blackwing Corruptor (18), Rend Blackhand (19), and Drakonid Crusher (17)
        var dragonRoot = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [16, 17, 18, 19]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 22])) }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30) with { Character = new(0, 30, Damage: 17) }, // 13 HP
            InPlay(22, "EX1_572", Second, 3, 4, 12), // Legendary Ysera
            Card(16, "BRM_033", First), // Blackwing Technician
            Card(17, "BRM_024", First), // Drakonid Crusher (Dragon in hand -> becomes 9/9)
            Card(18, "BRM_034", First), // Blackwing Corruptor
            Card(19, "BRM_029", First)  // Rend Blackhand
        ], First);

        var d = simulator.Apply(dragonRoot, new(SimulationActionKind.PlayCard, First, new(16), Position: 0)).State;
        Assert.Equal(3, d.Entities[new(16)].Character!.Attack);
        Assert.Equal(5, d.Entities[new(16)].Character!.Health);

        d = simulator.Apply(d, new(SimulationActionKind.PlayCard, First, new(18), SecondHero, Position: 0)).State;
        Assert.Equal(8, d.Entities[SecondHero].Character!.Health); // 13 - 5 = 8

        d = simulator.Apply(d, new(SimulationActionKind.PlayCard, First, new(19), new(22), Position: 0)).State;
        Assert.Equal(GameZone.Graveyard, d.LocationOf(new(22)).Zone); // Ysera destroyed!

        d = simulator.Apply(d, new(SimulationActionKind.PlayCard, First, new(17), Position: 0)).State;
        Assert.Equal(9, d.Entities[new(17)].Character!.Attack);
        Assert.Equal(9, d.Entities[new(17)].Character!.Health);
    }

    [Fact]
    public void BrmSpellsChoicesConsortThaurissanMajordomoNefarianAndChromaggusWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, CompiledCardData.Bind("HERO_08", new())]);
        var enemyMage = InPlay(20, "HERO_08", Second, 2, 0, 30);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones(
                    (GameZone.Play, [10, 11, 12]),
                    (GameZone.Hand, [13, 14, 15, 16, 17, 18, 50, 51]),
                    (GameZone.Deck, [30, 31]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 10, ManaCrystals = 10, MaximumManaCrystals = 10,
                Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Deck, [40]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30) with { Character = new(0, 30, Damage: 20) }, // 10 HP (<=12 for Revenge!)
            enemyMage,
            InPlay(11, "BRM_002", First, 3, 2, 4), // Flamewaker
            InPlay(12, "BRM_020", First, 4, 3, 5), // Dragonkin Sorcerer
            Card(13, "CS2_092", First), // Blessing of Kings (targets Dragonkin Sorcerer -> +1/+1 +4/+4 = 8/10!)
            Card(14, "BRM_010", First), // Druid of the Flame
            Card(15, "BRM_012", First), // Fireguard Destroyer
            Card(16, "BRM_018", First), // Dragon Consort
            Card(17, "BRM_025", First), // Volcanic Drake (6-cost Dragon -> becomes 4 with Consort!)
            Card(18, "BRM_015", First), // Revenge (3 damage to all minions at 10 HP)
            Card(50, "BRM_007", First), // Gang Up
            Card(51, "BRM_028", First), // Emperor Thaurissan
            InPlay(21, "CS2_182", Second, 5, 4, 5),
            Card(30, "CS2_231", First),
            Card(31, "CS2_231", First),
            Card(40, "CS2_231", Second)
        ], First);

        // Cast Blessing of Kings (13) on Dragonkin Sorcerer (12) -> TargetedBySpell (+1/+1) + BoK (+4/+4) = 8/10, and Flamewaker fires 2 pings!
        var enemyHpBefore = root.Entities[SecondHero].Character!.Health + root.Entities[new(21)].Character!.Health;
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(13), new(12))).State;
        Assert.Equal(8, s.Entities[new(12)].Character!.Attack);
        Assert.Equal(10, s.Entities[new(12)].Character!.Health);
        var enemyHpAfter = s.Entities[SecondHero].Character!.Health +
            (s.LocationOf(new(21)).Zone == GameZone.Play ? s.Entities[new(21)].Character!.Health : 0);
        Assert.Equal(enemyHpBefore - 2, enemyHpAfter);

        // Play Druid of the Flame (14) choosing 5/2 form (BRM_010a -> BRM_010t)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14), Position: 0) { Choice = "BRM_010a" }).State;
        Assert.Equal("BRM_010t", s.Entities[new(14)].CardId);
        Assert.Equal(5, s.Entities[new(14)].Character!.Attack);
        Assert.Equal(2, s.Entities[new(14)].Character!.Health);

        // Play Fireguard Destroyer (15) -> Attack between 4 and 7, Overload 1 owed
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15), Position: 0)).State;
        Assert.InRange(s.Entities[new(15)].Character!.Attack, 4, 7);
        Assert.Equal(1, s.Players[First].OverloadOwed);

        // Play Dragon Consort (16) -> Volcanic Drake (17) costs 6 - 2 = 4!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16), Position: 0)).State;
        Assert.Equal(4, simulator.EffectiveCost(s, new(17)));

        // Play Gang Up (50) on Dragonkin Sorcerer (12) -> shuffles 3 copies into deck
        var deckBefore = s.Players[First].In(GameZone.Deck).Length;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(50), new(12))).State;
        Assert.Equal(deckBefore + 3, s.Players[First].In(GameZone.Deck).Length);
        Assert.Equal(3, s.Players[First].In(GameZone.Deck).Count(id => s.Entities[id].CardId == "BRM_020"));

        // Play Emperor Thaurissan (51) and end turn -> Volcanic Drake (17) still has Consort's -2 AND Thaurissan's -1 (6 - 2 - 1 = 3)!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(51), Position: 0)).State;
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, Second)).State;
        Assert.Equal(3, simulator.EffectiveCost(s, new(17)));

        // Cast Revenge (18) at 10 HP -> deals 3 damage to all minions (kills 2-HP Druid of the Flame 14)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(18))).State;
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(14)).Zone);

        // Test Quick Shot (BRM_013) + Core Rager (BRM_014) empty hand, Hungry Dragon (BRM_026),
        // Majordomo (BRM_027), Nefarian (BRM_030), Chromaggus (BRM_031)
        var extra = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones(
                    (GameZone.Play, [10, 11, 12]),
                    (GameZone.Hand, [13]),
                    (GameZone.Deck, [30, 31, 32]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "HERO_08", Second, 2, 0, 30),
            InPlay(11, "BRM_027", First, 3, 9, 7), // Majordomo Executus
            InPlay(12, "BRM_031", First, 4, 6, 8), // Chromaggus
            Card(13, "BRM_013", First), // Quick Shot (last card in hand -> draws 30 AND Chromaggus copies 30!)
            InPlay(21, "CS2_231", Second, 5, 10, 10),
            Card(30, "BRM_014", First), // Core Rager (drawn & copied -> 2 in hand)
            Card(31, "BRM_026", First), // Hungry Dragon
            Card(32, "BRM_030", First)  // Nefarian
        ], First);

        // Quick Shot with empty hand -> draws BRM_014 (30), Chromaggus adds a copy of BRM_014!
        var e = simulator.Apply(extra, new(SimulationActionKind.PlayCard, First, new(13), new(21))).State;
        Assert.Equal(2, e.Players[First].In(GameZone.Hand).Count(id => e.Entities[id].CardId == "BRM_014"));

        // Trade Majordomo (11) into 21 -> Hero becomes BRM_027h (Ragnaros, 8 HP), Hero Power becomes BRM_027p!
        e = simulator.Apply(e, new(SimulationActionKind.Attack, First, new(11), new(21))).State;
        Assert.Equal("BRM_027h", e.Entities[e.Players[First].Hero].CardId);
        Assert.Equal(8, e.Entities[e.Players[First].Hero].Character!.Health);
        var ragPower = Assert.Single(e.Players[First].In(GameZone.HeroPower));
        Assert.Equal("BRM_027p", e.Entities[ragPower].CardId);
        e = simulator.Apply(e, new(SimulationActionKind.UseHeroPower, First, ragPower)).State;
        Assert.Equal(22, e.Entities[SecondHero].Character!.Health); // 30 - 8 = 22 (since 21 died from 9 attack)

        // End turns to test Nefarian + Hungry Dragon + Core Rager (non-empty hand -> 4/4, vs empty -> 7/7)
        var coreTest = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12, 13]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20])) }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "HERO_08", Second, 2, 0, 30),
            Card(11, "BRM_026", First), // Hungry Dragon -> opponent gets 1-cost minion
            Card(12, "BRM_030", First), // Nefarian -> adds 2 Mage spells
            Card(13, "BRM_014", First)  // Core Rager
        ], First);
        var c = simulator.Apply(coreTest, new(SimulationActionKind.PlayCard, First, new(11), Position: 0)).State;
        var enemySummon = Assert.Single(c.Players[Second].In(GameZone.Play), id => id != SecondHero);
        Assert.Equal(1, CompiledCardData.Bind(c.Entities[enemySummon].CardId, new()).Definition.PrintedCost);

        // Play Nefarian (12) -> adds 2 Mage spells (so hand has 3 cards: 13 + 2 Mage spells)
        c = simulator.Apply(c, new(SimulationActionKind.PlayCard, First, new(12), Position: 0)).State;
        Assert.Equal(3, c.Players[First].In(GameZone.Hand).Length);

        // Play Core Rager with non-empty hand -> stays 4/4; play Core Rager with empty hand -> 7/7
        c = simulator.Apply(c, new(SimulationActionKind.PlayCard, First, new(13), Position: 0)).State;
        Assert.Equal(4, c.Entities[new(13)].Character!.Attack);

        var emptyCore = simulator.CreateGame(
        [
            Player(First, FirstHero) with { Mana = 10, ManaCrystals = 10, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20])) }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            Card(11, "BRM_014", First)
        ], First);
        var ec = simulator.Apply(emptyCore, new(SimulationActionKind.PlayCard, First, new(11), Position: 0)).State;
        Assert.Equal(7, ec.Entities[new(11)].Character!.Attack);
        Assert.Equal(7, ec.Entities[new(11)].Character!.Health);
    }
}
