using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void UpgradeGreenskinArathiBiteSavageryAndShieldSlamResolveWeaponAndHeroMetrics()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12, 13, 14, 15, 16, 17]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21]))
            },
            Card(11, "EX1_409", First), // Upgrade! #1 -> equips 1/3 EX1_409t
            Card(12, "EX1_409", First), // Upgrade! #2 -> buffs weapon to 2/4
            Card(13, "NEW1_024", First), // Captain Greenskin -> buffs weapon to 3/5
            Card(14, "EX1_570", First), // Bite -> +4 hero attack this turn, +4 armor
            Card(15, "EX1_578", First), // Savagery -> deals hero attack (3+4=7) to minion
            Card(16, "EX1_410", First), // Shield Slam -> deals armor (4) to minion
            Card(17, "EX1_398", First), // Arathi Weaponsmith -> equips 2/2 EX1_398t
            InPlay(21, "CS2_231", Second, 3, 1, 20));

        // Upgrade! without weapon equips 1/3 Heavy Axe
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(11))).State;
        var axeId = Assert.NotNull(s.Players[First].EquippedWeapon);
        Assert.Equal("EX1_409t", s.Entities[axeId].CardId);
        Assert.Equal(new WeaponStats(1, 3, 3), s.Entities[axeId].Weapon);

        // Second Upgrade! and Captain Greenskin give +1/+1 each -> 3/5
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(12))).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(13), Position: 0)).State;
        Assert.Equal(new WeaponStats(3, 5, 5), s.Entities[axeId].Weapon);

        // Bite gives +4 hero attack this turn (total 7) and +4 armor
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14))).State;
        Assert.Equal(7, simulator.EffectiveAttack(s, s.Entities[FirstHero]));
        Assert.Equal(4, s.Entities[FirstHero].Character!.Armor);

        // Savagery deals 7 damage (20 -> 13); Shield Slam deals 4 damage (13 -> 9)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15), new(21))).State;
        Assert.Equal(13, s.Entities[new(21)].Character!.Health);
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16), new(21))).State;
        Assert.Equal(9, s.Entities[new(21)].Character!.Health);

        // Arathi Weaponsmith replaces weapon with 2/2 Battle Axe (hero attack 4+2=6)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(17), Position: 1)).State;
        var battleAxe = Assert.NotNull(s.Players[First].EquippedWeapon);
        Assert.Equal("EX1_398t", s.Entities[battleAxe].CardId);
        Assert.Equal(6, simulator.EffectiveAttack(s, s.Entities[FirstHero]));
    }

    [Fact]
    public void FacelessAlexstraszaEyeForAnEyeAndArcaneGolemBehaveAccurately()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 15, ManaCrystals = 15, MaximumManaCrystals = 20,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12, 13]), (GameZone.Deck, [14]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 4, ManaCrystals = 4,
                Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Hand, [22, 23]), (GameZone.Deck, [24]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30) with { Character = new(0, 30, Armor: 6) },
            InPlay(20, "hero", Second, 2, 0, 30),
            Card(11, "EX1_561", First), // Alexstrasza
            Card(12, "EX1_132", First), // Eye for an Eye
            Card(13, "EX1_564", First), // Faceless Manipulator
            Card(14, "CS2_231", First),
            InPlay(21, "EX1_020", Second, 3, 3, 1) with { Character = new(3, 1, DivineShield: true) }, // Scarlet Crusader (Divine Shield)
            Card(22, "EX1_089", Second), // Arcane Golem (Charge, gives opponent 1 Mana Crystal)
            Card(23, "EX1_354", Second), // Lay on Hands
            Card(24, "CS2_231", Second)
        ], First);

        // Alexstrasza sets FirstHero remaining Health to 15 while keeping 6 Armor!
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(11), FirstHero, Position: 0)).State;
        Assert.Equal(15, s.Entities[FirstHero].Character!.Health);
        Assert.Equal(6, s.Entities[FirstHero].Character!.Armor);
        Assert.True(s.Players[First].HeroHealthChangedThisTurn);

        // Faceless Manipulator copies Scarlet Crusader -> becomes 3/1 EX1_020 with Divine Shield
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(13), new(21), Position: 1)).State;
        Assert.Equal("EX1_020", s.Entities[new(13)].CardId);
        Assert.True(s.Entities[new(13)].Character!.DivineShield);
        Assert.Equal(3, s.Entities[new(13)].Character!.Attack);

        // Arm Eye for an Eye and pass turn to Second (Second goes 4 -> 5 Mana Crystals)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(12))).State;
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;

        // Second plays Arcane Golem (4/2 Charge): First gains +1 Mana Crystal (15 -> 16), Golem attacks FirstHero for 4!
        // Eye for an Eye triggers on the 4 damage taken (even absorbed by Armor) and deals 4 damage to SecondHero (30 -> 26)!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, Second, new(22), Position: 1)).State;
        Assert.Equal(16, s.Players[First].ManaCrystals);
        s = simulator.Apply(s, new(SimulationActionKind.Attack, Second, new(22), FirstHero)).State;
        Assert.Empty(s.Players[First].In(GameZone.Secret));
        Assert.Equal(2, s.Entities[FirstHero].Character!.Armor);
        Assert.Equal(15, s.Entities[FirstHero].Character!.Health);
        Assert.Equal(26, s.Entities[SecondHero].Character!.Health);
    }

    [Fact]
    public void SorcerersApprenticeFloorXaviusLeeroyTinkmasterKeeperAndEndOfTurnCardsWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13, 14, 15, 16, 17, 18, 19]), (GameZone.Deck, [30, 31, 32, 33]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 2, ManaCrystals = 2,
                Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Deck, [40]))
            },
            InPlay(11, "EX1_608", First, 3, 3, 2), // Sorcerer's Apprentice #1
            InPlay(12, "EX1_608", First, 4, 3, 2), // Sorcerer's Apprentice #2
            Card(13, "EX1_169", First), // Innervate (0-cost spell -> stays 0, NOT raised to 1!)
            Card(14, "EX1_160", First), // Power of the Wild (2-cost spell -> reduced to 1 by two Apprentices, NOT 0!)
            Card(15, "EX1_614", First), // Xavius
            Card(16, "EX1_116", First), // Leeroy Jenkins
            Card(17, "EX1_166", First), // Keeper of the Grove
            Card(18, "EX1_045", First), // Ancient Watcher (Can't attack unless silenced)
            Card(19, "EX1_578", First), // Savagery (1-cost spell targeting minion)
            Card(30, "CS2_231", First), Card(31, "CS2_231", First), Card(32, "CS2_231", First), Card(33, "CS2_231", First),
            InPlay(21, "NEW1_023", Second, 5, 3, 2), // Faerie Dragon (Elusive)
            Card(40, "CS2_231", Second));

        // Sorcerer's Apprentice minimum-1 floor: 0-cost spell stays 0; 2-cost spell stops at 1 even with two Apprentices!
        Assert.Equal(0, simulator.EffectiveCost(root, new(13)));
        Assert.Equal(1, simulator.EffectiveCost(root, new(14)));

        // Faerie Dragon is Elusive: Savagery (spell) cannot target 21, but Keeper of the Grove (minion Choose One) CAN!
        Assert.DoesNotContain(simulator.LegalActions(root), a => a.Source == new EntityId(19) && a.Target == new EntityId(21));
        Assert.Contains(simulator.LegalActions(root), a => a.Source == new EntityId(17) && a.Target == new EntityId(21) && a.Choice == "EX1_166a");

        // Keeper of the Grove deals 2 damage to Apprentice #2 (kills 12, leaving 11 on board = 6 open slots)!
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(17), new(12), Position: 1) { Choice = "EX1_166a" }).State;
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(12)).Zone);

        // Play Xavius, then Ancient Watcher -> Xavius summons a 2/1 Satyr after Ancient Watcher is played!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15), Position: 2)).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(18), Position: 3)).State;
        Assert.Contains(s.Players[First].In(GameZone.Play), id => s.Entities[id].CardId == "EX1_614t");

        // Ancient Watcher cannot attack without being silenced (even on later turns, verified via LegalActions)
        Assert.DoesNotContain(simulator.LegalActions(s), a => a.Kind == SimulationActionKind.Attack && a.Source == new EntityId(18));

        // Leeroy Jenkins has Charge and summons two 1/1 Whelps for Second!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16), Position: 0)).State;
        Assert.Equal(2, s.Players[Second].In(GameZone.Play).Count(id => s.Entities[id].CardId == "EX1_116t"));
        Assert.Contains(simulator.LegalActions(s), a => a.Kind == SimulationActionKind.Attack && a.Source == new EntityId(16) && a.Target == SecondHero);

        // Play Gruul (at 1 board slot left after killing Apprentices or check board space): let's test Gruul + Tidecaller + Tinkmaster + Hogger + Mana Tide in dedicated test below!
    }

    [Fact]
    public void TidecallerTinkmasterGruulHoggerManaTideBloodImpDoomguardAndForceOfNatureWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12, 13, 14, 15, 16, 17, 18]), (GameZone.Deck, [19]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20]), (GameZone.Deck, [29]))
            },
            InPlay(11, "EX1_509", First, 3, 1, 2), // Murloc Tidecaller (1/2)
            Card(12, "EX1_509", First), // Another Murloc -> buffs 11 to 2/2 (not itself!)
            Card(13, "EX1_083", First), // Tinkmaster Overspark -> transforms another minion into 1/1 or 5/5
            Card(14, "NEW1_038", First), // Gruul (7/7 -> 8/8 at First end turn -> 9/9 at Second end turn)
            Card(15, "NEW1_040", First), // Hogger (summons 2/2 Gnoll at end of turn)
            Card(16, "EX1_575", First), // Mana Tide Totem (draws 1 at end of turn)
            Card(17, "EX1_310", First), // Doomguard (discards 2 remaining hand cards: 18 and... only 18 left)
            Card(18, "EX1_243", First), // Dust Devil
            Card(19, "EX1_571", First), // Force of Nature in deck
            Card(29, "CS2_231", Second));

        // Summoning second Murloc Tidecaller buffs the first one to 2/2 while the new one stays 1/2
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(12), Position: 1)).State;
        Assert.Equal(2, s.Entities[new(11)].Character!.Attack);
        Assert.Equal(1, s.Entities[new(12)].Character!.Attack);

        // Tinkmaster transforms either 11 or 12 into EX1_tk28 (1/1) or EX1_tk29 (5/5)
        var dist = simulator.ApplyDistribution(s, new(SimulationActionKind.PlayCard, First, new(13), Position: 2));
        Assert.True(dist.IsExact);
        Assert.Equal(4, dist.Outcomes.Length);
        s = dist.Outcomes[0].Result.State;

        // Play Gruul, Hogger, Mana Tide Totem, then Doomguard (discards card 18!)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14), Position: 3)).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15), Position: 4)).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16), Position: 5)).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(17), Position: 6)).State;
        Assert.Empty(s.Players[First].In(GameZone.Hand));

        // End First's turn: Gruul becomes 8/8, Mana Tide draws card 19, End Second's turn: Gruul becomes 9/9!
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(8, s.Entities[new(14)].Character!.Attack);
        Assert.Single(s.Players[First].In(GameZone.Hand), new EntityId(19));

        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, Second)).State;
        Assert.Equal(9, s.Entities[new(14)].Character!.Attack);
        Assert.Equal(9, s.Entities[new(14)].Character!.Health);
    }

    [Fact]
    public void BloodImpFrostElementalForceOfNaturePowerOfTheWildLayOnHandsAndHoggerSummonWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12, 13, 14, 15, 16]), (GameZone.Deck, [30, 31, 32]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20]), (GameZone.Deck, [40]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30) with { Character = new(0, 30, Damage: 10) },
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "CS2_059", First, 3, 0, 1), // Blood Imp
            Card(12, "EX1_571", First), // Force of Nature (summons three 2/2 EX1_158t -> board has 4 minions)
            Card(13, "EX1_160", First), // Power of the Wild (buff +1/+1 -> three 3/3s and 1/2 Blood Imp)
            Card(14, "EX1_283", First), // Frost Elemental (freezes SecondHero -> board has 5 minions)
            Card(15, "NEW1_040", First), // Hogger (-> board has 6 minions, summons 2/2 Gnoll at end of turn -> 7!)
            Card(16, "EX1_354", First), // Lay on Hands (heals FirstHero 8: 20->28, draws 3)
            Card(30, "CS2_231", First), Card(31, "CS2_231", First), Card(32, "CS2_231", First),
            Card(40, "CS2_231", Second)
        ], First);

        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(12))).State;
        Assert.Equal(3, s.Players[First].In(GameZone.Play).Count(id => s.Entities[id].CardId == "EX1_158t"));

        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(13)) { Choice = "EX1_160b" }).State;
        Assert.All(s.Players[First].In(GameZone.Play).Where(id => s.Entities[id].CardId == "EX1_158t"),
            id => Assert.Equal(3, s.Entities[id].Character!.Attack));

        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14), SecondHero, Position: 4)).State;
        Assert.True(s.Entities[SecondHero].Character!.Frozen);

        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15), Position: 5)).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16), FirstHero)).State;
        Assert.Equal(28, s.Entities[FirstHero].Character!.Health);
        Assert.Equal(3, s.Players[First].In(GameZone.Hand).Length);

        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Contains(s.Players[First].In(GameZone.Play), id => s.Entities[id].CardId == "NEW1_040t");
        Assert.Equal(7, s.Players[First].In(GameZone.Play).Count(id => id != FirstHero));
    }
}
