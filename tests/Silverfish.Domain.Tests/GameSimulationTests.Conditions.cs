using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Theory]
    [InlineData(false, 2, 2, 29)]
    [InlineData(true, 1, 4, 30)]
    public void ConditionalEffectsUseTheStateAfterDamageAndTriggersCheckBeforeActivating(
        bool shielded, int expectedArmor, int expectedHealth, int expectedEnemyHealth)
    {
        var simulator = new GameSimulator([
            Hero,
            Minion("unit", 0, 4),
            Minion("observer", 0, 3) with
            {
                Triggers =
                [
                    new(TriggerTiming.AfterSpellResolved, TriggerRelation.Friendly, [new DamageEffect(DamageTarget.EnemyHero, 1)])
                    {
                        Condition = new CharacterCondition(ConditionCharacter.FriendlyHero, CharacterMetric.Armor, NumberComparison.AtLeast, 2)
                    }
                ]
            },
            Spell("signal") with
            {
                Target = TargetRequirement.EnemyMinion,
                OnPlay =
                [
                    new DamageEffect(DamageTarget.Selected, 1),
                    new ConditionalEffect(new CharacterCondition(ConditionCharacter.Selected, CharacterMetric.Damage,
                        NumberComparison.AtLeast, 1), [
                            new GainArmorEffect(2),
                            new ConditionalEffect(new ZoneCountCondition(ConditionPlayer.Opponent, GameZone.Play, NumberComparison.AtLeast, 1,
                                EntityKind.Minion), [new DamageEffect(DamageTarget.Selected, 1)])
                        ])
                    {
                        Otherwise = [new GainArmorEffect(1)]
                    }
                ]
            }
        ]);
        var source = new EntityId(12);
        var target = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "observer", First, 3, 0, 3), Card(12, "signal", First),
            Card(21, "unit", Second) with { PlayOrder = 4, Character = new(0, 4, DivineShield: shielded) });

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, source, target));

        Assert.Equal(expectedArmor, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(expectedHealth, result.State.Entities[target].Character!.Health);
        Assert.Equal(expectedEnemyHealth, result.State.Entities[SecondHero].Character!.Health);
        Assert.Equal(!shielded, result.Events.Any(change => change.Kind == SimulationEventKind.Triggered));
        Assert.Equal(4, root.Entities[target].Character!.Health);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(2, true)]
    public void SecretConditionControlsWhetherTheSecretReveals(int armor, bool revealed)
    {
        var simulator = new GameSimulator([
            Hero, Spell("signal") with { OnPlay = [new GainArmorEffect(1)] },
            Spell("ward") with
            {
                IsSecret = true,
                Triggers =
                [
                    new(TriggerTiming.SpellCounterWindow, TriggerRelation.Opponent, [new CounterSpellEffect()])
                    {
                        Condition = new CharacterCondition(ConditionCharacter.FriendlyHero, CharacterMetric.Armor, NumberComparison.AtLeast, 2)
                    }
                ]
            }
        ]);
        var hero = InPlay(20, "hero", Second, 2, 0, 30) with { Character = new(0, 30, Armor: armor) };
        var initial = simulator.CreateGame([
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Secret, [21])) }
        ], [
            InPlay(10, "hero", First, 1, 0, 30), hero, Card(11, "signal", First),
            Card(21, "ward", Second) with { PlayOrder = 3 }
        ], First);

        var result = simulator.Apply(initial, new(SimulationActionKind.PlayCard, First, new EntityId(11)));
        Assert.Equal(revealed ? GameZone.Graveyard : GameZone.Secret, result.State.LocationOf(new EntityId(21)).Zone);
        Assert.Equal(revealed, result.Events.Any(change => change.Kind == SimulationEventKind.SpellCountered));
        Assert.Equal(revealed ? 0 : 1, result.State.Entities[FirstHero].Character!.Armor);
    }

    [Fact]
    public void ValidatesTheEffectsInBothConditionalBranches()
    {
        var card = Spell("signal") with
        {
            OnPlay =
            [
                new ConditionalEffect(new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Hand, NumberComparison.AtLeast, 1),
                    [new GainArmorEffect(1)])
                {
                    Otherwise = [new DamageEffect(DamageTarget.Selected, 1)]
                }
            ]
        };
        Assert.Throws<ArgumentException>(() => new GameSimulator([Hero, card]));
    }
}
