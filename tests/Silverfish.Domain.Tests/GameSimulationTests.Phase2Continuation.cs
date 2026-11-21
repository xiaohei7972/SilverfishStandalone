using System.Text.Json;
using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    private static GameSimulator AdaptDamageSimulator(bool minionSource = false)
    {
        var optionDamage = Spell("adapt-damage") with
        {
            IsPlayChoiceOption = true,
            Target = TargetRequirement.Minion,
            OnPlay = [new DamageEffect(DamageTarget.Selected, 3)]
        };
        var optionArmor = Spell("adapt-armor") with
        {
            IsPlayChoiceOption = true,
            OnPlay = [new GainArmorEffect(2)]
        };
        var optionHeal = Spell("adapt-heal") with
        {
            IsPlayChoiceOption = true,
            Target = TargetRequirement.Minion,
            OnPlay = [new HealEffect(HealTarget.Selected, 1)]
        };
        var rule = new AdaptChoice(["adapt-damage", "adapt-armor", "adapt-heal"],
            minionSource ? AdaptRecipient.Source : AdaptRecipient.FriendlyMinions, Times: 2);
        var source = minionSource ? Minion("adapt-source", 1, 2) with { ChooseAfterPlay = rule }
            : Spell("adapt-source") with { ChooseAfterPlay = rule };
        return new GameSimulator([Hero, source, optionDamage, optionArmor, optionHeal,
            Minion("fragile", 1, 2), Minion("sturdy", 1, 6)]);
    }

    private static SimulationAction AdaptOption(GameSimulator simulator, GameState state, string cardId) =>
        Assert.Single(simulator.LegalActions(state), action =>
            action.Kind == SimulationActionKind.ChooseOption &&
            state.Entities[action.Target!.Value].CardId == cardId);

    [Fact]
    public void MultiRoundAdaptFiltersDeadRecipientsBeforeNextOfferAndRestoresDeterministically()
    {
        var simulator = AdaptDamageSimulator();
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Zones = Zones((GameZone.Play, [10, 12, 13]), (GameZone.Hand, [11]))
            },
            Player(Second, SecondHero),
            Card(11, "adapt-source", First),
            InPlay(12, "fragile", First, 3, 1, 2),
            InPlay(13, "sturdy", First, 4, 1, 6));
        var first = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));
        var paused = first.State;
        Assert.Equal(2, paused.PendingPlayChoice!.RemainingSelections);
        Assert.Equal([new EntityId(12), new EntityId(13)], paused.PendingPlayChoice.Recipients!.Value);

        var damageOption = AdaptOption(simulator, paused, "adapt-damage");
        var firstChoice = simulator.Apply(paused, damageOption);
        var next = firstChoice.State;
        Assert.Equal(GameZone.Graveyard, next.LocationOf(new EntityId(12)).Zone);
        Assert.Equal(3, next.Entities[new EntityId(13)].Character!.Health);
        Assert.Equal(1, next.PendingPlayChoice!.RemainingSelections);
        Assert.Equal([new EntityId(13)], next.PendingPlayChoice.Recipients!.Value);
        Assert.All(paused.PendingPlayChoice.Options, id => Assert.Equal(GameZone.Removed, next.LocationOf(id).Zone));
        Assert.Empty(paused.PendingPlayChoice.Options.Intersect(next.PendingPlayChoice.Options));
        Assert.Single(firstChoice.Events, e => e.Kind == SimulationEventKind.Died);
        Assert.DoesNotContain(firstChoice.Events, e => e.Kind == SimulationEventKind.CardPlayed);

        var continuation = new GameContinuation(next.NextEntity, next.NextPlayOrder, next.Outcome, next.Winner)
        {
            PendingPlayChoice = next.PendingPlayChoice
        };
        var checkpoint = JsonSerializer.Deserialize<GameContinuation>(JsonSerializer.Serialize(continuation))!;
        var restored = simulator.RestoreGame(next.Players.Values, next.Entities.Values, next.ActivePlayer,
            next.Turn, next.MinionDeaths, next.RandomState, checkpoint);
        var armorOption = AdaptOption(simulator, restored, "adapt-armor");
        var replay = simulator.Apply(next, armorOption);
        var resumed = simulator.Apply(restored, armorOption);

        Assert.Equal<SimulationEvent>(replay.Events, resumed.Events);
        Assert.Equal(2, resumed.State.Entities[FirstHero].Character!.Armor);
        Assert.Null(resumed.State.PendingPlayChoice);
        Assert.Equal(GameZone.Graveyard, resumed.State.LocationOf(new EntityId(11)).Zone);
        Assert.Equal(paused.Players[First].Mana, resumed.State.Players[First].Mana);
        Assert.Equal(GameZone.Play, root.LocationOf(new EntityId(12)).Zone);
        Assert.Equal(0, root.Entities[FirstHero].Character!.Armor);
    }

    [Fact]
    public void MultiRoundAdaptStopsIfEveryRecipientDiesAndDoesNotOfferGhostChoices()
    {
        var simulator = AdaptDamageSimulator();
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero),
            Card(11, "adapt-source", First),
            InPlay(12, "fragile", First, 3, 1, 2));
        var paused = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11))).State;
        var result = simulator.Apply(paused, AdaptOption(simulator, paused, "adapt-damage"));

        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new EntityId(12)).Zone);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new EntityId(11)).Zone);
        Assert.Null(result.State.PendingPlayChoice);
        Assert.DoesNotContain(result.Events, e => e.Kind == SimulationEventKind.PlayChoiceOffered);
        Assert.Equal(0, result.State.Entities[FirstHero].Character!.Armor);
        Assert.All(paused.PendingPlayChoice!.Options,
            id => Assert.Equal(GameZone.Removed, result.State.LocationOf(id).Zone));
    }

    [Fact]
    public void SelfAdaptSourceDyingTerminatesRemainingRoundsAndPreventsAfterPlayTrigger()
    {
        var simulator = AdaptDamageSimulator(minionSource: true);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero),
            Card(11, "adapt-source", First));
        var paused = simulator.Apply(root,
            new(SimulationActionKind.PlayCard, First, new EntityId(11), Position: 0)).State;
        Assert.Equal(2, paused.PendingPlayChoice!.RemainingSelections);
        var result = simulator.Apply(paused, AdaptOption(simulator, paused, "adapt-damage"));

        Assert.Null(result.State.PendingPlayChoice);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new EntityId(11)).Zone);
        Assert.DoesNotContain(result.Events, e => e.Kind == SimulationEventKind.PlayChoiceOffered);
        Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Died);
        Assert.DoesNotContain(result.Events, e => e.Kind == SimulationEventKind.CardPlayed);
        Assert.Equal(GameZone.Hand, root.LocationOf(new EntityId(11)).Zone);
    }

    [Fact]
    public void DamageTriggersRunBeforeHonorableKillBonusAndBeforeDeathrattle()
    {
        var source = Minion("finisher", 4, 1) with
        {
            Triggers =
            [
                new(TriggerTiming.Damage, TriggerRelation.Friendly, [new GainArmorEffect(1)])
                {
                    Focus = TriggerFocus.DamageSource, EntityMatch = TriggerEntityMatch.Self
                },
                KillBonus(TriggerTiming.HonorableKill, 2)
            ],
            Deathrattle = [new GainArmorEffect(4)]
        };
        var simulator = new GameSimulator([Hero, source, Minion("defender", 1, 4)]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "finisher", First, 3, 4, 1),
            InPlay(21, "defender", Second, 4, 1, 4));
        var action = new SimulationAction(SimulationActionKind.Attack, First, new EntityId(11), new EntityId(21));
        var result = simulator.Apply(root, action);
        var events = result.Events.ToArray();

        var ordinary = Array.FindIndex(events, e => e.Kind == SimulationEventKind.Triggered &&
            e.Source == new EntityId(11) && e.Timing == TriggerTiming.Damage);
        var bonus = Array.FindIndex(events, e => e.Kind == SimulationEventKind.Triggered &&
            e.Source == new EntityId(11) && e.Timing == TriggerTiming.HonorableKill);
        var death = Array.FindIndex(events, e => e.Kind == SimulationEventKind.Deathrattle &&
            e.Source == new EntityId(11));
        Assert.True(ordinary >= 0 && ordinary < bonus && bonus < death);
        Assert.Equal(7, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new EntityId(11)).Zone);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new EntityId(21)).Zone);
        Assert.Equal<SimulationEvent>(result.Events, simulator.Apply(root, action).Events);
    }
}
