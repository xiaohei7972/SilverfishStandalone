using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void TaskmasterGrommashBattleRageInnerFireAndOwlResolveCorrectly()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12, 13, 14, 15]), (GameZone.Deck, [16, 17, 18]))
            },
            Player(Second, SecondHero)
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30) with { Character = new(0, 30, Damage: 3) },
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "EX1_414", First, 3, 4, 9),
            Card(12, "EX1_603", First), Card(13, "EX1_392", First),
            Card(14, "CS1_129", First), Card(15, "CS2_203", First),
            Card(16, "CS2_231", First), Card(17, "CS2_231", First), Card(18, "CS2_231", First)
        ], First);

        // Cruel Taskmaster hits Grommash (4/9 -> 12/8: +2 from Taskmaster, +6 while damaged)
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(12), new(11), Position: 1)).State;
        Assert.Equal(12, s.Entities[new(11)].Character!.Attack);
        Assert.Equal(8, s.Entities[new(11)].Character!.Health);

        // Battle Rage draws 2 cards (damaged hero + damaged Grommash)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(13))).State;
        Assert.Single(s.Players[First].In(GameZone.Deck));

        // Inner Fire sets Taskmaster (2/2) or Grommash (12/8) attack to current health (8 + 6 aura = 14!)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14), new(11))).State;
        Assert.Equal(14, s.Entities[new(11)].Character!.Attack);

        // Ironbeak Owl silences Grommash -> back to 4/8!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15), new(11), Position: 2)).State;
        Assert.Equal(4, s.Entities[new(11)].Character!.Attack);
    }

    [Fact]
    public void VaporizeSpellbenderAndGladiatorsLongbowHandleCombatAndSpellInterception()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 20, ManaCrystals = 20, MaximumManaCrystals = 20,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12, 13]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 20, ManaCrystals = 20, MaximumManaCrystals = 20,
                Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Hand, [22, 23]), (GameZone.Deck, [24]))
            },
            InPlay(11, "CS2_231", First, 3, 2, 2),
            Card(12, "EX1_594", First), Card(13, "tt_010", First),
            InPlay(21, "EX1_560", Second, 4, 8, 8),
            Card(22, "DS1_188", Second), Card(23, "CS1_129", Second), Card(24, "CS2_231", Second));

        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(12))).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(13))).State;
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;

        // Second equips Gladiator's Longbow and attacks First's minion (2/2): Hero does NOT trigger Vaporize and takes 0 retaliation!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, Second, new(22))).State;
        s = simulator.Apply(s, new(SimulationActionKind.Attack, Second, SecondHero, new(11))).State;
        Assert.Equal(30, s.Entities[SecondHero].Character!.Health);
        Assert.Single(s.Players[First].In(GameZone.Secret), new EntityId(12));

        // Second casts Inner Fire on Nozdormu (8/8) -> Spellbender redirects it to 1/3 Spellbender (becomes 3/3)!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, Second, new(23), new(21))).State;
        var bender = Assert.Single(s.Players[First].In(GameZone.Play), id => s.Entities[id].CardId == "tt_010a");
        Assert.Equal(3, s.Entities[bender].Character!.Attack);

        // Nozdormu attacks FirstHero -> Vaporize destroys Nozdormu and FirstHero takes 0 damage!
        s = simulator.Apply(s, new(SimulationActionKind.Attack, Second, new(21), FirstHero)).State;
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(21)).Zone);
        Assert.Equal(30, s.Entities[FirstHero].Character!.Health);
    }

    [Fact]
    public void EdwinKidnapperPerditionsBladeManaWyrmMalygosSplashAndHolyWrathWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13, 14, 15, 16, 17, 18]), (GameZone.Deck, [19]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21, 22, 23]))
            },
            InPlay(11, "NEW1_012", First, 3, 1, 3),
            InPlay(12, "EX1_563", First, 4, 4, 12),
            Card(13, "EX1_275", First), Card(14, "EX1_537", First),
            Card(15, "EX1_365", First), Card(16, "EX1_133", First),
            Card(17, "NEW1_005", First), Card(18, "EX1_613", First),
            Card(19, "EX1_562", First),
            InPlay(21, "CS2_231", Second, 5, 1, 10),
            InPlay(22, "CS2_231", Second, 6, 1, 20),
            InPlay(23, "CS2_231", Second, 7, 1, 20));

        // Cone of Cold with Malygos (+5): freezes 21, 22, 23 and deals 6 damage to each; Mana Wyrm gains +1 Attack (2)
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(13), new(22))).State;
        Assert.All(new EntityId[] { new(21), new(22), new(23) }, id => Assert.True(s.Entities[id].Character!.Frozen));
        Assert.Equal(4, s.Entities[new(21)].Character!.Health);
        Assert.Equal(14, s.Entities[new(22)].Character!.Health);
        Assert.Equal(2, s.Entities[new(11)].Character!.Attack);

        // Explosive Shot with Malygos (+5): deals 10 to center (22: 14->4) and 7 to neighbors (21 dies, 23: 14->7)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14), new(22))).State;
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(21)).Zone);
        Assert.Equal(4, s.Entities[new(22)].Character!.Health);
        Assert.Equal(7, s.Entities[new(23)].Character!.Health);

        // Holy Wrath draws Onyxia (cost 9) + Malygos (+5) = 14 damage to 23 (kills it!); hero target rejected!
        Assert.Equal(ErrorType.REQ_MINION_TARGET,
            simulator.CheckAction(s, new(SimulationActionKind.PlayCard, First, new(15), SecondHero))?.Type);
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15), new(23))).State;
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(23)).Zone);

        // Perdition's Blade (Combo): deals 2 damage to SecondHero (30 -> 28)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16), SecondHero)).State;
        Assert.Equal(28, s.Entities[SecondHero].Character!.Health);

        // Kidnapper (Combo): bounces 22 to Second's hand
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(17), new(22), Position: 2)).State;
        Assert.Equal(GameZone.Hand, s.LocationOf(new(22)).Zone);

        // Edwin VanCleef after 5 cards played -> 2/2 + 10/10 = 12/12!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(18), Position: 3)).State;
        Assert.Equal(12, s.Entities[new(18)].Character!.Attack);
        Assert.Equal(12, s.Entities[new(18)].Character!.Health);
    }

    [Fact]
    public void OnyxiaRagnarosGeddonDeathrattlesMurlocsAndEndOfTurnEffectsWork()
    {
        var murloc = new SimulationCard(new("test-murloc", EntityKind.Minion, 1, 1, 1) { MinionTypes = MinionType.Murloc });
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, murloc]);

        // Onyxia fills the board alternating right and left + Soul of the Forest + Ancestral Spirit + Cairne + Ghoul
        var onyxiaRoot = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12, 13, 14]))
            },
            Player(Second, SecondHero),
            InPlay(11, "tt_004", First, 3, 2, 3),
            Card(12, "EX1_110", First), Card(13, "CS2_038", First), Card(14, "EX1_158", First));
        var s = simulator.Apply(onyxiaRoot, new(SimulationActionKind.PlayCard, First, new(12), Position: 1)).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(13), new(12))).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14))).State;

        // Kill Cairne with Abomination deathrattle in another setup, or verify Cairne's layered deathrattles
        var killRoot = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13, 14, 15, 16]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "tt_004", First, 3, 2, 6),
            InPlay(12, "EX1_110", First, 4, 5, 1),
            Card(13, "CS2_038", First), Card(14, "EX1_158", First),
            Card(15, "EX1_097", First), Card(16, "EX1_249", First),
            InPlay(21, "CS2_231", Second, 5, 5, 4));
        var k = simulator.Apply(killRoot, new(SimulationActionKind.PlayCard, First, new(13), new(12))).State;
        k = simulator.Apply(k, new(SimulationActionKind.PlayCard, First, new(14))).State;
        k = simulator.Apply(k, new(SimulationActionKind.PlayCard, First, new(15), Position: 2)).State;
        k = simulator.Apply(k, new(SimulationActionKind.PlayCard, First, new(16), Position: 3)).State;
        // End turn -> Geddon deals 2 to all other characters -> Cairne (1 hp) & Abomination (4->2, wait 4 hp)
        k = simulator.Apply(k, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Contains(k.Players[First].In(GameZone.Play), id => k.Entities[id].CardId == "EX1_110t");
        Assert.Contains(k.Players[First].In(GameZone.Play), id => k.Entities[id].CardId == "EX1_110");
        Assert.Contains(k.Players[First].In(GameZone.Play), id => k.Entities[id].CardId == "EX1_158t");
        Assert.Equal(3, k.Entities[new(11)].Character!.Attack);

        // Ragnaros (cannot attack, deals 8 at end of turn), Swordsmith, Dwarf, Priestess, Onyxia, Hungry Crab, Coldlight Seer
        var batchRoot = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13, 14, 15, 16, 17, 18]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30) with { Character = new(0, 30, Damage: 6) },
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, murloc.Definition.CardId, First, 3, 1, 1),
            InPlay(12, "NEW1_037", First, 4, 1, 3),
            Card(13, "EX1_103", First), Card(14, "EX1_046", First),
            Card(15, "EX1_583", First), Card(16, "NEW1_017", First),
            Card(17, "EX1_298", First), Card(18, "EX1_562", First),
            InPlay(21, murloc.Definition.CardId, Second, 5, 1, 1)
        ], First);

        var b = simulator.Apply(batchRoot, new(SimulationActionKind.PlayCard, First, new(13), Position: 2)).State;
        Assert.Equal(3, b.Entities[new(11)].Character!.Health);
        b = simulator.Apply(b, new(SimulationActionKind.PlayCard, First, new(14), new(11), Position: 3)).State;
        Assert.Equal(3, b.Entities[new(11)].Character!.Attack);
        b = simulator.Apply(b, new(SimulationActionKind.PlayCard, First, new(15), Position: 4)).State;
        Assert.Equal(28, b.Entities[FirstHero].Character!.Health);
        b = simulator.Apply(b, new(SimulationActionKind.PlayCard, First, new(16), new(21), Position: 5)).State;
        Assert.Equal(GameZone.Graveyard, b.LocationOf(new(21)).Zone);
        Assert.Equal(3, b.Entities[new(16)].Character!.Attack);
        Assert.Equal(4, b.Entities[new(16)].Character!.Health);
        b = simulator.Apply(b, new(SimulationActionKind.PlayCard, First, new(17), Position: 6)).State;
        Assert.DoesNotContain(simulator.LegalActions(b), a => a.Kind == SimulationActionKind.Attack && a.Source == new EntityId(17));
        var afterRag = simulator.Apply(b, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(21, afterRag.Entities[SecondHero].Character!.Health);

        var onlyOnyxia = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 9, ManaCrystals = 9, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero), Card(11, "EX1_562", First));
        var filled = simulator.Apply(onlyOnyxia, new(SimulationActionKind.PlayCard, First, new(11), Position: 0)).State;
        var slots = filled.Players[First].In(GameZone.Play).Where(id => id != FirstHero).ToArray();
        Assert.Equal(7, slots.Length);
        Assert.Equal(new EntityId(11), slots[3]);
        Assert.Equal(6, slots.Count(id => filled.Entities[id].CardId == "EX1_116t"));
    }
}
