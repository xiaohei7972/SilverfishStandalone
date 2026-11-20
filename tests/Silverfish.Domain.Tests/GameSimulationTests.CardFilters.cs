using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void BothTypesAndAllTypesAreAvailableToTargetsCountsAndStatAuras()
    {
        var beast = new CardFilter { MinionType = MinionType.Beast };
        var dual = new SimulationCard(new("dual", EntityKind.Minion, 0, 1, 4)
        {
            MinionTypes = MinionType.Beast | MinionType.Dragon
        });
        var universal = new SimulationCard(new("universal", EntityKind.Minion, 0, 1, 4) { MinionTypes = MinionType.All });
        var provider = Minion("provider", 0, 2) with
        {
            Auras = [new(AuraRecipients.OtherFriendlyMinions, new(StatModifierMode.Add, Attack: 1)) { CardFilter = beast }]
        };
        var spell = Spell("probe") with
        {
            Target = TargetRequirement.FriendlyMinion,
            TargetFilter = new() { CardFilter = beast },
            OnPlay = [
                new ConditionalEffect(
                    new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Play, NumberComparison.Equal, 2, EntityKind.Minion)
                    {
                        CardFilter = beast
                    },
                    [new GainArmorEffect(2)]),
                new DamageEffect(DamageTarget.Selected, 1)
            ]
        };
        var simulator = new GameSimulator([Hero, dual, universal, provider, Minion("plain", 1, 4), spell]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10, 11, 12, 13, 14]), (GameZone.Hand, [15])) },
            Player(Second, SecondHero), InPlay(11, "dual", First, 3, 1, 4), InPlay(12, "universal", First, 4, 1, 4),
            InPlay(13, "plain", First, 5, 1, 4), InPlay(14, "provider", First, 6, 0, 2), Card(15, "probe", First));

        Assert.Equal(2, root.Entities[new(11)].Character!.Attack);
        Assert.Equal(2, root.Entities[new(12)].Character!.Attack);
        Assert.Equal(1, root.Entities[new(13)].Character!.Attack);
        var targets = simulator.LegalActions(root).Where(action => action.Source == new EntityId(15)).Select(action => action.Target);
        Assert.Equal(new EntityId?[] { new(11), new(12) }, targets);

        var result = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(15), new EntityId(11)));
        Assert.Equal(2, result.State.Entities[FirstHero].Character!.Armor);
        Assert.Equal(3, result.State.Entities[new(11)].Character!.Health);
    }

    [Fact]
    public void ASpellSchoolFilterControlsBothDiscountsAndSpellTriggers()
    {
        var fire = new CardFilter { SpellSchool = SpellSchool.Fire };
        var provider = Minion("provider", 0, 2) with
        {
            CostAuras = [new(CostAuraRecipient.Friendly, CostAuraTarget.Spell, -2) { CardFilter = fire }],
            Triggers = [new(TriggerTiming.AfterSpellResolved, TriggerRelation.Friendly, [new GainArmorEffect(2)])
            {
                CardFilter = fire,
                EntityKind = EntityKind.Spell
            }]
        };
        var simulator = new GameSimulator([Hero, provider,
            new(new("warm", EntityKind.Spell, 3) { SpellSchool = SpellSchool.Fire }),
            new(new("cold", EntityKind.Spell, 2) { SpellSchool = SpellSchool.Frost }),
            new(new("neutral", EntityKind.Spell, 2))]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 2, ManaCrystals = 2,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12, 13, 14])) },
            Player(Second, SecondHero), InPlay(11, "provider", First, 3, 0, 2), Card(12, "warm", First),
            Card(13, "cold", First), Card(14, "neutral", First));
        Assert.Equal(1, simulator.EffectiveCost(root, new(12)));
        Assert.Equal(2, simulator.EffectiveCost(root, new(13)));
        Assert.Equal(2, simulator.EffectiveCost(root, new(14)));

        var warm = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(12)));
        Assert.Equal(1, warm.State.Players[First].Mana);
        Assert.Equal(2, warm.State.Entities[FirstHero].Character!.Armor);
        var cold = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, new EntityId(13)));
        Assert.Equal(0, cold.State.Players[First].Mana);
        Assert.Equal(0, cold.State.Entities[FirstHero].Character!.Armor);
    }
}
