using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    private static CardTrigger KillBonus(TriggerTiming timing, int armor) =>
        new(timing, TriggerRelation.Friendly, [new GainArmorEffect(armor)])
        {
            Focus = TriggerFocus.DamageSource,
            EntityMatch = TriggerEntityMatch.Self
        };

    [Theory]
    [InlineData(3, true, false, 3)]
    [InlineData(4, false, true, 7)]
    [InlineData(5, false, false, 0)]
    public void DistinguishesOverkillAndExactHonorableKill(int remainingHealth,
        bool overkill, bool honorable, int expectedArmor)
    {
        var simulator = new GameSimulator([
            Hero, Minion("finisher", 4, 5) with
            {
                Triggers = [KillBonus(TriggerTiming.Overkill, 3), KillBonus(TriggerTiming.HonorableKill, 7)]
            },
            Minion("defender", 0, 8)
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "finisher", First, 3, 4, 5),
            InPlay(21, "defender", Second, 4, 0, 8) with
            {
                Character = new(0, 8, Damage: 8 - remainingHealth)
            });

        var result = simulator.Apply(root, new(SimulationActionKind.Attack, First,
            new EntityId(11), new EntityId(21)));

        var hit = Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Damage &&
            e.Source == new EntityId(11));
        Assert.Equal(overkill, hit.IsOverkill);
        Assert.Equal(honorable, hit.IsHonorableKill);
        Assert.Equal(expectedArmor, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(overkill, result.Events.Any(e => e.Kind == SimulationEventKind.Triggered &&
            e.Timing == TriggerTiming.Overkill));
        Assert.Equal(honorable, result.Events.Any(e => e.Kind == SimulationEventKind.Triggered &&
            e.Timing == TriggerTiming.HonorableKill));
        Assert.Equal(remainingHealth <= 4 ? GameZone.Graveyard : GameZone.Play,
            result.State.LocationOf(new EntityId(21)).Zone);
        Assert.Equal(GameZone.Play, root.LocationOf(new EntityId(21)).Zone);
    }

    [Fact]
    public void ArmorAbsorptionCannotTriggerExactLethalByComparingRawAttack()
    {
        var simulator = new GameSimulator([
            Hero, Minion("attacker", 4, 5) with
            {
                Triggers = [KillBonus(TriggerTiming.Overkill, 3), KillBonus(TriggerTiming.HonorableKill, 7)]
            }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero),
            InPlay(11, "attacker", First, 3, 4, 5));
        root = simulator.CreateGame(root.Players.Values,
            root.Entities.Values.Select(entity => entity.Id == SecondHero
                ? entity with { Character = new(0, 30, Damage: 27, Armor: 2) }
                : entity), root.ActivePlayer, root.Turn, root.MinionDeaths, root.RandomState);

        var result = simulator.Apply(root,
            new(SimulationActionKind.Attack, First, new EntityId(11), SecondHero));

        var hit = Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Damage &&
            e.Source == new EntityId(11));
        Assert.Equal(2, hit.DamageToArmor);
        Assert.Equal(2, hit.DamageToHealth);
        Assert.False(hit.IsHonorableKill);
        Assert.False(hit.IsOverkill);
        Assert.Equal(1, result.State.Entities[SecondHero].Character!.Health);
        Assert.Equal(0, result.State.Entities[FirstHero].Character!.Armor);
    }

    [Fact]
    public void DivineShieldBlocksLethalBonusesAndDamageEvents()
    {
        var simulator = new GameSimulator([
            Hero, Minion("attacker", 4, 5) with
            {
                Triggers = [KillBonus(TriggerTiming.Overkill, 3), KillBonus(TriggerTiming.HonorableKill, 7)]
            },
            Minion("shield", 0, 2)
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "attacker", First, 3, 4, 5),
            InPlay(21, "shield", Second, 4, 0, 2) with
            {
                Character = new(0, 2, DivineShield: true)
            });

        var result = simulator.Apply(root, new(SimulationActionKind.Attack, First,
            new EntityId(11), new EntityId(21)));

        Assert.Contains(result.Events, e => e.Kind == SimulationEventKind.ShieldBroken);
        Assert.DoesNotContain(result.Events, e => e.Kind is SimulationEventKind.Triggered or SimulationEventKind.Damage);
        Assert.Equal(0, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(GameZone.Play, result.State.LocationOf(new EntityId(21)).Zone);
    }

    [Fact]
    public void DefenderDealingExactLethalOnOpponentsTurnGetsNoBonus()
    {
        var simulator = new GameSimulator([
            Hero, Minion("attacker", 1, 5),
            Minion("counter", 5, 8) with
            {
                Triggers = [KillBonus(TriggerTiming.HonorableKill, 7)]
            }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "attacker", First, 3, 1, 5),
            InPlay(21, "counter", Second, 4, 5, 8));

        var result = simulator.Apply(root, new(SimulationActionKind.Attack, First,
            new EntityId(11), new EntityId(21)));

        var retaliation = Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Damage &&
            e.Source == new EntityId(21));
        Assert.False(retaliation.IsHonorableKill);
        Assert.Equal(0, result.State.Entities[SecondHero].Character!.Armor);
        Assert.DoesNotContain(result.Events, e => e.Timing == TriggerTiming.HonorableKill);
    }

    [Fact]
    public void SpellWithOverkillTriggerResolvesBeforeSpellLeavesPlay()
    {
        var simulator = new GameSimulator([
            Hero, Minion("victim", 0, 2),
            Spell("blast") with
            {
                Target = TargetRequirement.EnemyMinion,
                OnPlay = [new DamageEffect(DamageTarget.Selected, 4)],
                Triggers = [KillBonus(TriggerTiming.Overkill, 5)]
            }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "blast", First), InPlay(21, "victim", Second, 3, 0, 2));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard,
            First, new EntityId(11), new EntityId(21)));

        Assert.Equal(5, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Contains(result.Events, e => e.Kind == SimulationEventKind.Triggered &&
            e.Timing == TriggerTiming.Overkill && e.Source == new EntityId(11));
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new EntityId(11)).Zone);
    }

    [Fact]
    public void HonorableKillFromWeaponGetsBonusWhileDamageSourceStaysHero()
    {
        var weapon = new SimulationCard(new("honorable-weapon", EntityKind.Weapon, 0,
            PrintedAttack: 4, PrintedDurability: 2))
        {
            Triggers = [KillBonus(TriggerTiming.HonorableKill, 6)]
        };
        var simulator = new GameSimulator([Hero, weapon, Minion("victim", 0, 4)]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                EquippedWeapon = new EntityId(11),
                Zones = Zones((GameZone.Play, [10, 11]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Weapon(11, "honorable-weapon", First, 3, 4, 2),
            InPlay(21, "victim", Second, 4, 0, 4));

        var result = simulator.Apply(root,
            new(SimulationActionKind.Attack, First, FirstHero, new EntityId(21)));

        Assert.Equal(6, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(1, result.State.Entities[new EntityId(11)].Weapon!.Durability);
        Assert.Equal(FirstHero, Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Damage).Source);
        Assert.Contains(result.Events, e => e.Kind == SimulationEventKind.Triggered &&
            e.Timing == TriggerTiming.HonorableKill && e.Source == new EntityId(11));
    }

    [Fact]
    public void OverkillTriggersBeforeAttackersDeathrattle()
    {
        var simulator = new GameSimulator([
            Hero, Minion("glass-cannon", 4, 1) with
            {
                Triggers = [KillBonus(TriggerTiming.Overkill, 2)],
                Deathrattle = [new GainArmorEffect(1)]
            },
            Minion("victim", 1, 3)
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "glass-cannon", First, 3, 4, 1),
            InPlay(21, "victim", Second, 4, 1, 3));

        var result = simulator.Apply(root,
            new(SimulationActionKind.Attack, First, new EntityId(11), new EntityId(21)));

        Assert.Equal(3, result.State.Entities[FirstHero].Character!.Armor);
        var events = result.Events.ToArray();
        var overkillIndex = Array.FindIndex(events, e => e.Kind == SimulationEventKind.Triggered &&
            e.Timing == TriggerTiming.Overkill);
        var deathIndex = Array.FindIndex(events, e => e.Kind == SimulationEventKind.Died);
        Assert.True(overkillIndex >= 0 && overkillIndex < deathIndex);
    }

    [Fact]
    public void InvalidLethalBonusConfigurationIsRejected()
    {
        var wrongFocus = Minion("wrong", 1, 1) with
        {
            Triggers = [new(TriggerTiming.Overkill, TriggerRelation.Friendly, [new GainArmorEffect(1)])]
        };
        var wrongMatch = Minion("wrong-match", 1, 1) with
        {
            Triggers = [new(TriggerTiming.HonorableKill, TriggerRelation.Friendly, [new GainArmorEffect(1)])
            {
                Focus = TriggerFocus.DamageSource
            }]
        };
        Assert.Throws<ArgumentException>(() => new GameSimulator([Hero, wrongFocus]));
        Assert.Throws<ArgumentException>(() => new GameSimulator([Hero, wrongMatch]));
    }
}
