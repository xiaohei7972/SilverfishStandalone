using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void AutomaticSpellQueuesReplacementFromTheDeckTopAndPreservesTheManualCounterWindow()
    {
        var fire = new CardFilter { SpellSchool = SpellSchool.Fire };
        var spell = new SimulationCard(new("automatic", EntityKind.Spell, 8) { SpellSchool = SpellSchool.Fire })
        {
            Keywords = CardKeywords.CastsWhenDrawn,
            OnPlay = [new GainArmorEffect(2)],
            Overload = 1
        };
        var watcher = Minion("watcher", 0, 2) with
        {
            Triggers = [
                new(TriggerTiming.CardDrawn, TriggerRelation.Friendly, [new GainArmorEffect(1)]),
                new(TriggerTiming.AfterSpellResolved, TriggerRelation.Friendly, [new GainArmorEffect(10)]) { CardFilter = fire }
            ]
        };
        var counter = Spell("sentinel") with
        {
            IsSecret = true,
            Triggers = [new(TriggerTiming.SpellCounterWindow, TriggerRelation.Opponent, [new CounterSpellEffect()])]
        };
        var simulator = new GameSimulator([
            Hero, watcher, spell, counter, Spell("replacement"),
            Minion("request", 1, 1) with { OnPlay = [new DrawMatchingEffect(fire)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11]), (GameZone.Deck, [21, 22]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Secret, [29])) },
            InPlay(12, "watcher", First, 3, 0, 2), Card(11, "request", First),
            Card(21, "replacement", First), Card(22, "automatic", First), Card(29, "sentinel", Second) with { PlayOrder = 4 });
        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11), Position: 1));

        Assert.Equal(1, result.State.Players[First].Mana);
        Assert.Equal(1, result.State.Players[First].OverloadOwed);
        Assert.Equal(4, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new(22)).Zone);
        Assert.Equal(GameZone.Secret, result.State.LocationOf(new(29)).Zone);
        Assert.Equal(new EntityId(21), Assert.Single(result.State.Players[First].In(GameZone.Hand)));
        Assert.Equal(new[] { SimulationEventKind.CardPlayed, SimulationEventKind.Summoned,
            SimulationEventKind.CardDrawn, SimulationEventKind.SpellCastWhenDrawn, SimulationEventKind.OverloadScheduled,
            SimulationEventKind.ArmorGained, SimulationEventKind.Triggered, SimulationEventKind.ArmorGained,
            SimulationEventKind.CardDrawn, SimulationEventKind.Triggered, SimulationEventKind.ArmorGained },
            result.Events.Select(entry => entry.Kind));
    }

    [Fact]
    public void BurningAnAutomaticSpellDoesNotConsumeAReplacement()
    {
        var automatic = Spell("automatic") with
        {
            Keywords = CardKeywords.CastsWhenDrawn,
            OnPlay = [new GainArmorEffect(7)]
        };
        var simulator = new GameSimulator([
            Hero, automatic, Spell("plain"), Spell("draw-two") with { OnPlay = [new DrawEffect(2)] }
        ]);
        var hand = Enumerable.Range(31, 9).ToArray();
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, ..hand]), (GameZone.Deck, [21, 22, 23]))
            },
            Player(Second, SecondHero),
            [Card(11, "draw-two", First), Card(21, "plain", First), Card(22, "automatic", First),
             Card(23, "plain", First), ..hand.Select(id => Card(id, "plain", First))]);
        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.Equal(new EntityId(23), Assert.Single(result.State.Players[First].In(GameZone.Deck)));
        Assert.Equal(0, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(new[] { SimulationEventKind.CardPlayed, SimulationEventKind.CardDrawn, SimulationEventKind.CardBurned },
            result.Events.Select(entry => entry.Kind));
        Assert.Equal(new EntityId(22), result.Events[^1].Source);
    }

    [Fact]
    public void ASpellAutomaticallyCastOnTheOpponentsTurnSelectsTargetsRelativeToItsController()
    {
        var automatic = Spell("automatic") with
        {
            Keywords = CardKeywords.CastsWhenDrawn,
            Target = TargetRequirement.FriendlyMinion,
            OnPlay = [new DamageEffect(DamageTarget.Selected, 1)]
        };
        var watcher = Minion("watcher", 0, 3) with
        {
            Triggers = [new(TriggerTiming.AfterSpellResolved, TriggerRelation.Opponent, [new DrawEffect(1)])]
        };
        var simulator = new GameSimulator([Hero, automatic, watcher, Minion("unit", 1, 3), Spell("trigger")]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21, 22]), (GameZone.Deck, [23, 24])) },
            Card(11, "trigger", First), InPlay(12, "unit", First, 3, 1, 3), InPlay(21, "watcher", Second, 4, 0, 3),
            InPlay(22, "unit", Second, 5, 1, 3), Card(23, "automatic", Second), Card(24, "trigger", Second));
        var result = simulator.ApplyDistribution(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.True(result.IsExact);
        Assert.Equal(2, result.Outcomes.Length);
        Assert.Equal(new EntityId?[] { new(21), new(22) }, result.Outcomes.Select(outcome =>
            Assert.Single(outcome.Result.Events, entry => entry.Kind == SimulationEventKind.SpellCastWhenDrawn).Target));
        Assert.All(result.Outcomes, outcome =>
        {
            Assert.Equal(0.5, outcome.Probability);
            Assert.Equal(new EntityId(24), Assert.Single(outcome.Result.State.Players[Second].In(GameZone.Hand)));
            Assert.Equal(3, outcome.Result.State.Entities[new(12)].Character!.Health);
        });
    }

    [Fact]
    public void AChainOfAutomaticSpellsCompletesBeforeTheActionChecksHeroDefeat()
    {
        var harmful = Spell("harmful") with
        {
            Keywords = CardKeywords.CastsWhenDrawn,
            OnPlay = [new DamageEffect(DamageTarget.FriendlyHero, 30)]
        };
        var recovery = Spell("recovery") with
        {
            Keywords = CardKeywords.CastsWhenDrawn,
            OnPlay = [new HealFriendlyHeroEffect(2)]
        };
        var simulator = new GameSimulator([
            Hero, harmful, recovery, Spell("plain"), Spell("draw") with { OnPlay = [new DrawEffect(1)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]), (GameZone.Deck, [21, 22, 23])) },
            Player(Second, SecondHero), Card(11, "draw", First), Card(21, "harmful", First),
            Card(22, "recovery", First), Card(23, "plain", First));
        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(11)));

        Assert.Equal(2, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(MatchOutcome.InProgress, result.State.Outcome);
        Assert.Equal(new EntityId(23), Assert.Single(result.State.Players[First].In(GameZone.Hand)));
        Assert.Equal(new EntityId?[] { new(21), new(22) }, result.Events
            .Where(entry => entry.Kind == SimulationEventKind.SpellCastWhenDrawn).Select(entry => entry.Source));
    }
}
