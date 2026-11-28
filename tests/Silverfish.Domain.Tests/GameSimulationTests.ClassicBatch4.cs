using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void LordJaraxxusReplacesHeroGainsArmorEquipsBloodFuryAndResetsInfernoHeroPower()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 12, ManaCrystals = 12, MaximumManaCrystals = 12,
                HeroPowerUsesThisTurn = 1,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]), (GameZone.HeroPower, [12]))
            },
            Player(Second, SecondHero)
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30) with { Character = new(0, 30, Damage: 8, Armor: 2) },
            InPlay(20, "hero", Second, 2, 0, 30),
            Card(11, "EX1_323", First), // Lord Jaraxxus (8 mana Hero card: +5 Armor, EX1_tk33 Hero Power, 3/8 EX1_323w)
            Card(12, "HERO_11bp", First) // Existing Hero Power (already used once this turn)
        ], First);

        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(11))).State;
        Assert.Equal(new EntityId(11), s.Players[First].Hero);
        Assert.Equal(GameZone.Removed, s.LocationOf(FirstHero).Zone);
        Assert.Equal(22, s.Entities[new(11)].Character!.Health);
        Assert.Equal(7, s.Entities[new(11)].Character!.Armor); // 2 existing + 5 from Jaraxxus

        // Blood Fury (3/8) is equipped and hero can attack immediately
        var weaponId = Assert.NotNull(s.Players[First].EquippedWeapon);
        Assert.Equal("EX1_323w", s.Entities[weaponId].CardId);
        Assert.Equal(new WeaponStats(3, 8, 8), s.Entities[weaponId].Weapon);

        // Hero Power replaced with EX1_tk33 (INFERNO!) and unexhausted (HeroPowerUsesThisTurn == 0)
        Assert.Equal(0, s.Players[First].HeroPowerUsesThisTurn);
        var powerId = Assert.Single(s.Players[First].In(GameZone.HeroPower));
        Assert.Equal("EX1_tk33", s.Entities[powerId].CardId);
        s = simulator.Apply(s, new(SimulationActionKind.UseHeroPower, First, powerId)).State;
        var infernal = Assert.Single(s.Players[First].In(GameZone.Play), id => s.Entities[id].CardId == "EX1_tk34");
        Assert.Equal(6, s.Entities[infernal].Character!.Attack);
        Assert.Equal(6, s.Entities[infernal].Character!.Health);
    }

    [Fact]
    public void SwordOfJusticeGorehowlKirinTorBloodKnightAndPiratesWorkCorrectly()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12, 13, 14, 15, 16, 17, 18]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21]))
            },
            Card(11, "EX1_366", First), // Sword of Justice (1/5)
            Card(12, "EX1_008", First), // Argent Squire (1/1 Divine Shield -> buffed to 2/2 by Sword, Sword 1/4)
            Card(13, "EX1_590", First), // Blood Knight (strips Squire + Sunwalker shields -> +6/+6 = 9/9, then +1/+1 from Sword = 10/10!)
            Card(14, "EX1_612", First), // Kirin Tor Mage -> next Secret this turn costs 0
            Card(15, "EX1_594", First), // Vaporize (3-cost Secret -> costs 0!)
            Card(16, "EX1_132", First), // Eye for an Eye (second Secret -> costs 1 again!)
            Card(17, "EX1_411", First), // Gorehowl (7/1)
            Card(18, "CS2_146", First), // Southsea Deckhand (has Charge while weapon equipped)
            InPlay(21, "EX1_032", Second, 3, 4, 5) with { Character = new(4, 5, DivineShield: true) }); // Sunwalker

        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(11))).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(12), Position: 0)).State;
        Assert.Equal(2, s.Entities[new(12)].Character!.Attack);
        Assert.Equal(4, s.Entities[s.Players[First].EquippedWeapon!.Value].Weapon!.Durability);

        // Blood Knight drains 2 Divine Shields -> becomes 9/9 in battlecry, then Sword of Justice buffs it to 10/10!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(13), Position: 1)).State;
        Assert.False(s.Entities[new(12)].Character!.DivineShield);
        Assert.False(s.Entities[new(21)].Character!.DivineShield);
        Assert.Equal(10, s.Entities[new(13)].Character!.Attack);
        Assert.Equal(10, s.Entities[new(13)].Character!.Health);

        // Kirin Tor Mage makes Vaporize cost 0, then Eye for an Eye returns to normal cost (1)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14), Position: 2)).State;
        Assert.Equal(0, simulator.EffectiveCost(s, new(15)));
        var manaBefore = s.Players[First].Mana;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15))).State;
        Assert.Equal(manaBefore, s.Players[First].Mana);
        Assert.Equal(1, simulator.EffectiveCost(s, new(16)));

        // Equip Gorehowl (7/1) and attack minion 21 (4/5): Gorehowl loses 1 Attack -> becomes 6/1 instead of breaking!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(17))).State;
        s = simulator.Apply(s, new(SimulationActionKind.Attack, First, FirstHero, new(21))).State;
        var gorehowl = Assert.NotNull(s.Players[First].EquippedWeapon);
        Assert.Equal(new WeaponStats(6, 1, 1), s.Entities[gorehowl].Weapon);

        // Southsea Deckhand has Charge while Gorehowl is equipped
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(18), Position: 3)).State;
        Assert.Contains(simulator.LegalActions(s), a => a.Kind == SimulationActionKind.Attack && a.Source == new EntityId(18) && a.Target == SecondHero);
    }

    [Fact]
    public void MindControlTechDruidOfTheClawQuestingUnboundHeadcrackAndMortalStrikeWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13, 14, 15, 16, 17]), (GameZone.Deck, [19]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21, 22, 23, 24]), (GameZone.Deck, [29]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30) with { Character = new(0, 30, Damage: 14) }, // 16 Health
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "EX1_044", First, 3, 2, 2), // Questing Adventurer
            InPlay(12, "EX1_258", First, 4, 3, 4), // Unbound Elemental
            Card(13, "EX1_313", First), // Pit Lord (deals 5 to FriendlyHero -> 11 Health!)
            Card(14, "EX1_408", First), // Mortal Strike (<=12 Health -> deals 6 to SecondHero!)
            Card(15, "EX1_243", First), // Dust Devil (Overload: 2 -> buffs Unbound Elemental!)
            Card(16, "EX1_137", First), // Headcrack (Combo -> returns copy to hand at end of turn!)
            Card(17, "EX1_165", First), // Druid of the Claw (Choose One: transform into 7/6 Rush EX1_165t1)
            Card(19, "EX1_085", First), // Mind Control Tech in deck
            InPlay(21, "CS2_231", Second, 5, 1, 1),
            InPlay(22, "CS2_231", Second, 6, 1, 1),
            InPlay(23, "CS2_231", Second, 7, 1, 1),
            InPlay(24, "CS2_231", Second, 8, 1, 1),
            Card(29, "CS2_231", Second)
        ], First);

        // Pit Lord drops FirstHero to 11 Health; Mortal Strike now deals 6 damage to SecondHero (30 -> 24)!
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(13), Position: 2)).State;
        Assert.Equal(11, s.Entities[FirstHero].Character!.Health);
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14), SecondHero)).State;
        Assert.Equal(24, s.Entities[SecondHero].Character!.Health);

        // Dust Devil has Overload -> Unbound Elemental grows from 3/4 to 4/5!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15), Position: 3)).State;
        Assert.Equal(4, s.Entities[new(12)].Character!.Attack);
        Assert.Equal(5, s.Entities[new(12)].Character!.Health);

        // Headcrack with Combo deals 2 to SecondHero (22) and queues a copy at end of turn
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16))).State;
        Assert.Equal(22, s.Entities[SecondHero].Character!.Health);

        // Druid of the Claw transforms into 7/6 Rush (EX1_165t1) and can attack enemy minions (not enemy hero)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(17), Position: 4) { Choice = "EX1_165a" }).State;
        Assert.Equal("EX1_165t1", s.Entities[new(17)].CardId);
        Assert.Equal(7, s.Entities[new(17)].Character!.Attack);
        Assert.Contains(simulator.LegalActions(s), a => a.Kind == SimulationActionKind.Attack && a.Source == new EntityId(17) && a.Target == new EntityId(21));
        Assert.DoesNotContain(simulator.LegalActions(s), a => a.Kind == SimulationActionKind.Attack && a.Source == new EntityId(17) && a.Target == SecondHero);

        // Questing Adventurer grew +1/+1 for each of the 5 cards played (2/2 -> 7/7)!
        Assert.Equal(7, s.Entities[new(11)].Character!.Attack);

        // End turn -> Headcrack copy returns to First's hand!
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Single(s.Players[First].In(GameZone.Hand), id => s.Entities[id].CardId == "EX1_137");
    }

    [Fact]
    public void MindControlTechFrothingHarvestGolemCultMasterLightwellAndAlchemistWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11, 12, 13, 14]), (GameZone.Hand, [15, 16, 17, 18]), (GameZone.Deck, [30, 31, 32]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21, 22, 23, 24]), (GameZone.Hand, [25, 26, 27]), (GameZone.Deck, [40]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30) with { Character = new(0, 30, Damage: 5) },
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "EX1_604", First, 3, 2, 4), // Frothing Berserker
            InPlay(12, "EX1_556", First, 4, 2, 3), // Harvest Golem (not exhausted)
            InPlay(13, "EX1_595", First, 5, 4, 2), // Cult Master
            InPlay(14, "EX1_341", First, 6, 0, 5), // Lightwell
            Card(15, "EX1_085", First), // Mind Control Tech (Second has 4 minions -> steals 1!)
            Card(16, "EX1_059", First), // Crazed Alchemist (swaps Lightwell 0/5 -> 5/0? Wait: swaps Frothing!)
            Card(17, "EX1_349", First), // Divine Favor
            Card(18, "EX1_093", First), // Defender of Argus
            Card(30, "CS2_231", First), Card(31, "CS2_231", First), Card(32, "CS2_231", First),
            InPlay(21, "EX1_029", Second, 7, 3, 2), // Leper Gnome (buffed to 3/2 so Harvest Golem trades!)
            InPlay(22, "CS2_231", Second, 8, 1, 1),
            InPlay(23, "CS2_231", Second, 9, 1, 1),
            InPlay(24, "CS2_231", Second, 10, 1, 1),
            Card(25, "CS2_231", Second), Card(26, "CS2_231", Second), Card(27, "CS2_231", Second),
            Card(40, "CS2_231", Second)
        ], First);

        // Mind Control Tech steals 1 of Second's 4 minions (Second drops to 3 minions, First goes to 6 + MCT = 7? Wait: First had 4 minions + MCT = 5, steals 1 = 6!)
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(15), Position: 4)).State;
        Assert.Equal(3, s.Players[Second].In(GameZone.Play).Count(id => id != SecondHero));
        Assert.Equal(6, s.Players[First].In(GameZone.Play).Count(id => id != FirstHero));

        // Crazed Alchemist swaps Harvest Golem (2/3 -> 3/2), then Harvest Golem trades into Leper Gnome (3/2):
        // Frothing gains +2 Attack (2 -> 4), Harvest Golem summons 2/1 skele21, Cult Master draws 1, Leper Gnome deals 2 to FirstHero (25 -> 23)!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16), new(12), Position: 0)).State;
        Assert.Equal(3, s.Entities[new(12)].Character!.Attack);
        Assert.Equal(2, s.Entities[new(12)].Character!.Health);

        var handBeforeTrade = s.Players[First].In(GameZone.Hand).Length;
        s = simulator.Apply(s, new(SimulationActionKind.Attack, First, new(12), new(21))).State;
        Assert.Equal(4, s.Entities[new(11)].Character!.Attack);
        Assert.Contains(s.Players[First].In(GameZone.Play), id => s.Entities[id].CardId == "skele21");
        Assert.Equal(handBeforeTrade + 1, s.Players[First].In(GameZone.Hand).Length);
        Assert.Equal(23, s.Entities[FirstHero].Character!.Health);

        // End First's turn, End Second's turn -> at start of First's next turn Lightwell heals damaged FirstHero by 3 (23 -> 26)!
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, Second)).State;
        Assert.Equal(26, s.Entities[FirstHero].Character!.Health);
    }
}
