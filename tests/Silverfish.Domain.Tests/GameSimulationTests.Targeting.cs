using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void GeneratesOnlyTargetsMeetingAllComparisonsInTheCurrentState()
    {
        var simulator = new GameSimulator([
            Hero, Minion("unit", 2, 5),
            Spell("prime") with
            {
                Target = TargetRequirement.EnemyMinion,
                OnPlay = [new DamageEffect(DamageTarget.Selected, 1), new FreezeEffect(FreezeTarget.Selected)]
            },
            Spell("signal") with
            {
                Target = TargetRequirement.EnemyMinion,
                TargetFilter = new()
                {
                    Frozen = true,
                    Comparisons = [
                        new(CharacterMetric.Attack, NumberComparison.AtMost, 3),
                        new(CharacterMetric.Damage, NumberComparison.AtLeast, 1)
                    ]
                },
                OnPlay = [new DamageEffect(DamageTarget.Selected, 1)]
            }
        ]);
        EntityId prime = new(11), signal = new(12), target = new(21);
        var state = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21, 22, 23, 24])) },
            Card(11, "prime", First), Card(12, "signal", First), InPlay(21, "unit", Second, 3, 2, 5),
            Card(22, "unit", Second) with { PlayOrder = 4, Character = new(4, 5, Damage: 1) { Frozen = true } },
            Card(23, "unit", Second) with { PlayOrder = 5, Character = new(2, 5, Damage: 1) },
            Card(24, "unit", Second) with { PlayOrder = 6, Character = new(2, 5) { Frozen = true } });
        var cast = new SimulationAction(SimulationActionKind.PlayCard, First, signal, target);
        Assert.DoesNotContain(simulator.LegalActions(state), action => action.Source == signal);
        Assert.Throws<InvalidOperationException>(() => simulator.Apply(state, cast));

        var changed = simulator.Apply(state, new(SimulationActionKind.PlayCard, First, prime, target)).State;
        Assert.Equal(cast, Assert.Single(simulator.LegalActions(changed), action => action.Source == signal));
        Assert.Equal(3, simulator.Apply(changed, cast).State.Entities[target].Character!.Health);
    }

    [Fact]
    public void FriendlyCharacterFilterAppliesToHeroPowers()
    {
        var simulator = new GameSimulator([
            Hero,
            new SimulationCard(new("pulse", EntityKind.HeroPower, 0))
            {
                Target = TargetRequirement.FriendlyCharacter,
                TargetFilter = new() { Comparisons = [new(CharacterMetric.Damage, NumberComparison.AtLeast, 1)] },
                OnPlay = [new DamageEffect(DamageTarget.Selected, 1)]
            }
        ]);
        var state = simulator.CreateGame([
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.HeroPower, [11])) },
            Player(Second, SecondHero)
        ], [
            Card(10, "hero", First) with { PlayOrder = 1, Character = new(0, 30, Damage: 1) },
            Card(20, "hero", Second) with { PlayOrder = 2, Character = new(0, 30, Damage: 5) },
            Card(11, "pulse", First)
        ], First);

        var usePower = Assert.Single(simulator.LegalActions(state), action => action.Kind == SimulationActionKind.UseHeroPower);
        Assert.Equal(FirstHero, usePower.Target);
        Assert.Equal(28, simulator.Apply(state, usePower).State.Entities[FirstHero].Character!.Health);
    }

    [Fact]
    public void AMinionCanEnterPlayWhenItsFilterExcludesAllTargets()
    {
        var simulator = new GameSimulator([
            Hero,
            Minion("unit", 0, 4) with
            {
                Target = TargetRequirement.EnemyCharacter,
                TargetFilter = new() { Frozen = true },
                OnPlay = [new DamageEffect(DamageTarget.Selected, 1)]
            }
        ]);
        var state = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero), Card(11, "unit", First));

        var action = Assert.Single(simulator.LegalActions(state), candidate => candidate.Kind == SimulationActionKind.PlayCard);
        Assert.Null(action.Target);
        var result = simulator.Apply(state, action);
        Assert.Equal(GameZone.Play, result.State.LocationOf(new EntityId(11)).Zone);
        Assert.DoesNotContain(result.Events, change => change.Kind == SimulationEventKind.Damage);
    }

    [Fact]
    public void RejectsFiltersWithNoTargetOrInvalidComparisons()
    {
        Assert.Throws<ArgumentException>(() => new GameSimulator([
            Hero, Spell("signal") with { TargetFilter = new() { Frozen = true } }
        ]));
        Assert.Throws<ArgumentException>(() => new GameSimulator([
            Hero, Spell("signal") with
            {
                Target = TargetRequirement.Character,
                TargetFilter = new() { Comparisons = [new(CharacterMetric.Health, NumberComparison.AtLeast, -1)] }
            }
        ]));
    }
}
