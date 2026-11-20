using System.Collections.Immutable;
using Silverfish.Core;
using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Tests;

public sealed class EntityChoiceForecastPlannerTests
{
    private static readonly PlayerId First = new(1);
    private static readonly PlayerId Second = new(2);

    private static GameSimulator CreateSimulator() => new([
        new SimulationCard(new("hero", EntityKind.Hero, 0, PrintedHealth: 30)),
        new SimulationCard(new("choice-source", EntityKind.Spell, 0)),
        new SimulationCard(new("weak", EntityKind.Minion, 0, 1, 1)),
        new SimulationCard(new("strong", EntityKind.Minion, 0, 6, 6)),
        new SimulationCard(new("target", EntityKind.Minion, 0, 0, 4))
    ]);

    private static GameState CreateState(GameSimulator simulator,
        EntityChoiceType type = EntityChoiceType.General,
        int min = 1, int max = 1, int[]? offered = null, int choiceId = 771)
    {
        var one = new EntityId(10);
        var two = new EntityId(20);
        var source = new EntityId(11);
        var choices = (offered ?? [31, 32]).Select(id => new EntityId(id)).ToImmutableArray();
        var packet = new EntityChoicePacket(choiceId, type, First, source, min, max, choices, []);
        var oneZones = ImmutableDictionary<GameZone, ImmutableArray<EntityId>>.Empty
            .Add(GameZone.Play, [one]).Add(GameZone.Hand, [source]);
        var twoZones = ImmutableDictionary<GameZone, ImmutableArray<EntityId>>.Empty
            .Add(GameZone.Play, [two]);
        return simulator.CreateGame(
            [new PlayerState(First, one) { Zones = oneZones },
             new PlayerState(Second, two) { Zones = twoZones }],
            [new GameEntity(one, "hero", First, First) { Character = new(0, 30), PlayOrder = 1 },
             new GameEntity(two, "hero", Second, Second) { Character = new(0, 30), PlayOrder = 2 },
             new GameEntity(source, "choice-source", First, First)],
            First, pendingChoice: new(packet));
    }

    private static ChoicePreviewRules DiscoverRules() =>
        new(771, EntityChoiceType.General)
        {
            PerOptionEffects = ImmutableDictionary<EntityId, ImmutableArray<CardEffect>>.Empty
                .Add(new EntityId(31), [new CreateCardEffect("weak")])
                .Add(new EntityId(32), [new CreateCardEffect("strong")])
        };

    private static double HandValue(GameState state, PlayerId perspective) =>
        state.Players[perspective].In(GameZone.Hand)
            .Sum(id => state.Entities[id].CardId == "strong" ? 20.0 :
                       state.Entities[id].CardId == "weak" ? 1.0 : 0.0);

    [Fact]
    public void PicksBestKnownDiscoverCardAndPreservesSourceState()
    {
        var simulator = CreateSimulator();
        var root = CreateState(simulator);
        var planner = new EntityChoiceForecastPlanner(simulator, HandValue);

        var forecast = planner.Plan(root, DiscoverRules());

        Assert.True(forecast.IsComplete);
        Assert.Equal(2, forecast.EvaluatedCandidates);
        Assert.Equal(20, forecast.Score);
        Assert.Equal([32], forecast.Action!.OrderedChoiceTargets.Select(id => id.Value));
        Assert.Null(root.PendingChoice!.Submitted);
        Assert.Empty(simulator.LegalActions(root));
        var intent = simulator.Apply(root, forecast.Action);
        Assert.Null(intent.State.Entities.Values.FirstOrDefault(e => e.CardId == "strong"));
        Assert.NotNull(intent.State.PendingChoice!.Submitted);
    }

    [Fact]
    public void DoesNotReturnPartialRankingWhenCandidateBudgetIsTooSmall()
    {
        var simulator = CreateSimulator();
        var root = CreateState(simulator);
        var planner = new EntityChoiceForecastPlanner(simulator, HandValue);

        var forecast = planner.Plan(root, DiscoverRules(), candidateLimit: 1);

        Assert.False(forecast.IsComplete);
        Assert.Null(forecast.Action);
        Assert.Equal(1, forecast.EvaluatedCandidates);
        Assert.Null(root.PendingChoice!.Submitted);
    }

    [Fact]
    public void OptionalZeroSelectionIsConsideredWithoutInventingAnEffect()
    {
        var simulator = CreateSimulator();
        var root = CreateState(simulator, min: 0, max: 0, offered: []);
        var rules = new ChoicePreviewRules(771, EntityChoiceType.General);
        var forecast = new EntityChoiceForecastPlanner(simulator, HandValue).Plan(root, rules);

        Assert.True(forecast.IsComplete);
        Assert.Empty(forecast.Action!.OrderedChoiceTargets);
        Assert.Equal(1, forecast.EvaluatedCandidates);
        Assert.Equal(771, forecast.Action.ChoicePacketId);
    }

    [Fact]
    public void ASubmittedChoiceCannotBeForecastAgainBeforeNextSnapshot()
    {
        var simulator = CreateSimulator();
        var root = CreateState(simulator);
        var action = new SimulationAction(SimulationActionKind.ChooseOption, First)
        {
            ChoicePacketId = 771, OrderedChoiceTargets = [new EntityId(31)]
        };
        var submitted = simulator.Apply(root, action).State;
        var planner = new EntityChoiceForecastPlanner(simulator, HandValue);

        var forecast = planner.Plan(submitted, DiscoverRules());

        Assert.Null(forecast.Action);
        Assert.False(forecast.IsComplete);
        Assert.Equal(0, forecast.EvaluatedCandidates);
    }

    [Fact]
    public void ForecastRequiresMatchingChoicePacketAndNeverGuessesTheNextOne()
    {
        var simulator = CreateSimulator();
        var root = CreateState(simulator, choiceId: 772);
        var planner = new EntityChoiceForecastPlanner(simulator, HandValue);

        var result = planner.Plan(root, DiscoverRules());

        Assert.Null(result.Action);
        Assert.False(result.IsComplete);
        Assert.Equal(0, result.EvaluatedCandidates);
        Assert.Null(root.PendingChoice!.Submitted);
    }
}
