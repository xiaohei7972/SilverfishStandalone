using System.Text;
using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Theory]
    [InlineData("REV_793")]
    [InlineData("REV_990")]
    public void SanguineDepthsUsesTheSameActivationAndEnchantmentForBothIds(string cardId)
    {
        var simulator = new GameSimulator([.. StarterCardCatalog.Cards, Hero, Minion("fragile", 1, 1), Minion("unit", 2, 4)]);
        var source = new EntityId(11);
        var friendly = new EntityId(12);
        var enemy = new EntityId(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                ManaCrystals = 2, ManaSpent = 0, Zones = Zones((GameZone.Play, [10, 12]), (GameZone.Hand, [11, 13]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, cardId, First), InPlay(12, "fragile", First, 3, 1, 1),
            Card(13, "EX1_332", First), InPlay(21, "unit", Second, 4, 2, 4));
        var play = new SimulationAction(SimulationActionKind.PlayCard, First, source, Position: 0);
        Assert.Contains(play, simulator.LegalActions(root));
        var placed = simulator.Apply(root, play);
        Assert.Equal(new LocationState(3), placed.State.Entities[source].Location);
        Assert.Equal(SimulationEventKind.CardPlayed, Assert.Single(placed.Events).Kind);

        var activate = new SimulationAction(SimulationActionKind.ActivateLocation, First, source, enemy);
        Assert.Equal(new EntityId?[] { friendly, enemy }, Assert.Single(simulator.ExplainSource(placed.State, source).Branches).Targets);
        Assert.Contains(activate, simulator.LegalActions(placed.State));
        Assert.Equal(ErrorType.REQ_MINION_TARGET, simulator.CheckAction(placed.State, activate with { Target = FirstHero })?.Type);
        Assert.Equal(ErrorType.REQ_TARGET_TO_PLAY, simulator.CheckAction(placed.State, activate with { Target = null })?.Type);

        var activated = simulator.Apply(placed.State, activate);
        Assert.Equal(new[] { SimulationEventKind.LocationActivated, SimulationEventKind.Damage,
            SimulationEventKind.EnchantmentApplied, SimulationEventKind.LocationHealthLost }, activated.Events.Select(entry => entry.Kind));
        var target = activated.State.Entities[enemy];
        Assert.Equal(3, target.Character!.Health);
        Assert.Equal(4, target.Character.Attack);
        var enchantment = Assert.Single(target.Enchantments);
        Assert.Equal("REV_990e", enchantment.CardId);
        Assert.Equal(source, enchantment.Source);
        Assert.Equal(new LocationState(2, 2), activated.State.Entities[source].Location);
        Assert.Equal(ErrorType.REQ_NOT_EXHAUSTED_LOCATION, simulator.CheckAction(activated.State, activate)?.Type);

        var silenced = simulator.Apply(activated.State, new(SimulationActionKind.PlayCard, First, new EntityId(13), enemy)).State.Entities[enemy];
        Assert.Empty(silenced.Enchantments);
        Assert.Equal(2, silenced.Character!.Attack);
        Assert.Equal(3, silenced.Character.Health);

        var lethal = simulator.Apply(placed.State, activate with { Target = friendly });
        Assert.Equal(GameZone.Graveyard, lethal.State.LocationOf(friendly).Zone);
        Assert.Equal(friendly, Assert.Single(lethal.State.MinionDeaths).Entity);
    }

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
            PlayReqs = new(default, [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)]),
            Activation = new([new DamageEffect(DamageTarget.Selected, 3)])
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
