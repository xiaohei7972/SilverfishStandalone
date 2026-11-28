using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void EachSpellChoiceUsesItsOwnTargetAndEffects()
    {
        var simulator = new GameSimulator([
            Hero, Minion("unit", 0, 4),
            Spell("signal") with
            {
                Choices =
                [
                    new("strike", [new DamageEffect(DamageTarget.Selected, 2)])
                    {
                        PlayReqs = [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)]
                    },
                    new("guard", [new GainArmorEffect(3)])
                ]
            }
        ]);
        var source = new EntityId(12);
        var target = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "unit", First, 3, 0, 4), Card(12, "signal", First), InPlay(21, "unit", Second, 4, 0, 4));
        var strike = new SimulationAction(SimulationActionKind.PlayCard, First, source, target) { Choice = "strike" };
        var guard = new SimulationAction(SimulationActionKind.PlayCard, First, source) { Choice = "guard" };

        Assert.Equal(new[] { strike, guard }, simulator.LegalActions(root).Where(action => action.Source == source));
        Assert.Equal(ErrorType.REQ_MINION_TARGET, simulator.CheckAction(root, strike with { Target = SecondHero })?.Type);
        Assert.NotNull(simulator.CheckAction(root, strike with { Target = source }));
        Assert.Throws<InvalidOperationException>(() => simulator.Apply(root, strike with { Target = null }));
        Assert.Throws<InvalidOperationException>(() => simulator.Apply(root, guard with { Target = target }));
        var struck = simulator.Apply(root, strike).State;
        var guarded = simulator.Apply(root, guard).State;
        Assert.Equal(2, struck.Entities[target].Character!.Health);
        Assert.Equal(0, struck.Entities[FirstHero].Character!.Armor);
        Assert.Equal(4, guarded.Entities[target].Character!.Health);
        Assert.Equal(3, guarded.Entities[FirstHero].Character!.Armor);
        Assert.Equal(GameZone.Hand, root.LocationOf(source).Zone);
    }

    [Fact]
    public void BoardSpaceRequirementAppliesToTheSummoningChoiceOnly()
    {
        var simulator = new GameSimulator([
            Hero, Minion("unit", 0, 4),
            Spell("signal") with
            {
                Choices =
                [
                    new("strike", [new DamageEffect(DamageTarget.Selected, 2)]) { Target = TargetRequirement.EnemyCharacter },
                    new("deploy", [new SummonEffect("unit")]) { Requirements = PlayRequirements.BoardSpace }
                ]
            }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Zones = Zones((GameZone.Play, [10, 11, 12, 13, 14, 15, 16, 17]), (GameZone.Hand, [18]))
            },
            Player(Second, SecondHero),
            [.. Enumerable.Range(11, 7).Select(id => InPlay(id, "unit", First, id, 0, 4)), Card(18, "signal", First)]);

        var action = Assert.Single(simulator.LegalActions(root), action => action.Kind == SimulationActionKind.PlayCard);
        Assert.Equal("strike", action.Choice);
        Assert.Equal(SecondHero, action.Target);
    }

    [Fact]
    public void MinionCanEnterPlayWithoutAChoiceTargetAndSkipThatEffect()
    {
        var options = new CardPlayChoice[]
        {
            new("strike", [new DamageEffect(DamageTarget.Selected, 2), new GainArmorEffect(3)])
                { Target = TargetRequirement.EnemyMinion },
            new("guard", [new GainArmorEffect(1)])
        };
        var simulator = new GameSimulator([
            Hero,
            Minion("unit", 0, 4) with { Choices = [.. options] },
            Spell("signal") with { Choices = [.. options] }
        ]);
        var source = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero), Card(11, "unit", First), Card(12, "signal", First));

        var strike = Assert.Single(simulator.LegalActions(root), action => action.Choice == "strike");
        Assert.Equal(source, strike.Source);
        Assert.Null(strike.Target);
        var result = simulator.Apply(root, strike);
        Assert.Equal(GameZone.Play, result.State.LocationOf(source).Zone);
        Assert.Equal(0, result.State.Entities[FirstHero].Character!.Armor);
        Assert.DoesNotContain(result.Events, e => e.Kind == SimulationEventKind.Damage);
    }
}
