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
