using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void PintSizedSummonerAndSeaGiantAdjustMinionCostsDynamically()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 10, ManaCrystals = 10, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12, 13])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21, 22])) },
            Card(11, "EX1_076", First), Card(12, "EX1_023", First), Card(13, "EX1_586", First),
            InPlay(21, "CS2_231", Second, 3, 1, 1), InPlay(22, "CS2_231", Second, 4, 1, 1));

        Assert.Equal(8, simulator.EffectiveCost(root, new(13)));
        var playedSummoner = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(11), Position: 0)).State;
        Assert.Equal(4, simulator.EffectiveCost(playedSummoner, new(12)));
        Assert.Equal(7, simulator.EffectiveCost(playedSummoner, new(13)));

        var nextTurn = simulator.Apply(
            simulator.Apply(playedSummoner, new(SimulationActionKind.EndTurn, First)).State,
            new(SimulationActionKind.EndTurn, Second)).State;
        Assert.Equal(3, simulator.EffectiveCost(nextTurn, new(12)));
        Assert.Equal(6, simulator.EffectiveCost(nextTurn, new(13)));

        var afterFirstMinion = simulator.Apply(nextTurn, new(SimulationActionKind.PlayCard, First, new(12), Position: 1)).State;
        Assert.Equal(6, simulator.EffectiveCost(afterFirstMinion, new(13)));
    }

    [Fact]
    public void TaurenWarriorAndSpitefulSmithActivateWhileDamagedAndStopWhenHealedOrSilenced()
    {
        var healSpell = Spell("test-heal") with
        {
            Target = TargetRequirement.Character,
            OnPlay = [new HealEffect(HealTarget.Selected, 2)]
        };
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, healSpell]);
        var tauren = new EntityId(11);
        var smith = new EntityId(12);
        var weapon = new EntityId(13);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 10, ManaCrystals = 10, EquippedWeapon = weapon,
                Zones = Zones((GameZone.Play, [10, 11, 12, 13]), (GameZone.Hand, [14, 15, 16]))
            },
            Player(Second, SecondHero),
            InPlay(11, "EX1_390", First, 3, 2, 3),
            InPlay(12, "CS2_221", First, 4, 4, 6),
            Card(13, "CS2_106", First) with { PlayOrder = 5, Weapon = new(3, 2, 2) },
            Card(14, "EX1_607", First), Card(15, "EX1_607", First),
            Card(16, healSpell.Definition.CardId, First));

        Assert.Equal(2, root.Entities[tauren].Character!.Attack);
        Assert.Equal(3, simulator.EffectiveAttack(root, root.Entities[FirstHero]));

        var damagedTauren = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(14), tauren)).State;
        Assert.Equal(7, damagedTauren.Entities[tauren].Character!.Attack);

        var damagedSmith = simulator.Apply(damagedTauren, new(SimulationActionKind.PlayCard, First, new(15), smith)).State;
        Assert.Equal(5, simulator.EffectiveWeaponAttack(damagedSmith, damagedSmith.Entities[weapon]));
        Assert.Equal(5, simulator.EffectiveAttack(damagedSmith, damagedSmith.Entities[FirstHero]));

        var healedSmith = simulator.Apply(damagedSmith, new(SimulationActionKind.PlayCard, First, new(16), smith)).State;
        Assert.Equal(3, simulator.EffectiveWeaponAttack(healedSmith, healedSmith.Entities[weapon]));
    }

    [Fact]
    public void StealthTokensChooseOneAndSimpleBattlecriesWorkAcrossBatch1Cards()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 10, ManaCrystals = 10,
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12, 13, 14, 15, 16]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21, 22]), (GameZone.Deck, [23])) },
            Card(11, "EX1_028", First), Card(12, "CS2_161", First), Card(13, "EX1_067", First),
            Card(14, "CS2_151", First), Card(15, "EX1_248", First), Card(16, "NEW1_007", First),
            InPlay(21, "EX1_096", Second, 3, 2, 1), InPlay(22, "EX1_055", Second, 4, 1, 3), Card(23, "CS2_231", Second));

        var s1 = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(11), Position: 0)).State;
        var s2 = simulator.Apply(s1, new(SimulationActionKind.PlayCard, First, new(15)) { Choice = null }).State;
        Assert.Equal(1, s2.Players[First].OverloadOwed);
        Assert.Equal(2, s2.Players[First].In(GameZone.Play).Count(id => s2.Entities[id].CardId == "EX1_tk11"));

        var enemyTurn = simulator.Apply(s2, new(SimulationActionKind.EndTurn, First)).State;
        Assert.DoesNotContain(simulator.LegalActions(enemyTurn),
            a => a.Kind == SimulationActionKind.Attack && a.Target == new EntityId(11));
        var turn2 = simulator.Apply(enemyTurn, new(SimulationActionKind.EndTurn, Second)).State;

        var starfallAoe = simulator.Apply(turn2, new(SimulationActionKind.PlayCard, First, new(16)) { Choice = "aoe" });
        Assert.Equal(GameZone.Graveyard, starfallAoe.State.LocationOf(new(21)).Zone);
        Assert.Single(starfallAoe.State.Players[Second].In(GameZone.Hand));
        Assert.Equal(1, starfallAoe.State.Entities[new(22)].Character!.Health);

        var knight = simulator.Apply(turn2, new(SimulationActionKind.PlayCard, First, new(14), Position: 0)).State;
        Assert.Contains(knight.Players[First].In(GameZone.Play), id => knight.Entities[id].CardId == "CS2_152");
    }

    [Fact]
    public void SecretsSecretkeeperDoomsayerAndMindgamesResolveCorrectly()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var keeper = new EntityId(11);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 10, ManaCrystals = 10,
                Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13, 14, 15, 16]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 10, ManaCrystals = 10,
                Zones = Zones((GameZone.Play, [20]), (GameZone.Hand, [21, 22]), (GameZone.Deck, [23]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30) with { Character = new(0, 30, Damage: 28) },
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "EX1_080", First, 3, 1, 2),
            InPlay(12, "EX1_023", First, 4, 3, 3) with { Character = new(3, 3, Damage: 2) },
            Card(13, "EX1_136", First), Card(14, "EX1_295", First),
            Card(15, "EX1_294", First), Card(16, "EX1_379", First),
            Card(21, "EX1_067", Second), Card(22, "CS2_029", Second), Card(23, "EX1_586", Second)
        ], First);

        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(13))).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14))).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15))).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16))).State;
        Assert.Equal(5, s.Entities[keeper].Character!.Attack);
        Assert.Equal(6, s.Entities[keeper].Character!.Health);

        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;

        var enemyPlayed = simulator.Apply(s, new(SimulationActionKind.PlayCard, Second, new(21), Position: 0)).State;
        Assert.Equal(1, enemyPlayed.Entities[new(21)].Character!.Health);
        var mirrored = Assert.Single(enemyPlayed.Players[First].In(GameZone.Play), id => id != FirstHero && id != keeper && id != new EntityId(12));
        Assert.Equal("EX1_067", enemyPlayed.Entities[mirrored].CardId);

        var killedFriendly = simulator.Apply(enemyPlayed, new(SimulationActionKind.Attack, Second, new(21), new(12))).State;
        var redeemed = Assert.Single(killedFriendly.Players[First].In(GameZone.Play),
            id => id != FirstHero && id != keeper && id != mirrored);
        Assert.Equal("EX1_023", killedFriendly.Entities[redeemed].CardId);
        Assert.Equal(1, killedFriendly.Entities[redeemed].Character!.Health);
        Assert.True(killedFriendly.Entities[redeemed].Character!.DivineShield);

        var blockedLethal = simulator.Apply(killedFriendly, new(SimulationActionKind.PlayCard, Second, new(22), FirstHero));
        Assert.Contains(blockedLethal.Events, e => e.Kind == SimulationEventKind.SecretRevealed);
        Assert.Equal(2, blockedLethal.State.Entities[FirstHero].Character!.Health);

        var mindgamesRoot = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 10, ManaCrystals = 10, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Deck, [21])) },
            Card(11, "EX1_345", First), Card(12, "NEW1_021", First), Card(21, "CS2_029", Second));
        var whiffed = simulator.Apply(mindgamesRoot, new(SimulationActionKind.PlayCard, First, new(11))).State;
        Assert.Contains(whiffed.Players[First].In(GameZone.Play), id => whiffed.Entities[id].CardId == "EX1_345t");
        var withDoomsayer = simulator.Apply(whiffed, new(SimulationActionKind.PlayCard, First, new(12), Position: 0)).State;
        var afterRound = simulator.Apply(simulator.Apply(withDoomsayer, new(SimulationActionKind.EndTurn, First)).State,
            new(SimulationActionKind.EndTurn, Second)).State;
        Assert.Equal(new[] { FirstHero }, afterRound.Players[First].In(GameZone.Play));
    }

    [Fact]
    public void ComboControlBetrayalShadowflameIceLanceAndAuchenaiResolveCorrectly()
    {
        var healSpell = Spell("auchenai-heal") with
        {
            Target = TargetRequirement.Character,
            OnPlay = [new HealEffect(HealTarget.Selected, 3)]
        };
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, healSpell]);

        // Ice Lance + Cold Blood + Defias Ringleader Combo
        var comboRoot = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 10, ManaCrystals = 10,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12, 13, 14, 15]))
            },
            Player(Second, SecondHero),
            InPlay(11, "CS2_231", First, 3, 1, 1),
            Card(12, "CS2_031", First), Card(13, "CS2_031", First),
            Card(14, "CS2_073", First), Card(15, "EX1_131", First));
        var frozen = simulator.Apply(comboRoot, new(SimulationActionKind.PlayCard, First, new(12), SecondHero)).State;
        Assert.True(frozen.Entities[SecondHero].Character!.Frozen);
        Assert.Equal(30, frozen.Entities[SecondHero].Character!.Health);
        var shattered = simulator.Apply(frozen, new(SimulationActionKind.PlayCard, First, new(13), SecondHero)).State;
        Assert.Equal(26, shattered.Entities[SecondHero].Character!.Health);
        var coldBlooded = simulator.Apply(shattered, new(SimulationActionKind.PlayCard, First, new(14), new(11))).State;
        Assert.Equal(5, coldBlooded.Entities[new(11)].Character!.Attack);
        var ringleader = simulator.Apply(coldBlooded, new(SimulationActionKind.PlayCard, First, new(15), Position: 1)).State;
        Assert.Contains(ringleader.Players[First].In(GameZone.Play), id => ringleader.Entities[id].CardId == "EX1_131t");

        // Shadow Madness + Brewmaster + Cabal + Betrayal + Shadowflame + Mark of Nature + Abusive + Naturalize + Auchenai
        var boardRoot = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 30, ManaCrystals = 30, MaximumManaCrystals = 30,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12, 13, 14, 15, 16, 17, 18, 19]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21, 22, 23]), (GameZone.Deck, [24, 25]))
            },
            InPlay(11, "EX1_591", First, 3, 3, 5),
            Card(12, healSpell.Definition.CardId, First),
            Card(13, "EX1_126", First),
            Card(14, "EX1_334", First),
            Card(15, "CS2_188", First),
            Card(16, "EX1_155", First),
            Card(17, "EX1_303", First),
            Card(18, "EX1_161", First),
            Card(19, "EX1_057", First),
            InPlay(21, "CS2_231", Second, 4, 1, 4),
            InPlay(22, "CS2_231", Second, 5, 3, 4),
            InPlay(23, "CS2_231", Second, 6, 2, 5),
            Card(24, "CS2_231", Second), Card(25, "CS2_231", Second));

        // Auchenai turns 3 heal into 3 damage on 23 (5 -> 2)
        var s = simulator.Apply(boardRoot, new(SimulationActionKind.PlayCard, First, new(12), new(23))).State;
        Assert.Equal(2, s.Entities[new(23)].Character!.Health);

        // Betrayal on 22 (3 atk) kills 23 (2 hp) and damages 21 (4 -> 1)
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(13), new(22))).State;
        Assert.Equal(1, s.Entities[new(21)].Character!.Health);
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(23)).Zone);

        // Shadow Madness steals 22 (3/4), ready to attack!
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14), new(22))).State;
        Assert.Equal(First, s.Entities[new(22)].Controller);
        Assert.Contains(simulator.LegalActions(s), a => a.Kind == SimulationActionKind.Attack && a.Source == new EntityId(22));

        // End turn returns 22 to Second
        var returned = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(Second, returned.Entities[new(22)].Controller);

        // Mark of Nature (+4 Health & Taunt) + Abusive Sergeant (+2 Attack) + Shadowflame + Naturalize + Brewmaster + Cabal
        var s2 = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16), new(22)) { Choice = "healthAndTaunt" }).State;
        Assert.Equal(8, s2.Entities[new(22)].Character!.Health);
        Assert.True(simulator.Cards["CS2_231"].Keywords == CardKeywords.None && s2.Entities[new(22)].Enchantments.Any(e => e.GrantedTag == GAME_TAG.TAUNT));
        s2 = simulator.Apply(s2, new(SimulationActionKind.PlayCard, First, new(15), new(22), Position: 0)).State;
        Assert.Equal(5, s2.Entities[new(22)].Character!.Attack);
        s2 = simulator.Apply(s2, new(SimulationActionKind.PlayCard, First, new(17), new(22))).State;
        Assert.Equal(GameZone.Graveyard, s2.LocationOf(new(22)).Zone);
        Assert.Equal(GameZone.Graveyard, s2.LocationOf(new(21)).Zone);

        // Brewmaster returns Abusive Sergeant (15) to hand; Naturalize kills Auchenai (11) and draws 2 for Second
        s2 = simulator.Apply(s2, new(SimulationActionKind.PlayCard, First, new(19), new(15), Position: 0)).State;
        Assert.Equal(GameZone.Hand, s2.LocationOf(new(15)).Zone);
        s2 = simulator.Apply(s2, new(SimulationActionKind.PlayCard, First, new(18), new(11))).State;
        Assert.Equal(2, s2.Players[Second].In(GameZone.Hand).Length);

        // Cabal Shadow Priest steals <=2 attack enemy minion
        var cabalRoot = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 6, ManaCrystals = 6, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "EX1_091", First), InPlay(21, "CS2_231", Second, 3, 2, 2));
        var cabaled = simulator.Apply(cabalRoot, new(SimulationActionKind.PlayCard, First, new(11), new(21), Position: 0)).State;
        Assert.Equal(First, cabaled.Entities[new(21)].Controller);
    }
}
