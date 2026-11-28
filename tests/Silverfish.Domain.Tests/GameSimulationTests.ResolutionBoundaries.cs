using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void ExternalChoiceBlocksARegularPlayableSpellAndItsExplanation()
    {
        var simulator = new GameSimulator([
            Hero, Spell("usable") with { OnPlay = [new GainArmorEffect(3)] }
        ]);
        var normal = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero), Card(11, "usable", First));
        var play = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(11));
        Assert.Contains(play, simulator.LegalActions(normal));
        Assert.Null(simulator.CheckAction(normal, play));

        var waiting = simulator.CreateGame(normal.Players.Values, normal.Entities.Values, First,
            pendingChoice: ChoicePacket(min: 1, max: 1));
        Assert.Empty(simulator.LegalActions(waiting));
        Assert.NotNull(simulator.CheckAction(waiting, play));
        var explanation = simulator.ExplainSource(waiting, new EntityId(11));
        Assert.NotNull(explanation.Failure);
        Assert.Empty(explanation.Branches);
        Assert.Throws<InvalidOperationException>(() => simulator.Apply(waiting, play));
        Assert.Equal(GameZone.Hand, waiting.LocationOf(new EntityId(11)).Zone);
        Assert.Equal(0, waiting.Entities[FirstHero].Character!.Armor);
        Assert.Null(waiting.PendingChoice!.Submitted);
    }

    [Fact]
    public void SubmittedExternalChoiceStillBlocksSpellsAfterCheckpointRestore()
    {
        var simulator = new GameSimulator([
            Hero, Spell("usable") with { OnPlay = [new GainArmorEffect(3)] }
        ]);
        var waiting = simulator.CreateGame(
            [
                Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
                Player(Second, SecondHero)
            ],
            [InPlay(10, "hero", First, 1, 0, 30),
             InPlay(20, "hero", Second, 2, 0, 30), Card(11, "usable", First)],
            First, pendingChoice: ChoicePacket(min: 1, max: 1));
        var selection = new SimulationAction(SimulationActionKind.ChooseOption, First)
        {
            ChoicePacketId = 123,
            OrderedChoiceTargets = [new EntityId(33)]
        };
        var submitted = simulator.Apply(waiting, selection).State;
        var continuation = new GameContinuation(submitted.NextEntity, submitted.NextPlayOrder,
            submitted.Outcome, submitted.Winner) { PendingChoice = submitted.PendingChoice };
        var restored = simulator.RestoreGame(submitted.Players.Values, submitted.Entities.Values,
            First, submitted.Turn, submitted.MinionDeaths, submitted.RandomState, continuation);

        Assert.Equal([33], restored.PendingChoice!.Submitted!.OrderedEntityIds.Select(id => id.Value));
        Assert.Empty(simulator.LegalActions(restored));
        var ordinary = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(11));
        Assert.NotNull(simulator.CheckAction(restored, ordinary));
        Assert.Throws<InvalidOperationException>(() => simulator.Apply(restored, ordinary));
        Assert.Throws<InvalidOperationException>(() => simulator.Apply(restored, selection));
        Assert.Equal(GameZone.Hand, restored.LocationOf(new EntityId(11)).Zone);
    }

    [Fact]
    public void OrdinaryPlayRejectsSmuggledEntityChoiceFields()
    {
        var simulator = new GameSimulator([
            Hero, Spell("usable") with { OnPlay = [new GainArmorEffect(1)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero), Card(11, "usable", First));
        var ordinary = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(11));
        Assert.Null(simulator.CheckAction(root, ordinary));

        var wrongId = ordinary with { ChoicePacketId = 123 };
        var wrongTargets = ordinary with { OrderedChoiceTargets = [new EntityId(33)] };
        Assert.NotNull(simulator.CheckAction(root, wrongId));
        Assert.NotNull(simulator.CheckAction(root, wrongTargets));
        Assert.Throws<InvalidOperationException>(() => simulator.Apply(root, wrongId));
        Assert.Throws<InvalidOperationException>(() => simulator.Apply(root, wrongTargets));
        Assert.Equal(GameZone.Hand, root.LocationOf(new EntityId(11)).Zone);
    }

    [Fact]
    public void WeaponCombatDamagePreservesHeroSourceAndReportsWeaponProvenance()
    {
        var weapon = new SimulationCard(new("blade", EntityKind.Weapon, 0,
            PrintedAttack: 2, PrintedDurability: 2)) { Keywords = CardKeywords.Lifesteal };
        var simulator = new GameSimulator([Hero, weapon]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                EquippedWeapon = new EntityId(11),
                Zones = Zones((GameZone.Play, [10, 11]))
            },
            Player(Second, SecondHero),
            Weapon(11, "blade", First, 3, 2, 2));
        var damaged = WithHeroDamage(simulator, root, 4);
        var result = simulator.Apply(damaged,
            new(SimulationActionKind.Attack, First, FirstHero, SecondHero));

        var hit = Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Damage);
        Assert.Equal(FirstHero, hit.Source);
        Assert.Equal(SecondHero, hit.Target);
        Assert.Equal(new EntityId(11), hit.AttackingWeapon);
        Assert.Equal(2, hit.DamageToHealth);
        Assert.Equal(0, hit.DamageToArmor);
        var healed = Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Healed);
        Assert.Equal(new EntityId(11), healed.Source);
        Assert.Equal(FirstHero, healed.Target);
        Assert.Equal(28, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(26, damaged.Entities[FirstHero].Character!.Health);
    }

    [Fact]
    public void AreaOverhealEmitsDamageIndependentEventsAndTriggersInPlayOrder()
    {
        var simulator = new GameSimulator([
            Hero, HealerMinion("healer", armor: 1),
            Spell("area") with { OnPlay = [new HealEffect(HealTarget.FriendlyMinions, 2)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13]))
            },
            Player(Second, SecondHero),
            InPlay(11, "healer", First, 3, 1, 5),
            InPlay(12, "healer", First, 4, 1, 5) with
            {
                Character = new(1, 5, Damage: 1)
            },
            Card(13, "area", First));

        var action = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(13));
        var result = simulator.Apply(root, action);
        var replay = simulator.Apply(root, action);
        Assert.Equal([new EntityId(11), new EntityId(12)],
            result.Events.Where(e => e.Kind == SimulationEventKind.Overhealed).Select(e => e.Target!.Value));
        Assert.Equal([2, 1],
            result.Events.Where(e => e.Kind == SimulationEventKind.Overhealed).Select(e => e.Amount!.Value));
        Assert.Equal([new EntityId(11), new EntityId(12)],
            result.Events.Where(e => e.Kind == SimulationEventKind.Triggered &&
                e.Timing == TriggerTiming.Overheal).Select(e => e.Source!.Value));
        Assert.Equal(2, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal<SimulationEvent>(result.Events, replay.Events);
        Assert.Equal(0, root.Entities[FirstHero].Character!.Armor);
        Assert.Equal(4, root.Entities[new EntityId(12)].Character!.Health);
    }

    [Fact]
    public void OrderedChoicePreviewReplaysIdenticallyAcrossRestoredSnapshots()
    {
        var simulator = new GameSimulator([Hero, Spell("choice-source")]);
        var root = ChoicePreviewState(simulator, EntityChoiceType.General, 2, 2, [31, 32]);
        var rules = new ChoicePreviewRules(771, EntityChoiceType.General)
        {
            PerOptionEffects = ImmutableDictionary<EntityId, ImmutableArray<CardEffect>>.Empty
                .Add(new EntityId(31), [new GainArmorEffect(2)])
                .Add(new EntityId(32), [new GainArmorEffect(3)])
        };
        var continuation = new GameContinuation(root.NextEntity, root.NextPlayOrder,
            root.Outcome, root.Winner) { PendingChoice = root.PendingChoice };
        var restored = new GameSimulator(simulator.Cards.Values).RestoreGame(
            root.Players.Values, root.Entities.Values, root.ActivePlayer, root.Turn,
            root.MinionDeaths, root.RandomState, continuation);
        var action = PreviewSelection(32, 31);
        var direct = simulator.PreviewChoice(root, action, rules);
        var resumed = simulator.PreviewChoice(restored, action, rules);

        Assert.Equal(5, direct.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal<SimulationEvent>(direct.Events, resumed.Events);
        Assert.Equal([3, 2], direct.Events.Where(e => e.Kind == SimulationEventKind.ArmorGained)
            .Select(e => e.Amount!.Value));
        Assert.Equal(0, root.Entities[FirstHero].Character!.Armor);
        Assert.Null(root.PendingChoice!.Submitted);
        Assert.Null(restored.PendingChoice!.Submitted);
    }
}
