using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void YseraDreamCardsVoidTerrorShadowformLightwardenAndMoltenGiantWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11, 12, 13]), (GameZone.Hand, [14, 15, 16, 17, 18]), (GameZone.HeroPower, [19]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Deck, [40]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30) with { Character = new(0, 30, Damage: 15) }, // 15 Health -> Molten Giant costs 5!
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "EX1_572", First, 3, 4, 12), // Ysera
            InPlay(12, "EX1_001", First, 4, 1, 2) with { Character = new(1, 2, Damage: 1) }, // Damaged Lightwarden (1/1)
            InPlay(13, "CS2_231", First, 5, 3, 4),
            Card(14, "EX1_620", First), // Molten Giant (20 - 15 = 5 cost!)
            Card(15, "EX1_621", First), // Circle of Healing (heals damaged Lightwarden -> Lightwarden gains +2 Attack!)
            Card(16, "EX1_304", First), // Void Terror (placed between 12 and 13 -> destroys both & absorbs their stats!)
            Card(17, "EX1_625", First), // Shadowform -> Hero Power becomes EX1_625t (Deal 2 damage)
            Card(18, "DREAM_02", First), // Ysera Awakens (deals 5 to all characters except Ysera!)
            Card(19, "HERO_11bp", First),
            InPlay(21, "CS2_231", Second, 6, 2, 6),
            Card(40, "CS2_231", Second)
        ], First);

        // Molten Giant costs 20 - 15 = 5
        Assert.Equal(5, simulator.EffectiveCost(root, new(14)));

        // Circle of Healing heals Lightwarden (1/1 -> 1/2) and triggers Lightwarden (+2 Attack -> 3/2)!
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(15))).State;
        Assert.Equal(3, s.Entities[new(12)].Character!.Attack);
        Assert.Equal(2, s.Entities[new(12)].Character!.Health);

        // Void Terror placed at Position 2 (between 12 [3/2] and 13 [3/4]) -> destroys both, becomes 3+3+3=9 / 4+2+4=10!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16), Position: 2)).State;
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(12)).Zone);
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(13)).Zone);
        Assert.Equal(9, s.Entities[new(16)].Character!.Attack);
        Assert.Equal(10, s.Entities[new(16)].Character!.Health);

        // Shadowform replaces Hero Power with EX1_625t (Deal 2 damage)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(17))).State;
        var powerId = Assert.Single(s.Players[First].In(GameZone.HeroPower));
        Assert.Equal("EX1_625t", s.Entities[powerId].CardId);
        s = simulator.Apply(s, new(SimulationActionKind.UseHeroPower, First, powerId, SecondHero)).State;
        Assert.Equal(28, s.Entities[SecondHero].Character!.Health);

        // Ysera Awakens deals 5 damage to all characters EXCEPT Ysera (Ysera stays at 12 HP, Void Terror 10->5, SecondHero 28->23)!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(18))).State;
        Assert.Equal(12, s.Entities[new(11)].Character!.Health);
        Assert.Equal(5, s.Entities[new(16)].Character!.Health);
        Assert.Equal(23, s.Entities[SecondHero].Character!.Health);

        // End turn -> Ysera adds 2 random Dream cards to First's hand!
        var handBefore = s.Players[First].In(GameZone.Hand).Length;
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(handBefore + 2, s.Players[First].In(GameZone.Hand).Length);
    }

    [Fact]
    public void BlessingOfWisdomEaglehornEtherealArcanistUnleashMassDispelKodoAndSylvanasWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12, 13, 14, 15, 16, 17, 18]), (GameZone.Deck, [30, 31, 32]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 10, ManaCrystals = 10,
                Zones = Zones((GameZone.Play, [20, 21, 22]), (GameZone.Hand, [23]), (GameZone.Deck, [40]))
            },
            InPlay(11, "EX1_274", First, 3, 3, 3), // Ethereal Arcanist (3/3 -> +2/+2 at end of turn if controlling a Secret)
            Card(12, "EX1_536", First), // Eaglehorn Bow (3/2 -> +1 durability when friendly Secret revealed)
            Card(13, "EX1_132", First), // Eye for an Eye (Secret)
            Card(14, "EX1_363", First), // Blessing of Wisdom (cast on ENEMY minion 21 -> when 21 attacks, FIRST draws 1!)
            Card(15, "EX1_538", First), // Unleash the Hounds (Second has 2 minions -> summons two 1/1 Charge Hounds!)
            Card(16, "NEW1_041", First), // Stampeding Kodo (destroys random enemy minion with Attack <= 2 -> kills 22 [2/5], NOT 21 [4/4]!)
            Card(17, "EX1_626", First), // Mass Dispel (silences enemy minions + draws 1)
            Card(18, "EX1_014", First), // King Mukla (gives opponent 2 Bananas)
            Card(30, "CS2_231", First), Card(31, "CS2_231", First), Card(32, "CS2_231", First),
            InPlay(21, "EX1_016", Second, 4, 4, 4), // Sylvanas Windrunner (4/4)
            InPlay(22, "CS2_231", Second, 5, 2, 5), // 2/5 minion (valid Kodo target!)
            Card(23, "CS2_231", Second),
            Card(40, "CS2_231", Second));

        // Equip Eaglehorn Bow (3/2), arm Eye for an Eye, cast Blessing of Wisdom on ENEMY Sylvanas (21)
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(12))).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(13))).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14), new(21))).State;

        // Unleash the Hounds summons two 1/1 Charge Hounds (EX1_538t)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15))).State;
        Assert.Equal(2, s.Players[First].In(GameZone.Play).Count(id => s.Entities[id].CardId == "EX1_538t"));

        // Stampeding Kodo destroys 22 (Attack 2 <= 2) and leaves 21 (Attack 4) alive!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16), Position: 0)).State;
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(22)).Zone);
        Assert.Equal(GameZone.Play, s.LocationOf(new(21)).Zone);

        // King Mukla gives Second 2 Bananas (EX1_014t)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(18), Position: 0)).State;
        Assert.Equal(2, s.Players[Second].In(GameZone.Hand).Count(id => s.Entities[id].CardId == "EX1_014t"));

        // End First's turn -> Ethereal Arcanist grows +2/+2 (3/3 -> 5/5) because Eye for an Eye is armed!
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(5, s.Entities[new(11)].Character!.Attack);
        Assert.Equal(5, s.Entities[new(11)].Character!.Health);

        // Second attacks FirstHero with Sylvanas (21):
        // 1) Blessing of Wisdom (cast by First!) draws 1 card for First!
        // 2) Eye for an Eye reveals -> Eaglehorn Bow gains +1 Durability (2 -> 3)!
        var firstHandBefore = s.Players[First].In(GameZone.Hand).Length;
        s = simulator.Apply(s, new(SimulationActionKind.Attack, Second, new(21), FirstHero)).State;
        Assert.Equal(firstHandBefore + 1, s.Players[First].In(GameZone.Hand).Length);
        Assert.Equal(3, s.Entities[s.Players[First].EquippedWeapon!.Value].Weapon!.Durability);
    }

    [Fact]
    public void AlarmOBotBlackKnightBigGameHunterHighmaneHyenaAndAvengingWrathWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13, 14, 15, 16]), (GameZone.Deck, [30]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21, 22]), (GameZone.Deck, [40]))
            },
            InPlay(11, "EX1_006", First, 3, 0, 3), // Alarm-o-Bot
            InPlay(12, "EX1_531", First, 4, 2, 2), // Scavenging Hyena
            Card(13, "EX1_002", First), // The Black Knight (destroys enemy Taunt 21!)
            Card(14, "EX1_005", First), // Big Game Hunter (destroys 7+ Attack minion 22!)
            Card(15, "EX1_534", First), // Savannah Highmane (6/5 Beast -> Deathrattle two 2/2 Hyenas)
            Card(16, "EX1_384", First), // Avenging Wrath (8 random damage to enemies)
            Card(30, "CS2_231", First),
            InPlay(21, "EX1_396", Second, 5, 1, 7), // Mogu'shan Warden (Taunt)
            InPlay(22, "EX1_543", Second, 6, 8, 8), // King Krush (8/8 -> BGH target!)
            Card(40, "CS2_231", Second));

        // The Black Knight destroys Taunt 21; Big Game Hunter destroys 8/8 King Krush 22!
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(13), new(21), Position: 2)).State;
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(21)).Zone);
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14), new(22), Position: 3)).State;
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(22)).Zone);

        // Avenging Wrath deals 8 damage to SecondHero (30 -> 22) since no enemy minions remain!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16))).State;
        Assert.Equal(22, s.Entities[SecondHero].Character!.Health);

        // End First's turn, End Second's turn -> at start of First's next turn Alarm-o-Bot swaps with Savannah Highmane (15) in hand!
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, Second)).State;
        Assert.Equal(GameZone.Hand, s.LocationOf(new(11)).Zone);
        Assert.Equal(GameZone.Play, s.LocationOf(new(15)).Zone);
    }
}
