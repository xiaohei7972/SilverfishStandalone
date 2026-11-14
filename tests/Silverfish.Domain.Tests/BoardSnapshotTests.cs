using System.Collections.Immutable;
using Silverfish.Domain;
using Xunit;

namespace Silverfish.Domain.Tests;

public sealed class BoardSnapshotTests
{
    private static EntityState Fighter(int id = 11) => new()
    {
        Id = new EntityId(id),
        CardId = "EX1_612",
        Kind = EntityKind.Minion,
        Side = PlayerSide.Friendly,
        Zone = EntityZone.Battlefield,
        Position = 1,
        Combat = new CombatStats(4, 3, 3, Ready: true),
        Keywords = ImmutableHashSet.Create("rush", "taunt"),
        EnchantmentCardIds = ImmutableArray.Create("EX1_612e"),
        Counters = ImmutableDictionary<string, int>.Empty.Add("spellburst", 1),
        RawTags = ImmutableDictionary<int, int>.Empty.Add(909090, 3)
    };

    [Fact]
    public void BranchChangeDoesNotMutateOriginalEntityOrNestedMechanics()
    {
        var first = BoardSnapshot.Create(new[] { Fighter() });
        var changed = first.Get(new EntityId(11)) with
        {
            Combat = first.Get(new EntityId(11)).Combat! with { Health = 1 },
            Keywords = first.Get(new EntityId(11)).Keywords.Remove("taunt"),
            Counters = first.Get(new EntityId(11)).Counters.SetItem("spellburst", 0),
            EnchantmentCardIds = ImmutableArray<string>.Empty
        };
        var second = first.Replace(changed);
        Assert.Equal(3, first.Get(new EntityId(11)).Combat!.Health);
        Assert.Equal(1, second.Get(new EntityId(11)).Combat!.Health);
        Assert.Contains("taunt", first.Get(new EntityId(11)).Keywords);
        Assert.DoesNotContain("taunt", second.Get(new EntityId(11)).Keywords);
        Assert.Equal(1, first.Get(new EntityId(11)).Counters["spellburst"]);
        Assert.Equal(0, second.Get(new EntityId(11)).Counters["spellburst"]);
        Assert.Single(first.Get(new EntityId(11)).EnchantmentCardIds);
        Assert.Empty(second.Get(new EntityId(11)).EnchantmentCardIds);
    }

    [Fact]
    public void EntityKeepsIdentityWhenChangingZones()
    {
        var hand = new EntityState
        {
            Id = new EntityId(42), CardId = "EX1_612", Kind = EntityKind.Minion,
            Side = PlayerSide.Friendly, Zone = EntityZone.Hand, Position = 2,
            Cost = new ManaCost(3, 1), HandModifiers = new HandStatModifiers(1, 0)
        };
        var original = BoardSnapshot.Create(new[] { hand });
        var moved = original.Move(hand.Id, EntityZone.Battlefield, 1);
        Assert.Equal(EntityZone.Hand, original.Get(hand.Id).Zone);
        Assert.Equal(EntityZone.Battlefield, moved.Get(hand.Id).Zone);
        Assert.Equal(hand.Id, moved.Get(hand.Id).Id);
    }

    [Fact]
    public void PrintedDefinitionAndCurrentInstanceStatsAreIndependent()
    {
        var definition = new CardDefinition("EX1_612", EntityKind.Minion, 3, 4, 3);
        var entity = Fighter() with { Combat = new CombatStats(8, 2, 3) };
        Assert.Equal(4, definition.PrintedAttack);
        Assert.Equal(8, entity.Combat!.Attack);
    }

    [Fact]
    public void WeaponUsesSameEntityIdentityButDifferentComponent()
    {
        var weapon = new EntityState
        {
            Id = new EntityId(77), CardId = "EX1_411", Kind = EntityKind.Weapon,
            Side = PlayerSide.Friendly, Zone = EntityZone.Weapon,
            Equipment = new WeaponStats(3, 2, 2)
        };
        var board = BoardSnapshot.Create(new[] { Fighter(), weapon });
        Assert.Null(board.Get(weapon.Id).Combat);
        Assert.Equal(2, board.Get(weapon.Id).Equipment!.Durability);
        Assert.Null(board.Get(new EntityId(11)).Equipment);
    }

    [Fact]
    public void DuplicateEntityIdIsNotSilentlyOverwritten()
    {
        Assert.Throws<ArgumentException>(() => BoardSnapshot.Create(new[] { Fighter(), Fighter() }));
    }

    [Fact]
    public void UnknownRawTagSurvivesBranchTransition()
    {
        var first = BoardSnapshot.Create(new[] { Fighter() });
        var next = first.Move(new EntityId(11), EntityZone.SetAside, 0);
        Assert.Equal(3, next.Get(new EntityId(11)).RawTags[909090]);
    }

    [Fact]
    public void InvalidCombatHealthIsRejected()
    {
        var entity = Fighter() with { Combat = new CombatStats(3, 9, 3) };
        Assert.Throws<ArgumentException>(() => BoardSnapshot.Create(new[] { entity }));
    }

    [Fact]
    public void EquippedWeaponWithoutDurabilityIsRejected()
    {
        var entity = Fighter() with
        {
            Kind = EntityKind.Weapon, Zone = EntityZone.Weapon, Combat = null
        };
        Assert.Throws<ArgumentException>(() => BoardSnapshot.Create(new[] { entity }));
    }

    [Fact]
    public void UnrelatedSimulationBranchesRemainIndependent()
    {
        var start = BoardSnapshot.Create(new[] { Fighter(), Fighter(22) });
        var attackBranch = start.Replace(start.Get(new EntityId(11)) with
        {
            Combat = new CombatStats(4, 1, 3)
        });
        var buffBranch = start.Replace(start.Get(new EntityId(11)) with
        {
            Combat = new CombatStats(6, 3, 3)
        });
        Assert.Equal(4, start.Get(new EntityId(11)).Combat!.Attack);
        Assert.Equal(4, attackBranch.Get(new EntityId(11)).Combat!.Attack);
        Assert.Equal(6, buffBranch.Get(new EntityId(11)).Combat!.Attack);
        Assert.Equal(3, start.Get(new EntityId(11)).Combat!.Health);
    }

    [Fact]
    public void InvalidEntityIdIsRejected()
    {
        Assert.Throws<ArgumentException>(() =>
            BoardSnapshot.Create(new[] { Fighter(-1) }));
    }

    [Fact]
    public void RemoveDoesNotMutatePreviousSnapshot()
    {
        var previous = BoardSnapshot.Create(new[] { Fighter() });
        var after = previous.Remove(new EntityId(11));
        Assert.Equal(1, previous.Count);
        Assert.Equal(0, after.Count);
    }
}
