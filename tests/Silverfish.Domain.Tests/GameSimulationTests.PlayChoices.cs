using System.Collections.Immutable;
using System.Text.Json;
using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Theory]
    [InlineData(1, 2)]
    [InlineData(6, 1)]
    public void ZarogsCrownResumesTheChosenMinionAndFinishesTheOriginalSpellOnce(int occupied, int copies)
    {
        var hero = Hero with { Definition = Hero.Definition with { ClassMask = 1U << ((int)TAG_CLASS.MAGE - 1) } };
        var watcher = Minion("watcher", 1, 3) with
        {
            Triggers =
            [
                new(TriggerTiming.BeforeSpellResolved, TriggerRelation.Friendly, [new GainArmorEffect(1)]),
                new(TriggerTiming.AfterSpellResolved, TriggerRelation.Friendly, [new GainArmorEffect(2)])
            ]
        };
        var replacement = ChoiceMinion("mage", TAG_CLASS.MAGE) with
        {
            Triggers = [new(TriggerTiming.AfterSpellResolved, TriggerRelation.Friendly, [new GainArmorEffect(99)])]
        };
        var cards = new[] { hero, watcher, replacement, ChoiceMinion("neutral", TAG_CLASS.NEUTRAL),
            ChoiceMinion("dual", TAG_CLASS.INVALID, classes: (1 << ((int)TAG_CLASS.MAGE - 1)) | (1 << ((int)TAG_CLASS.ROGUE - 1))),
            ChoiceMinion("wrong-class", TAG_CLASS.ROGUE), ChoiceMinion("not-legendary", TAG_CLASS.NEUTRAL, rarity: 1) };
        string[] pool = ["mage", "neutral", "dual", "wrong-class", "not-legendary"];
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, .. cards], randomMinionPool: pool);
        var source = new EntityId(11);
        var filledSlots = Enumerable.Range(30, occupied).ToArray();
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                ManaCrystals = 10, ManaSpent = 0, Zones = Zones((GameZone.Play, [10, .. filledSlots]), (GameZone.Hand, [11]))
            },
            Player(Second, SecondHero),
            [Card(11, "LOOT_998j", First), .. filledSlots.Select((id, slot) =>
                InPlay(id, slot == 0 ? "watcher" : "not-legendary", First, slot + 3, 1, 3))]);

        var play = new SimulationAction(SimulationActionKind.PlayCard, First, source);
        var afterPlay = simulator.Apply(root, play);
        var paused = afterPlay.State;
        var pending = Assert.IsType<PendingPlayChoice>(paused.PendingPlayChoice);
        Assert.Equal(source, pending.Source);
        Assert.Equal(["dual", "mage", "neutral"], pending.Options.Select(id => paused.Entities[id].CardId).Order());
        Assert.All(pending.Options, id => Assert.Equal(GameZone.SetAside, paused.LocationOf(id).Zone));
        Assert.Equal(GameZone.Play, paused.LocationOf(source).Zone);
        Assert.Equal(1, paused.Entities[FirstHero].Character!.Armor);
        Assert.Equal(10 - simulator.Cards["LOOT_998j"].Definition.PrintedCost, paused.Players[First].Mana);
        Assert.Single(afterPlay.Events, entry => entry.Kind == SimulationEventKind.CardPlayed);
        Assert.Equal(3, afterPlay.Events.Count(entry => entry.Kind == SimulationEventKind.PlayChoiceOffered));
        var legal = simulator.LegalActions(paused);
        Assert.Equal(3, legal.Length);
        Assert.All(legal, action => Assert.Equal(SimulationActionKind.ChooseOption, action.Kind));
        var select = Assert.Single(legal, action => paused.Entities[action.Target!.Value].CardId == "mage");
        Assert.NotNull(simulator.CheckAction(paused, new(SimulationActionKind.EndTurn, First)));
        Assert.NotNull(simulator.CheckAction(paused, select with { Actor = Second }));
        Assert.NotNull(simulator.CheckAction(paused, select with { Target = FirstHero }));
        // Internal ChooseAfterPlay options must not accept external EntityChoices fields.
        Assert.NotNull(simulator.CheckAction(paused, select with { ChoicePacketId = 123 }));
        Assert.NotNull(simulator.CheckAction(paused, select with
        {
            OrderedChoiceTargets = [new EntityId(11)]
        }));

        var continuation = new GameContinuation(paused.NextEntity, paused.NextPlayOrder, paused.Outcome, paused.Winner)
        {
            PendingPlayChoice = pending
        };
        var saved = JsonSerializer.Deserialize<GameContinuation>(JsonSerializer.Serialize(continuation))!;
        var fresh = new GameSimulator(simulator.Cards.Values, randomMinionPool: pool);
        var injectedOption = paused.Entities[pending.Options[0]] with { CardId = "wrong-class" };
        Assert.Throws<ArgumentException>(() => fresh.RestoreGame(paused.Players.Values,
            paused.Entities.SetItem(injectedOption.Id, injectedOption).Values, paused.ActivePlayer,
            paused.Turn, paused.MinionDeaths, paused.RandomState, saved));
        var restored = fresh.RestoreGame(paused.Players.Values, paused.Entities.Values, paused.ActivePlayer,
            paused.Turn, paused.MinionDeaths, paused.RandomState, saved);
        var resumed = fresh.Apply(restored, select);
        var selected = simulator.Apply(paused, select);

        Assert.Equal<SimulationEvent>(selected.Events, resumed.Events);
        Assert.Null(resumed.State.PendingPlayChoice);
        Assert.Equal(GameZone.Graveyard, resumed.State.LocationOf(source).Zone);
        Assert.All(pending.Options, id => Assert.Equal(GameZone.Removed, resumed.State.LocationOf(id).Zone));
        var summoned = resumed.State.Players[First].In(GameZone.Play).Where(id => resumed.State.Entities[id].CardId == "mage").ToArray();
        Assert.Equal(copies, summoned.Length);
        Assert.All(summoned, id => Assert.DoesNotContain(id, pending.Options));
        Assert.Equal(3, resumed.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(paused.Players[First].Mana, resumed.State.Players[First].Mana);
        Assert.Single(resumed.Events, entry => entry.Kind == SimulationEventKind.PlayChoiceSelected);
        Assert.DoesNotContain(resumed.Events, entry => entry.Kind == SimulationEventKind.CardPlayed);
        Assert.NotNull(fresh.CheckAction(resumed.State, select));
    }

    private static SimulationCard ChoiceMinion(string id, TAG_CLASS cardClass, int rarity = 5, int classes = 0)
    {
        var tags = ImmutableSortedDictionary<int, int>.Empty
            .Add((int)GAME_TAG.CARDTYPE, (int)TAG_CARDTYPE.MINION)
            .Add((int)GAME_TAG.CLASS, (int)cardClass)
            .Add((int)GAME_TAG.MULTIPLE_CLASSES, classes)
            .Add((int)GAME_TAG.RARITY, rarity)
            .Add((int)GAME_TAG.ATK, 1)
            .Add((int)GAME_TAG.HEALTH, 3);
        return SimulationCard.FromMetadata(new(id, 1, null, null, tags));
    }
}
