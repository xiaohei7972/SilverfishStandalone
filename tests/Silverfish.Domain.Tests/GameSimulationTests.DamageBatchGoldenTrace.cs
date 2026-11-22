using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Fact]
    public void MixedArmorShieldAndLethalAreaDamageKeepOrderedReplayTrace()
    {
        var simulator = new GameSimulator([
            Hero,
            Minion("shielded", 0, 2) with { Keywords = CardKeywords.DivineShield },
            Minion("victim", 0, 2),
            Spell("area-strike") with { OnPlay = [new DamageEffect(DamageTarget.AllEnemies, 3)] }
        ]);
        var root = Create(simulator, First,
            Player(First, FirstHero) with
            {
                Zones = Zones((GameZone.Play, [10]), (GameZone.Hand, [11]))
            },
            Player(Second, SecondHero) with
            {
                Zones = Zones((GameZone.Play, [20, 21, 22]))
            },
            Card(11, "area-strike", First),
            InPlay(21, "shielded", Second, 3, 0, 2) with
            {
                Character = new(0, 2, DivineShield: true)
            },
            InPlay(22, "victim", Second, 4, 0, 2));
        var armored = simulator.CreateGame(root.Players.Values,
            root.Entities.Values.Select(entity => entity.Id == SecondHero
                ? entity with { Character = entity.Character! with { Armor = 2 } }
                : entity),
            root.ActivePlayer, root.Turn, root.MinionDeaths, root.RandomState);
        var action = new SimulationAction(SimulationActionKind.PlayCard, First, new EntityId(11));

        var first = simulator.Apply(armored, action);
        var replay = simulator.Apply(armored, action);

        Assert.Equal(new[]
        {
            SimulationEventKind.CardPlayed,
            SimulationEventKind.Damage,
            SimulationEventKind.ShieldBroken,
            SimulationEventKind.Damage,
            SimulationEventKind.Died
        }, first.Events.Select(e => e.Kind));
        var damageEvents = first.Events.Where(e => e.Kind == SimulationEventKind.Damage).ToArray();
        Assert.Equal(new[] { SecondHero, new EntityId(22) }, damageEvents.Select(e => e.Target));
        Assert.Equal(2, damageEvents[0].DamageToArmor);
        Assert.Equal(1, damageEvents[0].DamageToHealth);
        Assert.Equal(0, damageEvents[1].DamageToArmor);
        Assert.Equal(3, damageEvents[1].DamageToHealth);
        Assert.True(damageEvents[1].IsOverkill);
        Assert.Null(damageEvents[1].AttackingWeapon);

        Assert.Equal(29, first.State.Entities[SecondHero].Character!.Health);
        Assert.Equal(0, first.State.Entities[SecondHero].Character!.Armor);
        Assert.Equal(2, first.State.Entities[new EntityId(21)].Character!.Health);
        Assert.False(first.State.Entities[new EntityId(21)].Character!.DivineShield);
        Assert.Equal(GameZone.Graveyard, first.State.LocationOf(new EntityId(22)).Zone);
        Assert.Equal(first.Events, replay.Events);
        Assert.Equal(30, armored.Entities[SecondHero].Character!.Health);
        Assert.Equal(2, armored.Entities[SecondHero].Character!.Armor);
        Assert.True(armored.Entities[new EntityId(21)].Character!.DivineShield);
        Assert.Equal(GameZone.Play, armored.LocationOf(new EntityId(22)).Zone);
    }
}
