using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void SilencingAnAuraEmitterUpdatesHealthBeforeTheNextSpellEffect()
    {
        AssertAuraRecipientDiesWithinSameSpell(new SilenceEffect(), enemySource: false);
    }

    [Fact]
    public void TransformingAnAuraEmitterUpdatesHealthBeforeTheNextSpellEffect()
    {
        AssertAuraRecipientDiesWithinSameSpell(new TransformMinionEffect("replacement"), enemySource: false);
    }

    [Fact]
    public void ReturningAnAuraEmitterUpdatesHealthBeforeTheNextSpellEffect()
    {
        AssertAuraRecipientDiesWithinSameSpell(new ReturnMinionEffect(), enemySource: true);
    }

    [Fact]
    public void StealingAnAuraEmitterUpdatesHealthBeforeTheNextSpellEffect()
    {
        AssertAuraRecipientDiesWithinSameSpell(new TakeControlEffect(), enemySource: true);
    }


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ImmediateAuraEmitterDeathUpdatesHealthBeforeFollowingDamage(bool fullHand)
    {
        // Start with six board slots so the steal is legal; the spell itself
        // summons slot seven before attempting the control change.
        var boardFillers = Enumerable.Range(30, 6).ToArray();
        var handFillers = Enumerable.Range(40, 10).ToArray();
        var simulator = new GameSimulator([
            Hero, Minion("banner", 0, 4) with
            {
                Auras = [new(AuraRecipients.OtherFriendlyMinions,
                    new(StatModifierMode.Add, MaximumHealth: 2))]
            },
            Minion("recipient", 0, 1), Minion("filler", 0, 3),
            Spell("remove-then-damage") with
            {
                Target = TargetRequirement.EnemyMinion,
                OnPlay = [
                    .. (fullHand ? new CardEffect[] { new ReturnMinionEffect() } :
                        new CardEffect[] { new SummonEffect("filler"), new TakeControlEffect() }),
                    new DamageEffect(DamageTarget.EnemyMinions, 2)
                ]
            }
        ]);
        var first = Player(First, FirstHero) with
        {
            Zones = Zones((GameZone.Play, fullHand ? [10] : [10, .. boardFillers]),
                (GameZone.Hand, [13]))
        };
        var second = Player(Second, SecondHero) with
        {
            Zones = Zones((GameZone.Play, [20, 21, 22]),
                (GameZone.Hand, fullHand ? handFillers : []))
        };
        var root = Create(simulator, First, first, second,
        [
            Card(13, "remove-then-damage", First),
            InPlay(21, "banner", Second, 3, 0, 4),
            InPlay(22, "recipient", Second, 4, 0, 1),
            .. (fullHand ? [] : boardFillers.Select(id => InPlay(id, "filler", First, id, 0, 3))),
            .. (fullHand ? handFillers.Select(id => Card(id, "filler", Second)) : [])
        ]);
        var action = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(13), new EntityId(21));

        Assert.Equal(3, root.Entities[new EntityId(22)].Character!.Health);
        var result = simulator.Apply(root, action);

        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new EntityId(21)).Zone);
        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(new EntityId(22)).Zone);
        Assert.Equal(new EntityId?[] { new(21), new(22) },
            result.Events.Where(e => e.Kind == SimulationEventKind.Died).Select(e => e.Source));
        Assert.Equal(result.Events, simulator.Apply(root, action).Events.AsEnumerable());
        Assert.Equal(GameZone.Play, root.LocationOf(new EntityId(21)).Zone);
    }

    private static void AssertAuraRecipientDiesWithinSameSpell(CardEffect transition, bool enemySource)
    {
        var aura = new StatAura(AuraRecipients.OtherFriendlyMinions,
            new(StatModifierMode.Add, MaximumHealth: 2));
        var simulator = new GameSimulator([
            Hero,
            Minion("banner", 0, 4) with { Auras = [aura] },
            Minion("recipient", 0, 1),
            Minion("replacement", 0, 4),
            Spell("transition-then-damage") with
            {
                Target = enemySource ? TargetRequirement.EnemyMinion : TargetRequirement.Minion,
                OnPlay = [transition, new DamageEffect(
                    enemySource ? DamageTarget.EnemyMinions : DamageTarget.AllMinions, 2)]
            }
        ]);
        var banner = new EntityId(enemySource ? 21 : 11);
        var recipient = new EntityId(enemySource ? 22 : 12);
        var source = new EntityId(13);
        var root = enemySource
            ? Create(simulator, First,
                Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [13])) },
                Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21, 22])) },
                Card(13, "transition-then-damage", First),
                InPlay(21, "banner", Second, 3, 0, 4),
                InPlay(22, "recipient", Second, 4, 0, 1))
            : Create(simulator, First,
                Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11, 12]), (GameZone.Hand, [13])) },
                Player(Second, SecondHero),
                InPlay(11, "banner", First, 3, 0, 4),
                InPlay(12, "recipient", First, 4, 0, 1),
                Card(13, "transition-then-damage", First));

        Assert.Equal(3, root.Entities[recipient].Character!.Health);
        var action = new SimulationAction(SimulationActionKind.PlayCard, First, source, banner);
        var result = simulator.Apply(root, action);

        Assert.Equal(GameZone.Graveyard, result.State.LocationOf(recipient).Zone);
        Assert.Contains(result.Events, e => e.Kind == SimulationEventKind.Died && e.Source == recipient);
        Assert.Equal(3, root.Entities[recipient].Character!.Health);
        Assert.Equal(GameZone.Play, root.LocationOf(recipient).Zone);
        Assert.Equal(result.Events, simulator.Apply(root, action).Events.AsEnumerable());
    }
}
