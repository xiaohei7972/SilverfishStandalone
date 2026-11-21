using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    private static SimulationCard HealerMinion(string id, int armor = 3) =>
        Minion(id, 1, 5) with
        {
            Keywords = CardKeywords.Overheal,
            Triggers =
            [
                new(TriggerTiming.Overheal, TriggerRelation.Friendly, [new GainArmorEffect(armor)])
                {
                    EntityMatch = TriggerEntityMatch.Self
                }
            ]
        };

    [Fact]
    public void HealingExactlyToFullDoesNotActivateOverheal()
    {
        var simulator = new GameSimulator([
            Hero, HealerMinion("overheal"),
            Spell("restore") with
            {
                Target = TargetRequirement.FriendlyMinion,
                OnPlay = [new HealEffect(HealTarget.Selected, 2)]
            }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero),
            InPlay(12, "overheal", First, 3, 1, 5) with { Character = new(1, 5, Damage: 2) },
            Card(11, "restore", First));

        var result = simulator.Apply(root,
            new(SimulationActionKind.PlayCard, First, new EntityId(11), new EntityId(12)));

        Assert.Equal(5, result.State.Entities[new EntityId(12)].Character!.Health);
        Assert.Equal(2, Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Healed).Amount);
        Assert.DoesNotContain(result.Events, e => e.Kind is SimulationEventKind.Overhealed or SimulationEventKind.Triggered);
        Assert.Equal(0, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(3, root.Entities[new EntityId(12)].Character!.Health);
    }

    [Fact]
    public void PositiveHealingOfAnAlreadyFullOverhealMinionTriggersEveryTime()
    {
        var simulator = new GameSimulator([
            Hero, HealerMinion("overheal"),
            Spell("restore") with { OnPlay = [new HealEffect(HealTarget.FriendlyMinions, 2)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11, 13])) },
            Player(Second, SecondHero),
            InPlay(12, "overheal", First, 3, 1, 5),
            Card(11, "restore", First), Card(13, "restore", First));

        var first = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));
        var second = simulator.Apply(first.State, new(SimulationActionKind.PlayCard, First, new EntityId(13)));

        Assert.Equal(6, second.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(5, second.State.Entities[new EntityId(12)].Character!.Health);
        Assert.DoesNotContain(first.Events, e => e.Kind == SimulationEventKind.Healed);
        Assert.Equal(2, Assert.Single(first.Events, e => e.Kind == SimulationEventKind.Overhealed).Amount);
        Assert.Single(first.Events, e => e.Kind == SimulationEventKind.Triggered &&
            e.Timing == TriggerTiming.Overheal);
        Assert.Single(second.Events, e => e.Kind == SimulationEventKind.Triggered &&
            e.Timing == TriggerTiming.Overheal);
    }

    [Fact]
    public void HealingBeyondFullReportsRestoredAndExcessSeparately()
    {
        var simulator = new GameSimulator([
            Hero, HealerMinion("overheal"),
            Spell("restore") with
            {
                Target = TargetRequirement.FriendlyMinion,
                OnPlay = [new HealEffect(HealTarget.Selected, 4)]
            }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero),
            InPlay(12, "overheal", First, 3, 1, 5) with { Character = new(1, 5, Damage: 1) },
            Card(11, "restore", First));

        var result = simulator.Apply(root,
            new(SimulationActionKind.PlayCard, First, new EntityId(11), new EntityId(12)));

        var healed = Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Healed);
        var over = Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Overhealed);
        Assert.Equal(1, healed.Amount);
        Assert.Equal(3, over.Amount);
        Assert.Equal(3, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(5, result.State.Entities[new EntityId(12)].Character!.Health);
        Assert.True(result.Events.ToList().IndexOf(healed) < result.Events.ToList().IndexOf(over));
        Assert.Contains(result.Events, e => e.Kind == SimulationEventKind.Triggered &&
            e.Target == new EntityId(12));
    }

    [Fact]
    public void AreaHealingOnlyActivatesEligibleMinionsAndSettlesEveryHealBeforeRewards()
    {
        var simulator = new GameSimulator([
            Hero, HealerMinion("overheal"),
            Minion("normal", 0, 5),
            Spell("area") with { OnPlay = [new HealEffect(HealTarget.AllMinions, 3)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12, 13]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(12, "overheal", First, 3, 1, 5),
            InPlay(13, "normal", First, 4, 0, 5) with { Character = new(0, 5, Damage: 2) },
            InPlay(21, "overheal", Second, 5, 1, 5) with { Character = new(1, 5, Damage: 3) },
            Card(11, "area", First));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));
        Assert.Equal(5, result.State.Entities[new EntityId(13)].Character!.Health);
        Assert.Equal(5, result.State.Entities[new EntityId(21)].Character!.Health);
        Assert.Equal(3, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(0, result.State.Entities[SecondHero].Character!.Armor);
        Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Overhealed &&
            e.Target == new EntityId(12));
        Assert.Equal(new EntityId(13), Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Healed &&
            e.Amount == 2).Target);
        Assert.DoesNotContain(result.Events, e => e.Kind == SimulationEventKind.Overhealed &&
            e.Target == new EntityId(21));
    }

    [Fact]
    public void HealingEnemyOverhealMinionRewardsItsControllerNotTheHealer()
    {
        var simulator = new GameSimulator([
            Hero, HealerMinion("overheal", armor: 4),
            Spell("heal-enemy") with
            {
                Target = TargetRequirement.EnemyMinion,
                OnPlay = [new HealEffect(HealTarget.Selected, 5)]
            }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "heal-enemy", First),
            InPlay(21, "overheal", Second, 3, 1, 5));

        var result = simulator.Apply(root,
            new(SimulationActionKind.PlayCard, First, new EntityId(11), new EntityId(21)));

        Assert.Equal(0, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(4, result.State.Entities[SecondHero].Character!.Armor);
        Assert.Equal(Second, Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Overhealed).Player);
    }

    [Fact]
    public void SilencedOverhealMinionCanStillBeHealedButDoesNotTrigger()
    {
        var simulator = new GameSimulator([
            Hero, HealerMinion("overheal"),
            Spell("restore") with
            {
                Target = TargetRequirement.FriendlyMinion,
                OnPlay = [new HealEffect(HealTarget.Selected, 4)]
            }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero),
            InPlay(12, "overheal", First, 3, 1, 5) with
            {
                Silenced = true, Character = new(1, 5, Damage: 1)
            },
            Card(11, "restore", First));

        var result = simulator.Apply(root,
            new(SimulationActionKind.PlayCard, First, new EntityId(11), new EntityId(12)));

        Assert.Equal(5, result.State.Entities[new EntityId(12)].Character!.Health);
        Assert.Equal(0, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Healed);
        Assert.DoesNotContain(result.Events, e => e.Kind is SimulationEventKind.Overhealed or SimulationEventKind.Triggered);
    }

    [Fact]
    public void FriendlyCharacterHealingAffectsHeroAndMinionWithoutGivingArmorFromHero()
    {
        var simulator = new GameSimulator([
            Hero, HealerMinion("overheal"),
            Spell("team") with { OnPlay = [new HealEffect(HealTarget.AllFriendlyCharacters, 3)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero),
            InPlay(12, "overheal", First, 3, 1, 5) with { Character = new(1, 5, Damage: 1) },
            Card(11, "team", First));
        root = simulator.CreateGame(root.Players.Values,
            root.Entities.Values.Select(e => e.Id == FirstHero
                ? e with { Character = e.Character! with { Damage = 2 } } : e),
            root.ActivePlayer, root.Turn, root.MinionDeaths, root.RandomState);

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.Equal(30, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(5, result.State.Entities[new EntityId(12)].Character!.Health);
        Assert.Equal(3, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(2, result.Events.Count(e => e.Kind == SimulationEventKind.Healed));
        Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Overhealed);
    }

    [Fact]
    public void InvalidHealingRulesAndOverhealTriggerDefinitionsAreRejected()
    {
        Assert.Throws<ArgumentException>(() => new GameSimulator([
            Hero, Spell("bad-target") with { OnPlay = [new HealEffect(HealTarget.Selected, 1)] }
        ]));
        Assert.Throws<ArgumentException>(() => new GameSimulator([
            Hero, Spell("bad-amount") with { OnPlay = [new HealEffect(HealTarget.FriendlyHero, 0)] }
        ]));
        Assert.Throws<ArgumentException>(() => new GameSimulator([
            Hero, Minion("bad-overheal", 1, 2) with
            {
                Triggers = [new(TriggerTiming.Overheal, TriggerRelation.Friendly, [new GainArmorEffect(1)])
                {
                    EntityMatch = TriggerEntityMatch.Self
                }]
            }
        ]));
        Assert.Throws<ArgumentException>(() => new GameSimulator([
            Hero, Minion("wrong-focus", 1, 2) with
            {
                Keywords = CardKeywords.Overheal,
                Triggers = [new(TriggerTiming.Overheal, TriggerRelation.Friendly, [new GainArmorEffect(1)])
                {
                    Focus = TriggerFocus.DamageSource, EntityMatch = TriggerEntityMatch.Self
                }]
            }
        ]));
    }

    [Fact]
    public void SameResolutionHealingCanRescueAHeroAtZeroHealthBeforeDeathChecking()
    {
        var simulator = new GameSimulator([
            Hero,
            Spell("last-second-recovery") with
            {
                OnPlay =
                [
                    new DamageEffect(DamageTarget.FriendlyHero, 30),
                    new HealEffect(HealTarget.FriendlyHero, 2)
                ]
            }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero), Card(11, "last-second-recovery", First));

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.Equal(MatchOutcome.InProgress, result.State.Outcome);
        Assert.Equal(2, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(2, Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Healed).Amount);
    }
}
