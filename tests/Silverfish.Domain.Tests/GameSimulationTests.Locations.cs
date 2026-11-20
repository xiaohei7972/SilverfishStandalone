using System.Text;
using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void LocationUsesHealthFromCardDataAndSkipsOneOwnerTurnAfterActivation()
    {
        using var xml = new MemoryStream(Encoding.UTF8.GetBytes("""
            <CardDefs build="1">
              <Entity CardID="training-site" ID="1">
                <Tag enumID="202" value="39" />
                <Tag enumID="48" value="1" />
                <Tag enumID="45" value="2" />
                <Tag enumID="187" value="5" />
              </Entity>
            </CardDefs>
            """));
        var metadata = CardDataCatalog.ImportXml(xml).GetCard("training-site");
        var site = SimulationCard.FromMetadata(metadata, new CardRules
        {
            Activation = new([new DamageEffect(DamageTarget.Selected, 3)]) { Target = TargetRequirement.EnemyCharacter }
        });
        var simulator = new GameSimulator([Hero, site]);
        var source = new EntityId(11);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11])) },
            Player(Second, SecondHero), Card(11, "training-site", First));
        var play = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, source, Position: 0));

        Assert.Null(root.Entities[source].Location);
        Assert.Equal(new LocationState(2), play.State.Entities[source].Location);
        Assert.Equal(0, play.State.Players[First].Mana);
        Assert.Equal(30, play.State.Entities[SecondHero].Character!.Health);
        Assert.Equal(SimulationEventKind.CardPlayed, Assert.Single(play.Events).Kind);

        var activation = new SimulationAction(SimulationActionKind.ActivateLocation, First, source, SecondHero);
        Assert.Equal(activation, Assert.Single(simulator.LegalActions(play.State), action => action.Source == source));
        var firstUse = simulator.Apply(play.State, activation);
        Assert.Equal(new LocationState(1, 2), firstUse.State.Entities[source].Location);
        Assert.Equal(0, firstUse.State.Players[First].Mana);
        Assert.Equal(27, firstUse.State.Entities[SecondHero].Character!.Health);
        Assert.DoesNotContain(activation, simulator.LegalActions(firstUse.State));
        Assert.Equal(new[] { SimulationEventKind.LocationActivated, SimulationEventKind.Damage,
            SimulationEventKind.LocationHealthLost }, firstUse.Events.Select(entry => entry.Kind));

        var saved = new GameContinuation(firstUse.State.NextEntity, firstUse.State.NextPlayOrder, MatchOutcome.InProgress, null);
        var state = Restore(simulator, firstUse.State, saved);
        state = simulator.Apply(state, new(SimulationActionKind.EndTurn, First)).State;
        Assert.Equal(new LocationState(1, 2), state.Entities[source].Location);
        state = simulator.Apply(state, new(SimulationActionKind.EndTurn, Second)).State;
        Assert.Equal(new LocationState(1, 1), state.Entities[source].Location);
        Assert.DoesNotContain(activation, simulator.LegalActions(state));
        state = simulator.Apply(state, new(SimulationActionKind.EndTurn, First)).State;
        state = simulator.Apply(state, new(SimulationActionKind.EndTurn, Second)).State;
        Assert.Contains(activation, simulator.LegalActions(state));

        var finalUse = simulator.Apply(state, activation);
        Assert.Equal(GameZone.Graveyard, finalUse.State.LocationOf(source).Zone);
        Assert.Equal(new LocationState(0), finalUse.State.Entities[source].Location);
        Assert.Equal(SimulationEventKind.LocationDestroyed, finalUse.Events[^1].Kind);
        Assert.Empty(finalUse.State.MinionDeaths);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void LocationOccupiesABoardSlotAndCanFreeItBeforeItsFinalSummon(int uses)
    {
        var site = new SimulationCard(new("workshop", EntityKind.Location, 0, PrintedDurability: 2))
        {
            Activation = new([new SummonEffect("unit")]) { RemoveBeforeFinalEffects = true }
        };
        var lead = Minion("leader", 1, 3) with
        {
            Auras = [new(AuraRecipients.AdjacentFriendlyMinions, new(StatModifierMode.Add, Attack: 2))]
        };
        var simulator = new GameSimulator([Hero, site, lead, Minion("unit", 1, 2)]);
        var source = new EntityId(12);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11, 12, 13, 14, 15, 16, 17]), (GameZone.Hand, [18])) },
            Player(Second, SecondHero), InPlay(11, "leader", First, 3, 1, 3),
            Card(12, "workshop", First) with { PlayOrder = 4, Location = new(uses) },
            InPlay(13, "unit", First, 5, 1, 2), InPlay(14, "unit", First, 6, 1, 2),
            InPlay(15, "unit", First, 7, 1, 2), InPlay(16, "unit", First, 8, 1, 2),
            InPlay(17, "unit", First, 9, 1, 2), Card(18, "unit", First));
        Assert.Equal(1, root.Entities[new(13)].Character!.Attack);
        Assert.DoesNotContain(simulator.LegalActions(root), action => action.Kind == SimulationActionKind.PlayCard);

        var result = simulator.Apply(root, new(SimulationActionKind.ActivateLocation, First, source));
        Assert.Equal(root.Players[First].In(GameZone.Play).Length, result.State.Players[First].In(GameZone.Play).Length);
        if (uses == 1)
        {
            Assert.Equal(new[] { SimulationEventKind.LocationActivated, SimulationEventKind.LocationHealthLost,
                SimulationEventKind.LocationDestroyed, SimulationEventKind.Summoned }, result.Events.Select(entry => entry.Kind));
            Assert.Equal(3, result.State.Entities[new(13)].Character!.Attack);
        }
        else
        {
            Assert.Equal(GameZone.Play, result.State.LocationOf(source).Zone);
            Assert.DoesNotContain(result.Events, entry => entry.Kind == SimulationEventKind.Summoned);
            Assert.Equal(1, result.State.Entities[new(13)].Character!.Attack);
        }
    }
}
