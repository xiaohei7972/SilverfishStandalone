using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    private static GameState ChoicePreviewState(GameSimulator simulator, EntityChoiceType type,
        int min, int max, int[] offered, params GameEntity[] others)
    {
        var packet = new EntityChoicePacket(771, type, First, new EntityId(11), min, max,
            offered.Select(id => new EntityId(id)).ToImmutableArray(), []);
        return CreateChoiceState(simulator, packet, others);
    }

    private static GameState CreateChoiceState(GameSimulator simulator, EntityChoicePacket packet,
        GameEntity[] others)
    {
        var first = Player(First, FirstHero) with
        {
            Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]))
        };
        var extraTargets = others.Where(x => x.Controller == Second && x.Character is not null)
            .Select(x => x.Id.Value).ToArray();
        var second = Player(Second, SecondHero) with
        {
            Zones = Zones((GameZone.Play, [20, ..extraTargets]))
        };
        return simulator.CreateGame([first, second],
            [InPlay(10, "hero", First, 1, 0, 30),
             InPlay(20, "hero", Second, 2, 0, 30),
             Card(11, "choice-source", First), ..others],
            First, pendingChoice: new(packet));
    }

    private static SimulationAction PreviewSelection(params int[] ids) =>
        new(SimulationActionKind.ChooseOption, First)
        {
            ChoicePacketId = 771,
            OrderedChoiceTargets = ids.Select(id => new EntityId(id)).ToImmutableArray()
        };

    [Fact]
    public void DiscoverPreviewCreatesKnownSelectedCardWithoutSubmittingRealChoice()
    {
        var simulator = new GameSimulator([
            Hero, Spell("choice-source"), Minion("cheap", 1, 1), Minion("strong", 6, 6)
        ]);
        var root = ChoicePreviewState(simulator, EntityChoiceType.General, 1, 1, [31, 32]);
        var rules = new ChoicePreviewRules(771, EntityChoiceType.General)
        {
            PerOptionEffects = ImmutableDictionary<EntityId, ImmutableArray<CardEffect>>.Empty
                .Add(new EntityId(31), [new CreateCardEffect("cheap")])
                .Add(new EntityId(32), [new CreateCardEffect("strong")])
        };

        var preview = simulator.PreviewChoice(root, PreviewSelection(32), rules);

        var added = Assert.Single(preview.State.Players[First].In(GameZone.Hand),
            id => preview.State.Entities[id].CardId == "strong");
        Assert.Equal("strong", preview.State.Entities[added].CardId);
        Assert.Equal(2, preview.State.Players[First].In(GameZone.Hand).Length);
        Assert.Single(root.Players[First].In(GameZone.Hand));
        Assert.Null(root.PendingChoice!.Submitted);
        Assert.Equal([32], preview.State.PendingChoice!.Submitted!.OrderedEntityIds.Select(id => id.Value));
        Assert.Empty(simulator.LegalActions(preview.State));
        Assert.Equal(SimulationEventKind.ChoiceSubmitted, preview.Events[0].Kind);
        Assert.Contains(preview.Events, e => e.Kind == SimulationEventKind.CardGenerated);
    }

    [Fact]
    public void OrderedTargetPreviewResolvesEachTargetSeparatelyWithoutChangingTheInput()
    {
        var simulator = new GameSimulator([
            Hero, Spell("choice-source"), Minion("victim", 1, 3)
        ]);
        var root = ChoicePreviewState(simulator, EntityChoiceType.Target, 2, 2, [21, 22],
            InPlay(21, "victim", Second, 3, 1, 3),
            InPlay(22, "victim", Second, 4, 1, 3));
        var rules = new ChoicePreviewRules(771, EntityChoiceType.Target)
        {
            PerTargetEffects = [new DamageEffect(DamageTarget.Selected, 2)]
        };

        var preview = simulator.PreviewChoice(root, PreviewSelection(22, 21), rules);

        Assert.Equal([new EntityId(22), new EntityId(21)], preview.Events
            .Where(e => e.Kind == SimulationEventKind.Damage).Select(e => e.Target!.Value));
        Assert.Equal(1, preview.State.Entities[new EntityId(21)].Character!.Health);
        Assert.Equal(1, preview.State.Entities[new EntityId(22)].Character!.Health);
        Assert.Equal(3, root.Entities[new EntityId(21)].Character!.Health);
        Assert.Null(root.PendingChoice!.Submitted);
        Assert.Empty(simulator.LegalActions(preview.State));
    }

    [Fact]
    public void KnownChoicePreviewPreservesTheRealSubmissionHandshake()
    {
        var simulator = new GameSimulator([Hero, Spell("choice-source")]);
        var root = ChoicePreviewState(simulator, EntityChoiceType.General, 1, 1, [31, 32]);
        var rules = new ChoicePreviewRules(771, EntityChoiceType.General)
        {
            PerOptionEffects = ImmutableDictionary<EntityId, ImmutableArray<CardEffect>>.Empty
                .Add(new EntityId(31), [new GainArmorEffect(2)])
                .Add(new EntityId(32), [new GainArmorEffect(5)])
        };
        var preview = simulator.PreviewChoice(root, PreviewSelection(32), rules);
        var authoritativeIntent = simulator.Apply(root, PreviewSelection(32));

        Assert.Equal(5, preview.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(0, authoritativeIntent.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(0, root.Entities[FirstHero].Character!.Armor);
        Assert.Single(authoritativeIntent.Events);
        Assert.Equal(SimulationEventKind.ChoiceSubmitted, authoritativeIntent.Events[0].Kind);
        Assert.Throws<InvalidOperationException>(() =>
            simulator.PreviewChoice(authoritativeIntent.State, PreviewSelection(32), rules));
    }

    [Fact]
    public void PreviewRefusesUnknownOfferedChoiceAndPacketMismatch()
    {
        var simulator = new GameSimulator([Hero, Spell("choice-source")]);
        var root = ChoicePreviewState(simulator, EntityChoiceType.General, 1, 1, [31, 32]);
        var incomplete = new ChoicePreviewRules(771, EntityChoiceType.General)
        {
            PerOptionEffects = ImmutableDictionary<EntityId, ImmutableArray<CardEffect>>.Empty
                .Add(new EntityId(31), [new GainArmorEffect(3)])
        };
        Assert.Throws<ArgumentException>(() =>
            simulator.PreviewChoice(root, PreviewSelection(31), incomplete));
        Assert.Throws<ArgumentException>(() =>
            simulator.PreviewChoice(root, PreviewSelection(31), incomplete with { ChoiceId = 772 }));
        Assert.Throws<ArgumentException>(() =>
            simulator.PreviewChoice(root, PreviewSelection(31), incomplete with { Type = EntityChoiceType.Target }));
        Assert.Null(root.PendingChoice!.Submitted);
    }

    [Fact]
    public void TargetPreviewRejectsUnknownCharactersAndMulligan()
    {
        var simulator = new GameSimulator([Hero, Spell("choice-source"), Minion("victim", 1, 2)]);
        var root = ChoicePreviewState(simulator, EntityChoiceType.Target, 1, 1, [31]);
        var targetRules = new ChoicePreviewRules(771, EntityChoiceType.Target)
        {
            PerTargetEffects = [new DamageEffect(DamageTarget.Selected, 1)]
        };
        Assert.Throws<ArgumentException>(() =>
            simulator.PreviewChoice(root, PreviewSelection(31), targetRules));
        var mulligan = ChoicePreviewState(simulator, EntityChoiceType.Mulligan, 1, 1, [31]);
        Assert.Throws<ArgumentException>(() =>
            simulator.PreviewChoice(mulligan, PreviewSelection(31),
                new ChoicePreviewRules(771, EntityChoiceType.Mulligan)));
    }

    [Fact]
    public void PreviewEnumeratesRandomEffectsIndependentlyOfReplaySeed()
    {
        var simulator = new GameSimulator([
            Hero, Spell("choice-source"), Minion("victim", 1, 3)
        ]);
        var root = ChoicePreviewState(simulator, EntityChoiceType.General, 1, 1, [31, 32],
            InPlay(21, "victim", Second, 3, 1, 3),
            InPlay(22, "victim", Second, 4, 1, 3));
        var rules = new ChoicePreviewRules(771, EntityChoiceType.General)
        {
            PerOptionEffects = ImmutableDictionary<EntityId, ImmutableArray<CardEffect>>.Empty
                .Add(new EntityId(31), [new RandomDamageEffect(RandomCharacterPool.EnemyMinions, 1)])
                .Add(new EntityId(32), [new GainArmorEffect(2)])
        };
        var otherSeed = simulator.CreateGame(root.Players.Values, root.Entities.Values,
            First, randomState: 9182, pendingChoice: root.PendingChoice);

        var first = simulator.PreviewChoiceDistribution(root, PreviewSelection(31), rules);
        var rerun = simulator.PreviewChoiceDistribution(otherSeed, PreviewSelection(31), rules);

        Assert.True(first.IsExact);
        Assert.Equal(2, first.Outcomes.Length);
        Assert.All(first.Outcomes, x => Assert.Equal(0.5, x.Probability));
        Assert.Equal(first.Outcomes.Select(x => x.Result.State.Entities[new EntityId(21)].Character!.Health),
            rerun.Outcomes.Select(x => x.Result.State.Entities[new EntityId(21)].Character!.Health));
        Assert.Null(root.PendingChoice!.Submitted);
        Assert.Throws<InvalidOperationException>(() => simulator.PreviewChoice(root, PreviewSelection(31), rules));
    }
}
