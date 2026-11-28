using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void GvgPart2TroggShadowboxerKezanCannonAnimaDruidSiegeSniperJeevesSapperWork()
    {
        var simulator = new GameSimulator([
            .. StarterCardCatalog.Cards,
            Hero,
            new SimulationCard(new CardDefinition("DS1h_292", EntityKind.HeroPower, 2))
            {
                PlayReqs = new([new(ErrorType.REQ_STEADY_SHOT)], []),
                OnPlay = [new DamageEffect(DamageTarget.Selected, 2)]
            }
        ]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones(
                    (GameZone.Play, [10, 11, 12, 13, 14, 15]),
                    (GameZone.Hand, [30, 31, 32, 33, 34, 35, 36, 37]),
                    (GameZone.HeroPower, [19]),
                    (GameZone.Deck, [50, 51, 52]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 10, ManaCrystals = 10, MaximumManaCrystals = 10,
                Zones = Zones(
                    (GameZone.Play, [20, 21]),
                    (GameZone.Hand, [60, 61, 62, 63, 64, 65]),
                    (GameZone.Secret, [29]),
                    (GameZone.Deck, [70, 71, 72, 73]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "GVG_067", First, 3, 2, 3), // Stonesplinter Trogg
            InPlay(12, "GVG_068", First, 4, 3, 5), // Burly Rockjaw Trogg
            InPlay(13, "GVG_118", First, 5, 6, 6), // Troggzor the Earthinator
            InPlay(14, "GVG_072", First, 6, 2, 3), // Shadowboxer
            InPlay(15, "GVG_086", First, 7, 5, 5), // Siege Engine
            InPlay(21, "CS2_182", Second, 8, 4, 5) with { Character = new(4, 5, Damage: 2) }, // Damaged enemy Yeti (3 HP)
            Card(19, "DS1h_292", First), // Hunter Steady Shot
            Card(29, "EX1_289", Second) with { PlayOrder = 9 }, // Enemy Ice Barrier secret
            Card(30, "GVG_074", First), // Kezan Mystic
            Card(31, "GVG_075", First), // Ship's Cannon
            Card(32, "GVG_070", First), // Salty Dog (Pirate)
            Card(33, "GVG_080", First), // Druid of the Fang
            Card(34, "GVG_053", First), // Shieldmaiden (+5 Armor -> Siege Engine +1 Atk)
            Card(35, "GVG_087", First), // Steamwheedle Sniper
            Card(36, "GVG_095", First), // Goblin Sapper (+4 Atk while opp has >= 6 hand cards)
            Card(37, "GVG_012", First), // Light of the Naaru (heals Yeti -> triggers Shadowboxer!)
            Card(50, "CS2_231", First),
            Card(51, "CS2_231", First),
            Card(52, "CS2_231", First),
            Card(60, "PART_001", Second),
            Card(61, "CS2_231", Second),
            Card(62, "CS2_231", Second),
            Card(63, "CS2_231", Second),
            Card(64, "CS2_231", Second),
            Card(65, "CS2_231", Second),
            Card(70, "CS2_231", Second),
            Card(71, "CS2_231", Second),
            Card(72, "CS2_231", Second),
            Card(73, "CS2_231", Second)
        ], First);

        // Goblin Sapper (36): opponent has 6 hand cards -> 2 + 4 = 6 Attack on board!
        var sSapper = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(36), Position: 0)).State;
        Assert.Equal(6, simulator.EffectiveAttack(sSapper, sSapper.Entities[new(36)]));

        // Kezan Mystic (30): steals Second's Secret (29)
        var sKezan = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(30), Position: 0)).State;
        Assert.Empty(sKezan.Players[Second].In(GameZone.Secret));
        Assert.Contains(new EntityId(29), sKezan.Players[First].In(GameZone.Secret));

        // Shieldmaiden (34): +5 Hero Armor -> Siege Engine (15) gains +1 Attack (5 -> 6)
        var sShield = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(34), Position: 0)).State;
        Assert.Equal(5, sShield.Entities[FirstHero].Character!.Armor);
        Assert.Equal(6, sShield.Entities[new(15)].Character!.Attack);

        // Steady Shot (19) CANNOT target enemy Yeti (21) before Steamwheedle Sniper (35), CAN after!
        Assert.NotNull(simulator.CheckAction(root, new(SimulationActionKind.UseHeroPower, First, new(19), new(21))));
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(35), Position: 0)).State;
        Assert.Null(simulator.CheckAction(s, new(SimulationActionKind.UseHeroPower, First, new(19), new(21))));
        s = simulator.Apply(s, new(SimulationActionKind.UseHeroPower, First, new(19), new(21))).State;
        Assert.Equal(1, s.Entities[new(21)].Character!.Health); // Yeti 3 HP -> 1 HP

        // Heal damaged Yeti (21) with Light of the Naaru (37) -> heals 3 & Shadowboxer (14) fires 1 random enemy damage (-3 + 1 = -2 net enemy damage!)
        var enemyDamageBefore = (30 - s.Entities[SecondHero].Character!.Health) + s.Entities[new(21)].Character!.Damage;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(37), new(21))).State;
        var enemyDamageAfter = (30 - s.Entities[SecondHero].Character!.Health) + s.Entities[new(21)].Character!.Damage;
        Assert.Equal(enemyDamageBefore - 2, enemyDamageAfter);

        // EndTurn from root -> Second casts Armor Plating (60) -> Stonesplinter (+1), Rockjaw (+2), Troggzor summons GVG_068!
        var sTrogg = simulator.Apply(root, new(SimulationActionKind.EndTurn, First)).State;
        sTrogg = simulator.Apply(sTrogg, new(SimulationActionKind.PlayCard, Second, new(60), new(21))).State;
        Assert.Equal(3, sTrogg.Entities[new(11)].Character!.Attack);
        Assert.Equal(5, sTrogg.Entities[new(12)].Character!.Attack);
        Assert.Equal(2, sTrogg.Players[First].In(GameZone.Play).Count(id => sTrogg.Entities[id].CardId == "GVG_068"));

        // Anima Golem (GVG_077), Druid of the Fang (GVG_080), Gnomish Experimenter (GVG_092), Jeeves (GVG_094), Ship's Cannon (31+32)
        var s2Root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones(
                    (GameZone.Play, [10, 11, 12]),
                    (GameZone.Hand, [30, 31, 32, 33]),
                    (GameZone.Deck, [50, 51, 52, 53]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Deck, [60]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "GVG_077", First, 3, 9, 9), // Anima Golem (with ally 12!)
            InPlay(12, "GVG_046", First, 4, 2, 5), // Friendly Beast (King of Beasts)
            InPlay(21, "CS2_182", Second, 5, 4, 5),
            Card(30, "GVG_080", First), // Druid of the Fang -> transforms into 7/7 GVG_080t!
            Card(31, "GVG_092", First), // Gnomish Experimenter -> draws top minion (50) and turns it into Chicken GVG_092t!
            Card(32, "GVG_075", First), // Ship's Cannon
            Card(33, "GVG_070", First), // Salty Dog (Pirate -> Cannon fires 2 dmg)
            Card(50, "CS2_182", First), // Top card: minion Yeti
            Card(51, "CS2_231", First),
            Card(52, "CS2_231", First),
            Card(53, "CS2_231", First),
            Card(60, "CS2_231", Second)
        ], First);

        // Druid of the Fang (30) with Beast (12) transforms into GVG_080t (7/7)
        var s2 = simulator.Apply(s2Root, new(SimulationActionKind.PlayCard, First, new(30), Position: 0)).State;
        Assert.Equal("GVG_080t", s2.Entities[new(30)].CardId);
        Assert.Equal(7, s2.Entities[new(30)].Character!.Attack);

        // Gnomish Experimenter (31) draws top minion (50) -> transformed into GVG_092t Chicken in hand!
        s2 = simulator.Apply(s2, new(SimulationActionKind.PlayCard, First, new(31), Position: 0)).State;
        Assert.Equal("GVG_092t", s2.Entities[new(50)].CardId);

        // Ship's Cannon (32) + Pirate Salty Dog (33) -> deals 2 enemy damage!
        s2 = simulator.Apply(s2, new(SimulationActionKind.PlayCard, First, new(32), Position: 0)).State;
        var enemyTotalBeforeCannon = s2.Entities[SecondHero].Character!.Health + s2.Entities[new(21)].Character!.Health;
        s2 = simulator.Apply(s2, new(SimulationActionKind.PlayCard, First, new(33), Position: 0)).State;
        var enemyTotalAfterCannon = s2.Entities[SecondHero].Character!.Health + s2.Entities[new(21)].Character!.Health;
        Assert.Equal(enemyTotalBeforeCannon - 2, enemyTotalAfterCannon);

        // Anima Golem alone at EndTurn -> self-destructs! Jeeves draws until 3 hand cards!
        var aloneRoot = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 10, ManaCrystals = 10, MaximumManaCrystals = 10,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, []), (GameZone.Deck, [50, 51, 52, 53]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 22]), (GameZone.Deck, [60]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "GVG_077", First, 3, 9, 9), // Only friendly minion!
            InPlay(22, "GVG_094", Second, 4, 1, 4), // Enemy Jeeves (TurnEnded TriggerRelation.Any!)
            Card(50, "CS2_231", First),
            Card(51, "CS2_231", First),
            Card(52, "CS2_231", First),
            Card(53, "CS2_231", First),
            Card(60, "CS2_231", Second)
        ], First);

        var aloneEnd = simulator.Apply(aloneRoot, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(GameZone.Graveyard, aloneEnd.LocationOf(new(11)).Zone);
        Assert.Equal(3, aloneEnd.Players[First].In(GameZone.Hand).Length);
    }

    [Fact]
    public void GvgPart2DeathrattlesWatcherPurifierHobgoblinEnhanceRecombobulatorAndBoomBotsWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones(
                    (GameZone.Play, [10, 11, 12, 13]),
                    (GameZone.Hand, [30, 31, 32, 33, 34, 35, 36]),
                    (GameZone.Deck, [50, 51]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 10, ManaCrystals = 10, MaximumManaCrystals = 10,
                Zones = Zones(
                    (GameZone.Play, [20, 21, 22]),
                    (GameZone.Deck, [60, 61]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "GVG_100", First, 3, 4, 4), // Floating Watcher
            InPlay(12, "GVG_104", First, 4, 2, 3), // Hobgoblin
            InPlay(13, "GVG_096", First, 5, 4, 3) with { Character = new(4, 3, Damage: 2) }, // Piloted Shredder (1 HP, Deathrattle)
            InPlay(21, "GVG_105", Second, 6, 6, 4), // Enemy Piloted Sky Golem (Deathrattle)
            InPlay(22, "GVG_114", Second, 7, 5, 7), // Enemy Sneed's Old Shredder (Deathrattle)
            Card(30, "GVG_097", First), // Lil' Exorcist (2 enemy DR -> +2/+2 -> 4/5!)
            Card(31, "GVG_101", First), // Scarlet Purifier (2 dmg to DR minions -> kills Shredder 13 & damages 21/22!)
            Card(32, "CS2_231", First), // Wisp (1-Atk ->? No Wisp is 1 Atk -> Hobgoblin buffs it +2/+2 -> 3/3!)
            Card(33, "GVG_108", First), // Recombobulator (transforms Lil' Exorcist 3-cost -> random 3-cost minion)
            Card(34, "GVG_107", First), // Enhance-o Mechano
            Card(35, "GVG_110", First), // Dr. Boom (summons two GVG_110t Boom Bots)
            Card(36, "GVG_009", First), // Shadowbomber (deals 3 dmg to FriendlyHero on First's turn -> Floating Watcher +2/+2!)
            Card(50, "CS2_231", First),
            Card(51, "CS2_231", First),
            Card(60, "CS2_231", Second),
            Card(61, "CS2_231", Second)
        ], First);

        // Lil' Exorcist (30): 2 enemy Deathrattle minions (21, 22) -> 2/3 + 2/2 = 4/5!
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(30), Position: 0)).State;
        Assert.Equal(4, s.Entities[new(30)].Character!.Attack);
        Assert.Equal(5, s.Entities[new(30)].Character!.Health);

        // Scarlet Purifier (31) from root: deals 2 dmg to DR minions only -> kills Shredder (13) (summons 2-cost minion!), damages Sky Golem (21: 4->2) & Sneed's (22: 7->5), leaves Watcher/Hobgoblin undamaged!
        var purifyCountBefore = root.Players[First].In(GameZone.Play).Length;
        var sPurify = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(31), Position: 0)).State;
        Assert.Equal(GameZone.Graveyard, sPurify.LocationOf(new(13)).Zone);
        Assert.Equal(2, sPurify.Entities[new(21)].Character!.Health);
        Assert.Equal(5, sPurify.Entities[new(22)].Character!.Health);
        Assert.Equal(0, sPurify.Entities[new(11)].Character!.Damage);
        // Shredder summoned a 2-cost minion (+1 Purifier, -1 Shredder, +1 token = +1 net!)
        Assert.Equal(purifyCountBefore + 1, sPurify.Players[First].In(GameZone.Play).Length);

        // Shadowbomber (36): deals 3 damage to FirstHero on First's turn -> Floating Watcher (11) gains +2/+2 (6/6)!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(36), Position: 0)).State;
        Assert.Equal(6, s.Entities[new(11)].Character!.Attack);
        Assert.Equal(6, s.Entities[new(11)].Character!.Health);

        // Hobgoblin (12) + playing 1-Attack Wisp (32) -> Wisp gets +2/+2 (3/3)!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(32), Position: 0)).State;
        Assert.Equal(3, s.Entities[new(32)].Character!.Attack);
        Assert.Equal(3, s.Entities[new(32)].Character!.Health);

        // Recombobulator (33) on Lil' Exorcist (30, 3-cost) -> transformed into another 3-cost minion!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(33), new(30), Position: 0)).State;
        var recombDef = CompiledCardData.Bind(s.Entities[new(30)].CardId, new()).Definition;
        Assert.Equal(3, recombDef.PrintedCost);
        Assert.Equal(EntityKind.Minion, recombDef.Kind);

        // Enhance-o Mechano (34) from a compact board -> grants each other friendly minion Windfury, Taunt, or Divine Shield
        var enhRoot = simulator.CreateGame(
        [
            Player(First, FirstHero) with { Mana = 20, ManaCrystals = 20, MaximumManaCrystals = 20, Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [34, 35])) },
            Player(Second, SecondHero)
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "GVG_044", First, 3, 3, 4), // Spider Tank
            Card(34, "GVG_107", First), // Enhance-o Mechano
            Card(35, "GVG_110", First)  // Dr. Boom
        ], First);
        var enhState = simulator.Apply(enhRoot, new(SimulationActionKind.PlayCard, First, new(34), Position: 0)).State;
        var tank = enhState.Entities[new(11)];
        Assert.True(
            tank.Character!.DivineShield ||
            tank.Enchantments.Any(e => e.GrantedTag is GAME_TAG.WINDFURY or GAME_TAG.TAUNT or GAME_TAG.DIVINE_SHIELD));

        // Dr. Boom (35) summons two Boom Bots (GVG_110t)
        enhState = simulator.Apply(enhState, new(SimulationActionKind.PlayCard, First, new(35), Position: 0)).State;
        Assert.Equal(2, enhState.Players[First].In(GameZone.Play).Count(id => enhState.Entities[id].CardId == "GVG_110t"));
    }

    [Fact]
    public void GvgPart2MimironMogorCleaveGazloweBlingtronDarkbombCallPetOilChoicesGiantWeeSootWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);

        // Test 1: Mimiron's Head -> V-07-TR-0N (Charge + MegaWindfury = 4 attacks!) & Mogor & Foe Reaper cleave & Wee Spellstopper
        GameState CreateMimironAndCleaveBoard(PlayerId active) => simulator.CreateGame(
        [
            Player(Second, SecondHero) with
            {
                Mana = 10, ManaCrystals = 10, MaximumManaCrystals = 10,
                Zones = Zones((GameZone.Play, [20, 21, 22, 23]), (GameZone.Deck, [60]))
            },
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones(
                    (GameZone.Play, [10, 11, 12, 13, 14, 15]),
                    (GameZone.Hand, [30, 31]),
                    (GameZone.Deck, [50, 51]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "GVG_111", First, 3, 4, 5), // Mimiron's Head (Mech 1)
            InPlay(12, "GVG_044", First, 4, 3, 4), // Spider Tank (Mech 2)
            InPlay(13, "GVG_058", First, 5, 2, 2), // Shielded Minibot (Mech 3)
            InPlay(14, "GVG_113", First, 6, 6, 9), // Foe Reaper 4000 (Cleave, Mech 4!)
            InPlay(15, "GVG_122", First, 7, 2, 5), // Wee Spellstopper
            InPlay(21, "CS2_182", Second, 8, 4, 10), // Left Yeti
            InPlay(22, "CS2_182", Second, 9, 4, 10), // Middle Yeti
            InPlay(23, "CS2_182", Second, 10, 4, 10), // Right Yeti
            Card(30, "GVG_112", First), // Mogor the Ogre
            Card(31, "GVG_117", First), // Gazlowe
            Card(50, "CS2_231", First),
            Card(51, "CS2_231", First),
            Card(60, "CS2_231", Second)
        ], active);

        var boardRoot = CreateMimironAndCleaveBoard(Second);
        var cleaveRoot = CreateMimironAndCleaveBoard(First);

        // End Second turn -> Start of First turn triggers Mimiron's Head (since First controls >= 3 Mechs): destroys Mechs & summons V-07-TR-0N (GVG_111t)!
        // Test Foe Reaper cleave BEFORE Mimiron destroys Mechs!
        var cleave = simulator.Apply(cleaveRoot, new(SimulationActionKind.Attack, First, new(14), new(22))).State;
        Assert.Equal(4, cleave.Entities[new(21)].Character!.Health); // 10 - 6 cleave
        Assert.Equal(4, cleave.Entities[new(22)].Character!.Health); // 10 - 6 target
        Assert.Equal(4, cleave.Entities[new(23)].Character!.Health); // 10 - 6 cleave

        // Wee Spellstopper (15) grants ELUSIVE to adjacent friendly minions (14):
        Assert.Contains(boardRoot.Entities[new(14)].AuraGrants, g => g.GrantedTag == GAME_TAG.ELUSIVE);

        // Mimiron's Head on EndTurn(Second -> First):
        var mimironState = simulator.Apply(boardRoot, new(SimulationActionKind.EndTurn, Second)).State;
        var voltron = Assert.Single(mimironState.Players[First].In(GameZone.Play), id => mimironState.Entities[id].CardId == "GVG_111t");
        // V-07-TR-0N has Charge and MegaWindfury (can attack 4 times in one turn -> 16 hero damage)!
        for (var hit = 0; hit < 4; hit++)
            mimironState = simulator.Apply(mimironState, new(SimulationActionKind.Attack, First, voltron, SecondHero)).State;
        Assert.Equal(14, mimironState.Entities[SecondHero].Character!.Health); // 30 - 16 = 14
        Assert.DoesNotContain(simulator.LegalActions(mimironState), a => a.Kind == SimulationActionKind.Attack && a.Source == voltron);

        // Test 2: Mogor wrong-enemy distribution, Gazlowe, Blingtron, Darkbomb, Call Pet, Oil, Anodized Robo Cub, Grove Tender, Clockwork Giant, Soot Spewer
        var spellRoot = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones(
                    (GameZone.Play, [10, 11, 12, 13]),
                    (GameZone.Hand, [30, 31, 32, 33, 34, 35, 36, 37, 38, 39]),
                    (GameZone.Deck, [50, 51, 52]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones(
                    (GameZone.Play, [20, 21]),
                    (GameZone.Hand, [60, 61, 62, 63]),
                    (GameZone.Deck, [70]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "GVG_112", First, 3, 7, 6), // Mogor the Ogre
            InPlay(12, "GVG_117", First, 4, 3, 6), // Gazlowe
            InPlay(13, "GVG_044", First, 5, 3, 4), // Friendly Spider Tank (Mech)
            InPlay(21, "CS2_231", Second, 6, 1, 1), // Enemy Wisp 1/1 (for Darkbomb kill -> Shadow draw)
            Card(30, "PART_001", First), // 1-cost spell -> triggers Gazlowe (adds random Mech)
            Card(31, "GVG_119", First), // Blingtron 3000 -> equips weapons for BOTH players
            Card(32, "GVG_022", First), // Tinker's Sharpsword Oil (+3 weapon atk + Combo +3 minion atk)
            Card(33, "GVG_015", First), // Darkbomb on Wisp -> kills Wisp -> draws Shadow spell (51)
            Card(34, "GVG_017", First), // Call Pet -> draws Beast Malorne (50) and discounts -4!
            Card(35, "GVG_030", First), // Anodized Robo Cub Choose One (+1 HP)
            Card(36, "GVG_032", First), // Grove Tender Choose One (GainFilled 1 mana crystal each player)
            Card(37, "GVG_121", First), // Clockwork Giant (12 - 4 opponent hand cards = 8!)
            Card(38, "GVG_123", First), // Soot Spewer (+1 SD; with Mech 13 on board -> adds Fire spell!)
            Card(39, "CS2_231", First),
            Card(50, "GVG_035", First), // Beast Malorne (9 mana -> 5 after Call Pet!)
            Card(51, "GVG_015", First), // Shadow spell Darkbomb for Darkbomb draw check
            Card(52, "CS2_231", First),
            Card(60, "CS2_231", Second),
            Card(61, "CS2_231", Second),
            Card(62, "CS2_231", Second),
            Card(63, "CS2_231", Second),
            Card(70, "CS2_231", Second)
        ], First);

        // Clockwork Giant (37): 12 - 4 opponent cards = 8 cost
        Assert.Equal(8, simulator.EffectiveCost(spellRoot, new(37)));

        // Mogor (11): Spider Tank (13) attacking SecondHero has >= 2 outcomes
        var mogorOutcomes = simulator.ApplyDistribution(spellRoot, new(SimulationActionKind.Attack, First, new(13), SecondHero));
        Assert.True(mogorOutcomes.Outcomes.Length >= 2);

        // Call Pet (34) draws top Beast Malorne (50) and reduces its cost by 4 (7 -> 3)
        var sp = simulator.Apply(spellRoot, new(SimulationActionKind.PlayCard, First, new(34))).State;
        Assert.Equal(GameZone.Hand, sp.LocationOf(new(50)).Zone);
        Assert.Equal(3, simulator.EffectiveCost(sp, new(50)));

        // Darkbomb (33) kills enemy Wisp (21) -> draws Shadow spell Darkbomb (51)!
        sp = simulator.Apply(sp, new(SimulationActionKind.PlayCard, First, new(33), new(21))).State;
        Assert.Equal(GameZone.Hand, sp.LocationOf(new(51)).Zone);

        // Soot Spewer (38) with friendly Mech Spider Tank (13) adds random Fire spell to hand!
        var handBeforeSoot = sp.Players[First].In(GameZone.Hand).Length;
        sp = simulator.Apply(sp, new(SimulationActionKind.PlayCard, First, new(38), Position: 0)).State;
        Assert.Equal(handBeforeSoot, sp.Players[First].In(GameZone.Hand).Length); // -1 played +1 Fire spell
        var fireSpellDef = CompiledCardData.Bind(sp.Entities[sp.Players[First].In(GameZone.Hand)[^1]].CardId, new()).Definition;
        Assert.Equal(SpellSchool.Fire, fireSpellDef.SpellSchool);

        // Gazlowe (12) + 1-cost spell PART_001 (30) -> adds random Mech to hand!
        var handBeforeGaz = sp.Players[First].In(GameZone.Hand).Length;
        sp = simulator.Apply(sp, new(SimulationActionKind.PlayCard, First, new(30), new(13))).State;
        Assert.Equal(handBeforeGaz, sp.Players[First].In(GameZone.Hand).Length); // -1 played +1 Mech
        var mechDef = CompiledCardData.Bind(sp.Entities[sp.Players[First].In(GameZone.Hand)[^1]].CardId, new()).Definition;
        Assert.Equal(MinionType.Mech, mechDef.MinionTypes & MinionType.Mech);

        // Blingtron 3000 (31): equips weapons for BOTH First and Second, then Tinker's Sharpsword Oil (32, Combo!) gives +3 weapon Atk!
        sp = simulator.Apply(sp, new(SimulationActionKind.PlayCard, First, new(31), Position: 0)).State;
        Assert.NotNull(sp.Players[First].EquippedWeapon);
        Assert.NotNull(sp.Players[Second].EquippedWeapon);
        var weaponAtkBefore = simulator.EffectiveAttack(sp, sp.Entities[FirstHero]);
        sp = simulator.Apply(sp, new(SimulationActionKind.PlayCard, First, new(32))).State;
        Assert.Equal(weaponAtkBefore + 3, simulator.EffectiveAttack(sp, sp.Entities[FirstHero]));

        // Anodized Robo Cub (35) Choose One +1 Health (GVG_030b -> 2/3)
        sp = simulator.Apply(sp, new(SimulationActionKind.PlayCard, First, new(35), Position: 0) { Choice = "GVG_030b" }).State;
        Assert.Equal(3, sp.Entities[new(35)].Character!.Health);

        // Grove Tender (36) Choose One: Gift of Mana (GVG_032a -> both players +1 ManaCrystal)
        var secondCrystalsBefore = sp.Players[Second].ManaCrystals;
        sp = simulator.Apply(sp, new(SimulationActionKind.PlayCard, First, new(36), Position: 0) { Choice = "GVG_032a" }).State;
        Assert.Equal(secondCrystalsBefore + 1, sp.Players[Second].ManaCrystals);
    }
}
