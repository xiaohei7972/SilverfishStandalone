using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void PaymentMatchesLegalityWhenHandAndBoardModifiersStack()
    {
        var simulator = new GameSimulator([
            Hero,
            Minion("supplier", 0, 2) with
            {
                CostAuras =
                [
                    new(CostAuraRecipient.Friendly, CostAuraTarget.Hand, -2),
                    new(CostAuraRecipient.Friendly, CostAuraTarget.HeroPower, -1)
                ]
            },
            Minion("toll", 0, 2) with
            {
                CostAuras = [new(CostAuraRecipient.Opponent, CostAuraTarget.Spell, 1)]
            },
            new(new("signal", EntityKind.Spell, 5)),
            new(new("power", EntityKind.HeroPower, 3)),
            new(new("blade", EntityKind.Weapon, 1, 1, PrintedDurability: 2))
        ]);
        EntityId spellId = new(12), powerId = new(13), weaponId = new(14);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Mana = 2, ManaCrystals = 2,
                Zones = Zones((GameZone.Play, [10, 11]), (GameZone.Hand, [12, 14]), (GameZone.HeroPower, [13]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            InPlay(11, "supplier", First, 3, 0, 2),
            Card(12, "signal", First) with { CostModifiers = [new(FirstHero, -2)] },
            Card(13, "power", First), Card(14, "blade", First),
            InPlay(21, "toll", Second, 4, 0, 2));

        Assert.Equal(2, simulator.EffectiveCost(root, spellId));
        Assert.Equal(2, simulator.EffectiveCost(root, powerId));
        Assert.Equal(0, simulator.EffectiveCost(root, weaponId));
        var actions = simulator.LegalActions(root);
        var cast = Assert.Single(actions, action => action.Source == spellId);
        var usePower = Assert.Single(actions, action => action.Source == powerId);

        var castState = simulator.Apply(root, cast).State;
        var powerState = simulator.Apply(root, usePower).State;
        Assert.Equal(2, castState.Players[First].ManaSpent);
        Assert.Empty(castState.Entities[spellId].CostModifiers);
        Assert.Equal(2, powerState.Players[First].ManaSpent);
        Assert.Equal(0, root.Players[First].ManaSpent);
    }

    [Theory]
    [InlineData("silence")]
    [InlineData("return")]
    [InlineData("control")]
    public void CostChangesWhenTheSourceStopsAffectingItsOpponent(string operation)
    {
        CardEffect removal = operation switch
        {
            "silence" => new SilenceEffect(),
            "return" => new ReturnMinionEffect(),
            "control" => new TakeControlEffect(),
            _ => throw new ArgumentException("无法识别测试动作。", nameof(operation))
        };
        var simulator = new GameSimulator([
            Hero,
            Minion("supplier", 0, 2) with
            {
                CostAuras = [new(CostAuraRecipient.Opponent, CostAuraTarget.Spell, -2)]
            },
            Spell("remove") with { Target = TargetRequirement.EnemyMinion, OnPlay = [removal] },
            new(new("signal", EntityKind.Spell, 2))
        ]);
        EntityId spellId = new(12);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Mana = 0, Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Card(11, "remove", First), Card(12, "signal", First), InPlay(21, "supplier", Second, 3, 0, 2));
        var action = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(11), new EntityId(21));

        Assert.Contains(simulator.LegalActions(root), item => item.Source == spellId);
        var result = simulator.Apply(root, action).State;
        Assert.Equal(2, simulator.EffectiveCost(result, spellId));
        Assert.DoesNotContain(simulator.LegalActions(result), item => item.Source == spellId);
        Assert.Equal(0, simulator.EffectiveCost(root, spellId));
    }

    [Fact]
    public void AWeaponOnlyDiscountsOtherCardsAfterItIsEquipped()
    {
        var simulator = new GameSimulator([
            Hero, new(new("signal", EntityKind.Spell, 1)),
            new SimulationCard(new("blade", EntityKind.Weapon, 1, 1, PrintedDurability: 1))
            {
                CostAuras = [new(CostAuraRecipient.BothPlayers, CostAuraTarget.Hand, -1)]
            }
        ]);
        EntityId weaponId = new(11), spellId = new(12), enemySpellId = new(21);
        var root = Create(simulator, First,
            Player(First, FirstHero) with { Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11, 12])) },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20]), (GameZone.Hand, [21])) },
            Card(11, "blade", First), Card(12, "signal", First), Card(21, "signal", Second));
        Assert.Equal(1, simulator.EffectiveCost(root, weaponId));
        var equipped = simulator.Apply(root, new(SimulationActionKind.PlayCard, First, weaponId)).State;
        Assert.Equal(0, equipped.Players[First].Mana);
        Assert.Equal(0, simulator.EffectiveCost(equipped, enemySpellId));
        Assert.Contains(simulator.LegalActions(equipped), action => action.Source == spellId);

        var broken = simulator.Apply(equipped, new(SimulationActionKind.Attack, First, FirstHero, SecondHero)).State;
        Assert.Equal(1, simulator.EffectiveCost(broken, spellId));
        Assert.Equal(1, simulator.EffectiveCost(broken, enemySpellId));
        Assert.DoesNotContain(simulator.LegalActions(broken), action => action.Source == spellId);
    }
}
