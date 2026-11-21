using Silverfish.Domain;
using Silverfish.Simulation;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed partial class GameSimulationTests
{
    [Theory]
    [InlineData(2, 0, 30, 2)]
    [InlineData(4, 2, 27, 3)]
    public void LifestealWeaponHealsAfterBothCombatDamageHits(
        int defenderAttack, int heroDamage, int expectedHealth, int expectedHeal)
    {
        var blade = new SimulationCard(new("combat-lifesteal", EntityKind.Weapon, 0,
            PrintedAttack: 3, PrintedDurability: 2))
        {
            Keywords = CardKeywords.Lifesteal
        };
        var simulator = new GameSimulator([Hero, blade, Minion("defender", defenderAttack, 5)]);
        var initial = Create(simulator, First,
            Player(First, FirstHero) with
            {
                EquippedWeapon = new EntityId(11),
                Zones = Zones((GameZone.Play, [10, 11]))
            },
            Player(Second, SecondHero) with { Zones = Zones((GameZone.Play, [20, 21])) },
            Weapon(11, "combat-lifesteal", First, 3, 3, 2),
            InPlay(21, "defender", Second, 4, defenderAttack, 5));
        var root = simulator.CreateGame(initial.Players.Values,
            initial.Entities.Values.Select(e => e.Id == FirstHero
                ? e with { Character = e.Character! with { Damage = heroDamage } } : e),
            initial.ActivePlayer, initial.Turn, initial.MinionDeaths, initial.RandomState);
        var action = new SimulationAction(SimulationActionKind.Attack, First, FirstHero, new EntityId(21));

        var result = simulator.Apply(root, action);

        Assert.Equal(expectedHealth, result.State.Entities[FirstHero].Character!.Health);
        Assert.Equal(2, result.State.Entities[new EntityId(21)].Character!.Health);
        var heal = Assert.Single(result.Events, e => e.Kind == SimulationEventKind.Healed);
        Assert.Equal(expectedHeal, heal.Amount);
        Assert.Equal(new EntityId(11), heal.Source);
        Assert.Equal(FirstHero, heal.Target);
        Assert.Equal(new[]
        {
            SimulationEventKind.Attack, SimulationEventKind.Damage, SimulationEventKind.Damage,
            SimulationEventKind.Healed, SimulationEventKind.WeaponDurabilityLost
        }, result.Events.Select(e => e.Kind));
        Assert.Equal(result.Events, simulator.Apply(root, action).Events.AsEnumerable());
        Assert.Equal(30 - heroDamage, root.Entities[FirstHero].Character!.Health);
    }
}
