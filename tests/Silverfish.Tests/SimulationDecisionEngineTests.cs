using System.Collections.Immutable;
using Silverfish.Contracts;
using Silverfish.Core;
using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Tests;

public sealed class SimulationDecisionEngineTests
{
    private static readonly CandidateAction Strike = new()
    {
        Id = "host-strike", Kind = GameActionKind.PlayCard, SourceEntityId = 11, TargetEntityId = 20, Choice = "strike"
    };
    private static readonly CandidateAction Guard = new()
    {
        Id = "host-guard", Kind = GameActionKind.PlayCard, SourceEntityId = 11, Choice = "guard"
    };

    [Fact]
    public void UsesCallerActionAndSnapshotIdentityForATargetedChoice()
    {
        var (engine, state) = Scenario();
        var decision = engine.Decide(state, "match-42", 8, [Guard, Strike]);

        Assert.Same(Strike, decision.ChosenAction);
        Assert.Equal("match-42", decision.MatchId);
        Assert.Equal(8, decision.SnapshotSequence);
        Assert.Equal(GameZone.Hand, state.LocationOf(new EntityId(11)).Zone);
    }

    [Fact]
    public void SearchesOnlyAllowedFirstActionsAndReturnsNoActionForAnEmptySet()
    {
        var (engine, state) = Scenario();
        var end = new CandidateAction { Id = "host-end", Kind = GameActionKind.EndTurn };
        var restricted = engine.Decide(state, "match", 0, [end, Guard]);
        var empty = engine.Decide(state, "match", 0, []);

        Assert.Same(Guard, restricted.ChosenAction);
        Assert.Null(empty.ChosenAction);
        Assert.Null(empty.Score);
    }

    [Fact]
    public void RejectsIllegalChoicesAndAmbiguousCandidateMappings()
    {
        var (engine, state) = Scenario();
        Assert.Throws<ArgumentException>(() => engine.Decide(state, "match", 0, [Strike with { TargetEntityId = null }]));
        Assert.Throws<ArgumentException>(() => engine.Decide(state, "match", 0, [Strike with { Position = 0 }]));
        Assert.Throws<ArgumentException>(() => engine.Decide(state, "match", 0, [new CandidateAction { Id = "missing-kind" }]));
        Assert.Throws<ArgumentException>(() => engine.Decide(state, "match", 0, [Strike, Guard with { Id = Strike.Id }]));
        Assert.Throws<ArgumentException>(() => engine.Decide(state, "match", 0, [Strike, Strike with { Id = Guard.Id }]));
    }

    private static (SimulationDecisionEngine Engine, GameState State) Scenario()
    {
        PlayerId first = new(1), second = new(2);
        EntityId hero = new(10), spell = new(11), enemy = new(20);
        var simulator = new GameSimulator([
            new(new("hero", EntityKind.Hero, 0, PrintedHealth: 30)),
            new SimulationCard(new("signal", EntityKind.Spell, 0))
            {
                Choices =
                [
                    new("strike", [new DamageEffect(DamageTarget.Selected, 3)]) { Target = TargetRequirement.EnemyCharacter },
                    new("guard", [new GainArmorEffect(2)])
                ]
            }
        ]);
        var state = simulator.CreateGame([
            new PlayerState(first, hero)
            {
                Zones = ImmutableDictionary<GameZone, ImmutableArray<EntityId>>.Empty
                    .Add(GameZone.Play, [hero]).Add(GameZone.Hand, [spell])
            },
            new PlayerState(second, enemy)
            {
                Zones = ImmutableDictionary<GameZone, ImmutableArray<EntityId>>.Empty.Add(GameZone.Play, [enemy])
            }
        ], [
            new GameEntity(hero, "hero", first, first) { Character = new(0, 30), PlayOrder = 1 },
            new GameEntity(enemy, "hero", second, second) { Character = new(0, 30, Damage: 27), PlayOrder = 2 },
            new GameEntity(spell, "signal", first, first)
        ], first);
        return (new(simulator), state);
    }
}
