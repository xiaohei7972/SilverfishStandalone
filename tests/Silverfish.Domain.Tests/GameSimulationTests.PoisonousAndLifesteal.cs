using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    private static GameState WithHeroDamage(GameSimulator simulator, GameState state, int damage) =>
        simulator.CreateGame(state.Players.Values,
            state.Entities.Values.Select(entity => entity.Id == FirstHero
                ? entity with { Character = entity.Character! with { Damage = damage } }
                : entity),
            state.ActivePlayer, state.Turn, state.MinionDeaths, state.RandomState);

    [Fact]
    public void PoisonousMinionKillsLargeDefenderAfterBothCombatHits()
    {
        var simulator = new GameSimulator([
            Hero, Minion("venom", 1, 2) with { Keywords = CardKeywords.Poisonous },
            Minion("giant", 4, 8)
        ]);
        var first = Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) };
        var second = Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) };
        var root = Create(simulator, First, first, second,
            InPlay(11, "venom", First, 3, 1, 2),
            InPlay(21, "giant", Second, 4, 4, 8));

        var result = simulator.Apply(root,
            new(SimulationActionKind.Attack, First, new EntityId(11), new EntityId(21)));

        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new EntityId(11)).Zone);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new EntityId(21)).Zone);
        Assert.Equal(2, result.Events.Count(e => e.Kind == SimulationEventKind.Damage));
        Assert.Equal(2, result.Events.Count(e => e.Kind == SimulationEventKind.Died));
        Assert.Equal(7, result.State.Entities[new EntityId(21)].Character!.Health);
        Assert.Equal(GameZone.Play, root.LocationOf(new EntityId(21)).Zone);
    }

    [Fact]
    public void DivineShieldBlocksBothPoisonAndLifesteal()
    {
        var simulator = new GameSimulator([
            Hero, Minion("drainer", 3, 5) with { Keywords = CardKeywords.Poisonous | CardKeywords.Lifesteal },
            Minion("shielded", 0, 6) with { Keywords = CardKeywords.DivineShield }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "drainer", First, 3, 3, 5),
            InPlay(21, "shielded", Second, 4, 0, 6)
                with { Character = new(0, 6, DivineShield: true) });
        var damagedHero = WithHeroDamage(simulator, root, 7);

        var result = simulator.Apply(damagedHero,
            new(SimulationActionKind.Attack, First, new EntityId(11), new EntityId(21)));

        Assert.Equal(23, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(GameZone.Play, result.State.LocationOf(new EntityId(21)).Zone);
        Assert.False(result.State.Entities[new EntityId(21)].Character!.DivineShield);
        Assert.DoesNotContain(result.Events, e => e.Kind is SimulationEventKind.Healed or SimulationEventKind.Damage);
    }

    [Fact]
    public void LifestealSpellHealsOnlyForUnshieldedDamageAndTracksDamageBreakdown()
    {
        var simulator = new GameSimulator([
            Hero, Spell("drain") with
            {
                Keywords = CardKeywords.Lifesteal,
                OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 2)]
            },
            Minion("victim", 0, 5), Minion("shielded", 0, 5) with { Keywords = CardKeywords.DivineShield }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21, 22])) },
            Card(11, "drain", First),
            InPlay(21, "victim", Second, 3, 0, 5),
            InPlay(22, "shielded", Second, 4, 0, 5)
                with { Character = new(0, 5, DivineShield: true) });
        root = WithHeroDamage(simulator, root, 8);

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.Equal(24, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(3, result.State.Entities[new EntityId(21)].Character!.Health);
        Assert.Equal(5, result.State.Entities[new EntityId(22)].Character!.Health);
        var hit = Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Damage);
        Assert.Equal(new EntityId(21), hit.Target);
        Assert.Equal(0, hit.DamageToArmor);
        Assert.Equal(2, hit.DamageToHealth);
        Assert.Equal(2, Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Healed).Amount);
    }

    [Fact]
    public void PoisonousAreaSpellKillsOnlyMinionsThatTakeDamage()
    {
        var simulator = new GameSimulator([
            Hero, Spell("venom-cloud") with
            {
                Keywords = CardKeywords.Poisonous,
                OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 1)]
            },
            Minion("victim", 0, 8), Minion("shielded", 0, 8)
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21, 22])) },
            Card(11, "venom-cloud", First),
            InPlay(21, "victim", Second, 3, 0, 8),
            InPlay(22, "shielded", Second, 4, 0, 8)
                with { Character = new(0, 8, DivineShield: true) });

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new EntityId(21)).Zone);
        Assert.Equal(GameZone.Play, result.State.LocationOf(new EntityId(22)).Zone);
        Assert.False(result.State.Entities[new EntityId(22)].Character!.DivineShield);
    }

    [Fact]
    public void EquippedWeaponGrantsLifestealToAttackingHeroWithoutChangingDamageSource()
    {
        var weapon = new SimulationCard(new("vampire-blade", EntityKind.Weapon, 0,
            PrintedAttack: 3, PrintedDurability: 2)) { Keywords = CardKeywords.Lifesteal };
        var simulator = new GameSimulator([Hero, weapon]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                EquippedWeapon = new EntityId(11),
                Zones = Zones((GameZone.Play, [10, 11]))
            },
            Player(Second, SecondHero),
            Weapon(11, "vampire-blade", First, 3, 3, 2));
        root = WithHeroDamage(simulator, root, 5);

        var result = simulator.Apply(root, new(SimulationActionKind.Attack, First, FirstHero, SecondHero));

        Assert.Equal(28, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(27, result.State.Entities[SecondHero].Character!.Health);
        Assert.Equal(1, result.State.Entities[new EntityId(11)].Weapon!.Durability);
        Assert.Equal(FirstHero, Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Damage).Source);
        Assert.Equal(new EntityId(11), Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Healed).Source);
    }

    [Fact]
    public void PoisonousWeaponMarksTheDamagedMinionForDeath()
    {
        var weapon = new SimulationCard(new("poison-dagger", EntityKind.Weapon, 0,
            PrintedAttack: 1, PrintedDurability: 2)) { Keywords = CardKeywords.Poisonous };
        var simulator = new GameSimulator([Hero, weapon, Minion("wall", 0, 9)]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                EquippedWeapon = new EntityId(11),
                Zones = Zones((GameZone.Play, [10, 11]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Weapon(11, "poison-dagger", First, 3, 1, 2),
            InPlay(21, "wall", Second, 4, 0, 9));

        var result = simulator.Apply(root, new(SimulationActionKind.Attack, First, FirstHero, new EntityId(21)));

        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new EntityId(21)).Zone);
        Assert.Equal(1, result.State.Entities[new EntityId(11)].Weapon!.Durability);
    }

    [Fact]
    public void SilencedPoisonousLifestealMinionDealsOnlyNormalDamage()
    {
        var simulator = new GameSimulator([
            Hero, Minion("venom-vampire", 1, 4)
                with { Keywords = CardKeywords.Poisonous | CardKeywords.Lifesteal },
            Minion("wall", 0, 9)
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "venom-vampire", First, 3, 1, 4) with { Silenced = true },
            InPlay(21, "wall", Second, 4, 0, 9));
        root = WithHeroDamage(simulator, root, 4);

        var result = simulator.Apply(root,
            new(SimulationActionKind.Attack, First, new EntityId(11), new EntityId(21)));

        Assert.Equal(GameZone.Play, result.State.LocationOf(new EntityId(21)).Zone);
        Assert.Equal(8, result.State.Entities[new EntityId(21)].Character!.Health);
        Assert.Equal(26, result.State.Entities[FirstHero].Character!.Health);
        Assert.DoesNotContain(result.Events, e => e.Kind == SimulationEventKind.Healed);
    }
}
