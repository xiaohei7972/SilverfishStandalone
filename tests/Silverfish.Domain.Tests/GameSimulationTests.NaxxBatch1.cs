using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void NaxxBatch1DeathrattlesWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 20, ManaCrystals = 20, MaximumManaCrystals = 20,
                Zones = Zones((GameZone.Play, [10, 11, 12, 13, 14, 15, 16, 17]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Hand, []), (GameZone.Deck, [40, 41]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30) with { Character = new(0, 30, Damage: 10) }, // 20 HP
            InPlay(11, "FP1_001", First, 3, 2, 3), // Zombie Chow
            InPlay(12, "FP1_002", First, 4, 1, 2), // Haunted Creeper
            InPlay(13, "FP1_007", First, 5, 0, 2), // Nerubian Egg
            InPlay(14, "FP1_012", First, 6, 3, 6), // Sludge Belcher
            InPlay(15, "FP1_023", First, 7, 3, 4), // Dark Cultist
            InPlay(16, "FP1_024", First, 8, 1, 3), // Unstable Ghoul
            InPlay(17, "FP1_029", First, 9, 4, 4), // Dancing Swords
            InPlay(21, "CS2_231", Second, 10, 10, 30), // Giant enemy minion to trade into
            Card(40, "CS2_231", Second),
            Card(41, "CS2_231", Second)
        ], First);

        // Zombie Chow dies -> Enemy hero healed 20 -> 25
        var s = simulator.Apply(root, new(SimulationActionKind.Attack, First, new(11), new(21))).State;
        Assert.Equal(25, s.Entities[SecondHero].Character!.Health);

        // Haunted Creeper dies -> summons two 1/1 FP1_002t
        s = simulator.Apply(s, new(SimulationActionKind.Attack, First, new(12), new(21))).State;
        Assert.Equal(2, s.Players[First].In(GameZone.Play).Count(id => s.Entities[id].CardId == "FP1_002t"));

        // Unstable Ghoul dies -> deals 1 damage to all minions (kills both 1/1 FP1_002t, damages enemy 21)
        var enemyHpBeforeGhoul = s.Entities[new(21)].Character!.Health;
        s = simulator.Apply(s, new(SimulationActionKind.Attack, First, new(16), new(21))).State;
        Assert.Equal(0, s.Players[First].In(GameZone.Play).Count(id => s.Entities[id].CardId == "FP1_002t"));
        Assert.Equal(enemyHpBeforeGhoul - 2, s.Entities[new(21)].Character!.Health); // 1 attack + 1 deathrattle

        // Sludge Belcher dies -> summons 1/2 Taunt FP1_012t
        s = simulator.Apply(s, new(SimulationActionKind.Attack, First, new(14), new(21))).State;
        var slimeId = Assert.Single(s.Players[First].In(GameZone.Play), id => s.Entities[id].CardId == "FP1_012t");
        Assert.Equal(1, s.Entities[slimeId].Character!.Attack);
        Assert.Equal(2, s.Entities[slimeId].Character!.Health);

        // Dancing Swords dies -> Opponent draws 1 card
        var enemyHandBefore = s.Players[Second].In(GameZone.Hand).Length;
        s = simulator.Apply(s, new(SimulationActionKind.Attack, First, new(17), new(21))).State;
        Assert.Equal(enemyHandBefore + 1, s.Players[Second].In(GameZone.Hand).Length);

        // Dark Cultist dies -> gives a random other friendly minion +3 Health
        var totalFriendlyHpBefore = s.Players[First].In(GameZone.Play)
            .Where(id => id != FirstHero && id != new EntityId(15))
            .Sum(id => s.Entities[id].Character!.MaximumHealth);
        s = simulator.Apply(s, new(SimulationActionKind.Attack, First, new(15), new(21))).State;
        var totalFriendlyHpAfter = s.Players[First].In(GameZone.Play)
            .Where(id => id != FirstHero)
            .Sum(id => s.Entities[id].Character!.MaximumHealth);
        Assert.Equal(totalFriendlyHpBefore + 3, totalFriendlyHpAfter);

        // Nerubian Egg dies via spell -> summons 4/4 FP1_007t
        var eggTest = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 10, ManaCrystals = 10,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20])) }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "FP1_007", First, 3, 0, 2),
            Card(12, "CS2_029", First) // Fireball
        ], First);
        var afterEgg = simulator.Apply(eggTest, new(SimulationActionKind.PlayCard, First, new(12), new(11))).State;
        var nerubianId = Assert.Single(afterEgg.Players[First].In(GameZone.Play), id => afterEgg.Entities[id].CardId == "FP1_007t");
        Assert.Equal(4, afterEgg.Entities[nerubianId].Character!.Attack);
        Assert.Equal(4, afterEgg.Entities[nerubianId].Character!.Health);
    }

    [Fact]
    public void NaxxBatch1TriggersSecretLoathebAndKeywordsWork()
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero]);
        var root = simulator.CreateGame(
        [
            Player(First, FirstHero) with
            {
                Mana = 20, ManaCrystals = 20, MaximumManaCrystals = 20,
                Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13, 14, 15, 16]), (GameZone.Deck, [30, 31]))
            },
            Player(Second, SecondHero) with
            {
                Mana = 10, ManaCrystals = 10, MaximumManaCrystals = 10,
                Zones = Zones((GameZone.Play, [20, 21]), (GameZone.Hand, [22]), (GameZone.Deck, [40, 41]))
            }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "FP1_005", First, 3, 2, 2), // Shade of Naxxramas
            InPlay(12, "FP1_027", First, 4, 1, 4) with { Character = new(1, 4, Damage: 3) }, // Damaged Stoneskin Gargoyle (1/1)
            Card(13, "FP1_020", First), // Avenge
            Card(14, "FP1_030", First), // Loatheb
            Card(15, "FP1_008", First), // Spectral Knight
            Card(16, "FP1_010", First), // Maexxna
            InPlay(21, "CS2_231", Second, 5, 5, 5), // Enemy attacker
            Card(22, "CS2_029", Second), // Enemy Fireball (base cost 4)
            Card(30, "CS2_231", First),
            Card(31, "CS2_231", First),
            Card(40, "CS2_231", Second),
            Card(41, "CS2_231", Second)
        ], First);

        // Play Avenge + Loatheb + Spectral Knight + Maexxna
        var s = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new(13))).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(14), Position: 0)).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(15), Position: 0)).State;
        s = simulator.Apply(s, new(SimulationActionKind.PlayCard, First, new(16), Position: 0)).State;
        Assert.Equal(GameZone.Secret, s.LocationOf(new(13)).Zone);

        // End First's turn -> Second's turn: Fireball costs 4 + 5 = 9!
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(9, simulator.EffectiveCost(s, new(22)));

        // Second attacks and kills Loatheb (14, 5/5) -> Avenge triggers and buffs one surviving friendly minion by +3/+2
        var totalAttackBefore = s.Players[First].In(GameZone.Play)
            .Where(id => id != FirstHero && id != new EntityId(14))
            .Sum(id => s.Entities[id].Character!.Attack);
        s = simulator.Apply(s, new(SimulationActionKind.Attack, Second, new(21), new(14))).State;
        Assert.Equal(GameZone.Graveyard, s.LocationOf(new(13)).Zone); // Avenge revealed
        var totalAttackAfter = s.Players[First].In(GameZone.Play)
            .Where(id => id != FirstHero)
            .Sum(id => s.Entities[id].Character!.Attack);
        Assert.Equal(totalAttackBefore + 3, totalAttackAfter);

        // End Second's turn -> First's turn starts: Stoneskin Gargoyle heals to full Health!
        s = simulator.Apply(s, new(SimulationActionKind.EndTurn, Second)).State;
        Assert.Equal(0, s.Entities[new(12)].Character!.Damage);

        // Verify Shade of Naxxramas start-of-turn +1/+1 trigger separately
        var shadeRoot = simulator.CreateGame(
        [
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Deck, [30])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Deck, [40])) }
        ],
        [
            InPlay(10, "hero", First, 1, 0, 30),
            InPlay(20, "hero", Second, 2, 0, 30),
            InPlay(11, "FP1_005", First, 3, 2, 2),
            Card(30, "CS2_231", First),
            Card(40, "CS2_231", Second)
        ], Second);
        var shadeTurn = simulator.Apply(shadeRoot, new(SimulationActionKind.EndTurn, Second)).State;
        Assert.Equal(3, shadeTurn.Entities[new(11)].Character!.Attack);
        Assert.Equal(3, shadeTurn.Entities[new(11)].Character!.Health);
    }
}
