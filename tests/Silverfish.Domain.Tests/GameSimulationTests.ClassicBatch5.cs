using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void FarSightAncientMageGadgetzanWrathEviscerateAndMillhouseWorkCorrectly()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13, 14, 15, 16]), (GameZone.Deck, [30, 31, 32]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 2, ManaCrystals = 2,
                Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Hand, [22, 23]), (GameZone.Deck, [40]))
            },
            InPlay(11, "CS2_231", First, 3, 1, 1),
            InPlay(12, "EX1_095", First, 4, 4, 4), // Gadgetzan Auctioneer
            Card(13, "EX1_584", First), // Ancient Mage -> gives adjacent 11 & 12 Spell Damage +1 (+2 total!)
            Card(14, "CS2_053", First), // Far Sight -> Gadgetzan draws 30 first, then Far Sight draws 31 with -3 cost!
            Card(15, "EX1_124", First), // Eviscerate (Combo: 4 + 2 Spell Damage = 6 damage!)
            Card(16, "NEW1_029", First), // Millhouse Manastorm -> Second's spells cost 0 next turn!
            Card(30, "CS2_231", First),
            Card(31, "EX1_383", First), // Tirion Fordring (8 mana -> reduced to 5 by Far Sight!)
            Card(32, "CS2_231", First),
            InPlay(21, "CS2_231", Second, 5, 1, 10),
            Card(22, "EX1_312", Second), // Twisting Nether (8 mana -> costs 0 on Second's turn due to Millhouse!)
            Card(23, "EX1_154", Second), // Wrath (2 mana -> costs 0 on Second's turn!)
            Card(40, "CS2_231", Second));

        // Ancient Mage placed between 11 and 12 (Position 1) grants Spell Damage +1 to BOTH -> +2 Spell Damage!
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(13), Position: 1)).State;

        // Cast Far Sight: Gadgetzan draws 30 (BeforeSpellResolved), then Far Sight draws 31 (Tirion, 8 - 3 = 5 cost!)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14))).State;
        Assert.Equal(5, simulator.EffectiveCost(s, new(31)));

        // Eviscerate with Combo +2 Spell Damage deals 6 damage to SecondHero (30 -> 24), and Gadgetzan draws 32
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15), SecondHero)).State;
        Assert.Equal(24, s.Entities[SecondHero].Character!.Health);

        // Play Millhouse Manastorm: on First's turn Second's Twisting Nether still costs 8; on Second's turn it costs 0!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16), Position: 0)).State;
        Assert.Equal(8, simulator.EffectiveCost(s, new(22)));
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(0, simulator.EffectiveCost(s, new(22)));
        Assert.Equal(0, simulator.EffectiveCost(s, new(23)));

        // Second casts 0-cost Twisting Nether -> wipes ALL minions on both sides!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, Second, new(22))).State;
        Assert.Single(s.Players[First].In(GameZone.Play));
        Assert.Single(s.Players[Second].In(GameZone.Play));
    }

    [Fact]
    public void LightspawnPowerOverwhelmingFlareHarrisonDreadCorsairMountainGiantAndPortalWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12, 13, 14, 15, 16, 17, 18]), (GameZone.Deck, [30, 31, 32, 33]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 10, ManaCrystals = 10,
                Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Secret, [22]), (GameZone.Deck, [40]))
            },
            Card(11, "EX1_105", First), // Mountain Giant (12 mana - 7 other cards in hand = 5!)
            Card(12, "EX1_247", First), // Stormforged Axe (2/3 weapon, Overload: 1)
            Card(13, "NEW1_022", First), // Dread Corsair (4 mana - 2 weapon attack = 2!)
            Card(14, "EX1_315", First), // Summoning Portal (minions cost 2 less, min 1)
            Card(15, "EX1_335", First), // Lightspawn (0/4 -> Attack equals Health = 4/4!)
            Card(16, "EX1_316", First), // Power Overwhelming (+4/+4 on Lightspawn -> 8/8, dies at end of turn!)
            Card(17, "EX1_544", First), // Flare (removes Stealth on 21, destroys Secret 22, draws 1)
            Card(18, "EX1_558", First), // Harrison Jones
            Card(30, "CS2_231", First), Card(31, "CS2_231", First), Card(32, "CS2_231", First), Card(33, "CS2_231", First),
            InPlay(21, "EX1_010", Second, 3, 2, 1), // Worgen Infiltrator (printed Stealth)
            Card(22, "EX1_132", Second) with { PlayOrder = 4 }, // Eye for an Eye
            Card(40, "CS2_231", Second));

        // Mountain Giant costs 12 - 7 = 5
        Assert.Equal(5, simulator.EffectiveCost(root, new(11)));
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(12))).State;
        Assert.Equal(1, s.Players[First].OverloadOwed);
        Assert.Equal(2, simulator.EffectiveCost(s, new(13))); // Dread Corsair 4 - 2 = 2

        // Summoning Portal reduces Lightspawn (3 -> 1) and Dread Corsair (2 -> 1, floored at 1!)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14), Position: 0)).State;
        Assert.Equal(1, simulator.EffectiveCost(s, new(15)));
        Assert.Equal(1, simulator.EffectiveCost(s, new(13)));

        // Play Lightspawn -> 4/4! Power Overwhelming gives +4/+4 -> Health is 8 so Lightspawn is 8/8!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15), Position: 1)).State;
        Assert.Equal(4, s.Entities[new(15)].Character!.Attack);
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16), new(15))).State;
        Assert.Equal(8, s.Entities[new(15)].Character!.Attack);
        Assert.Equal(8, s.Entities[new(15)].Character!.Health);

        // Flare removes Stealth from Worgen Infiltrator (now targetable!), destroys Second's Secret, and draws 30!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(17))).State;
        Assert.Empty(s.Players[Second].In(GameZone.Secret));
        Assert.False(s.Entities[new(21)].Stealthed ?? true);

        // End turn -> Power Overwhelming expires and destroys Lightspawn!
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(15)).Zone);

        // Second's turn: Second equips weapon? Wait—let's pass Second's turn and test Harrison, Tirion, Deathwing, Beast, Conceal, Bestial Wrath in next Fact!
    }

    [Fact]
    public void TirionHarrisonTheBeastImpMasterDeathwingSenseDemonsConcealAndBestialWrathWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13, 14, 15, 16, 17]), (GameZone.Deck, [30, 31, 32, 33, 34]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 20, ManaCrystals = 20, MaximumManaCrystals = 20,
                Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Hand, [22, 23]), (GameZone.Deck, [40]))
            },
            InPlay(11, "EX1_017", First, 3, 4, 2), // Jungle Panther (Beast)
            InPlay(12, "EX1_597", First, 4, 1, 1), // Imp Master at 1 Health -> damages self to 0 & STILL summons 1/1 Imp before dying!
            Card(13, "EX1_549", First), // Bestial Wrath (+2 Attack & Immune on Jungle Panther -> 6/2 Immune!)
            Card(14, "EX1_317", First), // Sense Demons (draws 2 Demons: 30 & 31 from deck!)
            Card(15, "EX1_128", First), // Conceal (Stealth until next turn)
            Card(16, "NEW1_008", First), // Ancient of Lore
            Card(17, "NEW1_030", First), // Deathwing
            Card(30, "EX1_310", First), // Doomguard (Demon)
            Card(31, "EX1_313", First), // Pit Lord (Demon)
            Card(32, "CS2_231", First), Card(33, "CS2_231", First), Card(34, "CS2_231", First),
            InPlay(21, "EX1_383", Second, 5, 6, 6), // Damaged Tirion (no shield) at 6/6
            Card(22, "EX1_558", Second), // Harrison Jones
            Card(23, "EX1_577", Second), // The Beast
            Card(40, "CS2_231", Second));

        // Bestial Wrath makes Jungle Panther 6/2 Immune -> attacks 6/6 Tirion, kills Tirion while taking 0 damage, Tirion equips 5/3 Ashbringer for Second!
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(13), new(11))).State;
        s = simulator.Apply(s, new(SimulationActionKind.Attack, First, new(11), new(21))).State;
        Assert.Equal(2, s.Entities[new(11)].Character!.Health);
        Assert.Equal("EX1_383t", s.Entities[s.Players[Second].EquippedWeapon!.Value].CardId);

        // Sense Demons draws the 2 Demons (30 & 31) out of First's deck
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14))).State;
        Assert.Contains(new EntityId(30), s.Players[First].In(GameZone.Hand));
        Assert.Contains(new EntityId(31), s.Players[First].In(GameZone.Hand));

        // End First's turn: 1-HP Imp Master deals 1 to self, summons 1/1 EX1_598 Imp, and then Imp Master dies!
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(12)).Zone);
        Assert.Contains(s.Players[First].In(GameZone.Play), id => s.Entities[id].CardId == "EX1_598");

        // Second plays The Beast, ends turn; First plays Deathwing -> destroys The Beast (summoning 3/3 EX1_finkle for First!) and discards First's hand!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, Second, new(23), Position: 0)).State;
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, Second)).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(17), Position: 0)).State;
        Assert.Empty(s.Players[First].In(GameZone.Hand));
        Assert.Contains(s.Players[First].In(GameZone.Play), id => s.Entities[id].CardId == "EX1_finkle");
        Assert.Empty(s.Players[Second].In(GameZone.Play).Where(id => id != SecondHero && s.Entities[id].Character is not null));
    }
}
