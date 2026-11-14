using HREngine.Bots;
using Silverfish.ClassicModelAdapter;
using Silverfish.Domain;
using Xunit;

namespace Silverfish.ClassicModelAdapter.Tests;

public sealed class LegacyEntityProjectionTests
{
    [Fact]
    public void CharacterSnapshotDoesNotShareMutableEnchantmentOrStatFields()
    {
        var old = new Minion
        {
            entitiyID = 11, own = true, Hp = 3, maxHp = 3,
            Angr = 4, zonepos = 1, Ready = true, taunt = true
        };
        // The latest source stores typed enchantments, while older classic
        // code used a single string. Exercise both layouts without a fork.
        var rawEnchants = typeof(Minion).GetField("enchs")!.GetValue(old);
        var typedEnchants = rawEnchants as IList<CardDB.cardIDEnum>;
        typedEnchants?.Add(CardDB.cardIDEnum.None);
        typeof(Minion).GetField("TAG_SCRIPT_DATA_NUM_1")?.SetValue(old, 42);

        var definition = new CardDefinition("EX1_612", EntityKind.Minion, 3, 4, 3);
        var captured = LegacyEntityProjection.CaptureCharacter(old, definition);
        old.Hp = 1;
        old.taunt = false;
        typedEnchants?.Clear();

        Assert.Equal(3, captured.Combat!.Health);
        Assert.Contains("TAUNT", captured.Keywords);
        if (typedEnchants is not null)
        {
            Assert.Single(captured.EnchantmentCardIds);
            Assert.Equal(42, captured.Counters["scriptData1"]);
        }
        else
        {
            Assert.Empty(captured.EnchantmentCardIds);
            Assert.False(captured.Counters.ContainsKey("scriptData1"));
        }
    }

    [Fact]
    public void HandCardSnapshotRetainsPrintedAndCurrentCostsSeparately()
    {
        var old = new Handmanager.Handcard
        {
            entity = 77, position = 2, manacost = 1,
            addattack = 2, conditionalCount = 3, temporary = true
        };
        old.enchs.Add(CardDB.cardIDEnum.None);
        var definition = new CardDefinition("EX1_612", EntityKind.Minion, 3, 4, 3);
        var captured = LegacyEntityProjection.CaptureHandCard(old, definition);
        old.manacost = 5;
        old.enchs.Clear();

        Assert.Equal(3, captured.Cost!.PrintedCost);
        Assert.Equal(1, captured.Cost!.CurrentCost);
        Assert.Equal(2, captured.HandModifiers!.AttackDelta);
        Assert.Single(captured.EnchantmentCardIds);
        Assert.Equal(3, captured.Counters["conditionalCount"]);
    }

    [Fact]
    public void WeaponRequiresExplicitEntityIdentityAndMaximumDurability()
    {
        var old = new Weapon { Angr = 3, Durability = 2, lifesteal = true };
        var definition = new CardDefinition("CS2_097", EntityKind.Weapon, 4, 3, 0, 2);
        var captured = LegacyEntityProjection.CaptureWeapon(
            old, definition, new EntityId(88), PlayerSide.Friendly, 2);
        old.Durability = 0;

        Assert.Equal(88, captured.Id.Value);
        Assert.Equal(2, captured.Equipment!.Durability);
        Assert.Contains("LIFESTEAL", captured.Keywords);
    }

    [Fact]
    public void MissingHandCardEntityIdFailsRatherThanCreatingSyntheticIdentity()
    {
        var old = new Handmanager.Handcard { entity = -1 };
        var definition = new CardDefinition("EX1_612", EntityKind.Minion, 3);
        Assert.Throws<ArgumentException>(() =>
            LegacyEntityProjection.CaptureHandCard(old, definition));
    }
}
