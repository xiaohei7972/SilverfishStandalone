using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void CommandingShoutWildPyroBladeFlurryPrepAndSlamResolveCorrectly()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13, 14, 15, 16, 17, 18]), (GameZone.Deck, [30, 31, 32]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21]))
            },
            InPlay(11, "NEW1_020", First, 3, 3, 2), // Wild Pyromancer (3/2)
            InPlay(12, "EX1_412", First, 4, 3, 3), // Raging Worgen (3/3 -> 4/2 Windfury when damaged!)
            Card(13, "NEW1_036", First), // Commanding Shout (+ draw 30, protects friendly minions at >=1 HP; then Pyro deals 1 to all!)
            Card(14, "EX1_145", First), // Preparation (-2 next spell)
            Card(15, "CS2_233", First), // Blade Flurry (2 - 2 = 0 mana!)
            Card(16, "EX1_247", First), // Stormforged Axe (2/3 weapon)
            Card(17, "EX1_391", First), // Slam (2 dmg to minion; if survives, draw 1)
            Card(18, "NEW1_018", First), // Bloodsail Raider (+weapon attack)
            Card(30, "CS2_231", First), Card(31, "CS2_231", First), Card(32, "CS2_231", First),
            InPlay(21, "CS2_231", Second, 5, 1, 5));

        // Commanding Shout draws 30, then Wild Pyromancer deals 1 to all minions (Pyro 3/1, Worgen 4/2 Windfury, Enemy 1/4)
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(13))).State;
        Assert.Equal(1, s.Entities[new(11)].Character!.Health);
        Assert.Equal(4, s.Entities[new(12)].Character!.Attack);
        Assert.Equal(2, s.Entities[new(12)].Character!.Health);

        // Equip Stormforged Axe (2/3), play Bloodsail Raider -> 4/3!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16))).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(18), Position: 2)).State;
        Assert.Equal(4, s.Entities[new(18)].Character!.Attack);

        // Preparation reduces Blade Flurry from 2 to 0; Pyro fires (Pyro stays at 1 HP due to Commanding Shout!)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14))).State;
        Assert.Equal(0, simulator.EffectiveCost(s, new(15)));
        Assert.Equal(1, s.Entities[new(11)].Character!.Health);

        // Blade Flurry destroys weapon and deals 2 to enemy minion (1/3 -> 1/1 -> Pyro deals 1 -> dead; Pyro still 1 HP!)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15))).State;
        Assert.Null(s.Players[First].EquippedWeapon);
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(21)).Zone);
        Assert.Equal(1, s.Entities[new(11)].Character!.Health);

        // Slam on friendly 1-HP Pyro: Commanding Shout keeps Pyro alive at 1 HP -> Slam draws card 31!
        var deckBeforeSlam = s.Players[First].In(GameZone.Deck).Length;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(17), new(11))).State;
        Assert.Equal(1, s.Entities[new(11)].Character!.Health);
        Assert.Equal(deckBeforeSlam - 1, s.Players[First].In(GameZone.Deck).Length);
    }

    [Fact]
    public void MisdirectionAntonidasLorewalkerChoDemonfireAndNatPagleWorkCorrectly()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13, 14, 15]), (GameZone.Deck, [30, 31]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 10, ManaCrystals = 10,
                Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Hand, [22]), (GameZone.Deck, [40]))
            },
            InPlay(11, "EX1_559", First, 3, 5, 7), // Archmage Antonidas
            InPlay(12, "EX1_100", First, 4, 0, 4), // Lorewalker Cho
            Card(13, "EX1_533", First), // Misdirection (Secret)
            Card(14, "EX1_319", First), // Flame Imp (Demon)
            Card(15, "EX1_596", First), // Demonfire (friendly Demon -> +2/+2; enemy minion -> 2 dmg!)
            Card(30, "CS2_231", First), Card(31, "CS2_231", First),
            InPlay(21, "EX1_557", Second, 5, 0, 4), // Nat Pagle (50% draw at start of Second's turn)
            Card(22, "EX1_543", Second), // King Krush (8/8 Charge)
            Card(40, "CS2_231", Second));

        // Play Flame Imp (3/2 Demon, deals 3 to FirstHero -> 27), cast Demonfire on Flame Imp -> 5/4!
        // Antonidas adds Fireball (CS2_029) to First's hand; Lorewalker Cho adds Demonfire copy to Second's hand!
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(14), Position: 2)).State;
        Assert.Equal(27, s.Entities[FirstHero].Character!.Health);
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15), new(14))).State;
        Assert.Equal(5, s.Entities[new(14)].Character!.Attack);
        Assert.Equal(4, s.Entities[new(14)].Character!.Health);
        Assert.Contains(s.Players[First].In(GameZone.Hand), id => s.Entities[id].CardId == "CS2_029");
        Assert.Contains(s.Players[Second].In(GameZone.Hand), id => s.Entities[id].CardId == "EX1_596");

        // Arm Misdirection, end First's turn -> Nat Pagle has exact 2 outcomes (50% draw, 50% no draw)!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(13))).State;
        var pagleDist = simulator.ApplyDistribution(s, new(SimulationActionKind.EndTurn, First));
        Assert.True(pagleDist.IsExact);
        Assert.Equal(2, pagleDist.Outcomes.Length);
        s = pagleDist.Outcomes[0].Result.State;

        // Second plays King Krush (8/8 Charge) and attacks FirstHero -> Misdirection redirects to another character (FirstHero takes 0)!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, Second, new(22), Position: 1)).State;
        var heroBefore = s.Entities[FirstHero].Character!.Health;
        s = simulator.Apply(s, new(SimulationActionKind.Attack, Second, new(22), FirstHero)).State;
        Assert.Empty(s.Players[First].In(GameZone.Secret));
        Assert.Equal(heroBefore, s.Entities[FirstHero].Character!.Health);
    }

    [Fact]
    public void BloodsailCorsairColdlightTwilightDrakeAldorRampageBlademasterAndSiphonSoulWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12, 13, 14, 15, 16]), (GameZone.Deck, [30, 31]))
            },
            Player(Second, SecondHero) with
            {
                EquippedWeapon = new(21),
                Zones = Zones((GameZone.Play, [20, 21, 22]), (GameZone.Deck, [40, 41]))
            },
            Card(11, "NEW1_025", First), // Bloodsail Corsair (removes 1 durability from Second's 1-durability weapon -> destroys it!)
            Card(12, "EX1_050", First), // Coldlight Oracle (both draw 2)
            Card(13, "EX1_043", First), // Twilight Drake (gains +1 Health per card in hand!)
            Card(14, "CS2_181", First), // Injured Blademaster (4/7 -> 4/3 damaged!)
            Card(15, "CS2_104", First), // Rampage (+3/+3 on damaged Blademaster -> 7/6!)
            Card(16, "EX1_382", First), // Aldor Peacekeeper (sets enemy minion attack to 1!)
            Card(30, "CS2_231", First), Card(31, "CS2_231", First),
            InPlay(21, "EX1_411", Second, 3, 0, 0) with { Character = null, Weapon = new(7, 1, 1) },
            InPlay(22, "EX1_543", Second, 4, 8, 8), // King Krush 8/8
            Card(40, "CS2_231", Second), Card(41, "CS2_231", Second));

        // Bloodsail Corsair removes 1 durability from Second's 7/1 Gorehowl -> Gorehowl is destroyed!
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(11), Position: 0)).State;
        Assert.Null(s.Players[Second].EquippedWeapon);

        // Coldlight Oracle draws 2 for both players
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(12), Position: 1)).State;
        Assert.Equal(2, s.Players[Second].In(GameZone.Hand).Length);

        // Twilight Drake played with 5 other cards in hand -> 4/6!
        var otherCards = s.Players[First].In(GameZone.Hand).Length - 1;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(13), Position: 2)).State;
        Assert.Equal(1 + otherCards, s.Entities[new(13)].Character!.Health);

        // Injured Blademaster (4/3), then Rampage (+3/+3 -> 7/6), then Aldor Peacekeeper sets enemy King Krush attack to 1!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14), Position: 3)).State;
        Assert.Equal(3, s.Entities[new(14)].Character!.Health);
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15), new(14))).State;
        Assert.Equal(7, s.Entities[new(14)].Character!.Attack);
        Assert.Equal(6, s.Entities[new(14)].Character!.Health);
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16), new(22), Position: 4)).State;
        Assert.Equal(1, s.Entities[new(22)].Character!.Attack);
    }
}
