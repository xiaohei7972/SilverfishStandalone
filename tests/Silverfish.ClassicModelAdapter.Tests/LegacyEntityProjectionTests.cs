using HREngine.Bots;
using Silverfish.ClassicModelAdapter;
using Silverfish.Domain;
using Xunit;

namespace Silverfish.ClassicModelAdapter.Tests;

public sealed class LegacyEntityProjectionTests
{
    [Fact]
    public void PlayfieldCopyIsolatesMinionAndHandCardStateFromSiblingBranches()
    {
        Settings.Instance.setFilePath(AppContext.BaseDirectory);
        var fixture = new BoardTester(
            """
            水晶： 1 / 1 [我方英雄] 法师 生命值:( 30 + 0 奥秘数: 0 ) [敌方英雄] 法师 生命值:( 3 + 0 奥秘数: 0 )
            [我方场面] 肯瑞托法师 ( 4 / 3 )
            Enemy cards: 0
            Enemy deck: 0
            """);
        Assert.True(fixture.datareaded);
        var original = new Playfield();
        var minion = Assert.Single(original.ownMinions);
        minion.handcard.extraParam2 = 17;
        minion.handcard.enchs.Add(CardDB.cardIDEnum.EX1_612);
        var originalEnchantments = MinionEnchantments(minion);
        originalEnchantments?.Add(CardDB.cardIDEnum.EX1_612);
        var handCard = new Handmanager.Handcard { entity = 77, manacost = 2 };
        handCard.enchs.Add(CardDB.cardIDEnum.EX1_612);
        handCard.conditionalList.Add(CardDB.cardIDEnum.EX1_612);
        original.owncards.Add(handCard);

        var left = new Playfield(original);
        var right = new Playfield(original);
        var leftMinion = Assert.Single(left.ownMinions);
        Assert.Equal(17, leftMinion.handcard.extraParam2);
        Assert.Equal(CardDB.cardIDEnum.EX1_612, Assert.Single(leftMinion.handcard.enchs));
        leftMinion.handcard.extraParam2 = 99;
        leftMinion.handcard.enchs.Clear();
        if (originalEnchantments is not null)
        {
            var enchantments = Assert.IsAssignableFrom<IList<CardDB.cardIDEnum>>(MinionEnchantments(leftMinion));
            Assert.Equal(CardDB.cardIDEnum.EX1_612, Assert.Single(enchantments));
            enchantments.Clear();
        }
        var leftHandCard = Assert.Single(left.owncards);
        Assert.Equal(CardDB.cardIDEnum.EX1_612, Assert.Single(leftHandCard.enchs));
        Assert.Equal(CardDB.cardIDEnum.EX1_612, Assert.Single(leftHandCard.conditionalList));
        leftHandCard.enchs.Clear();
        leftHandCard.conditionalList.Clear();

        foreach (var untouched in new[] { original, right })
        {
            var untouchedMinion = Assert.Single(untouched.ownMinions);
            Assert.Equal(17, untouchedMinion.handcard.extraParam2);
            Assert.Equal(CardDB.cardIDEnum.EX1_612, Assert.Single(untouchedMinion.handcard.enchs));
            if (originalEnchantments is not null)
            {
                var enchantments = Assert.IsAssignableFrom<IList<CardDB.cardIDEnum>>(MinionEnchantments(untouchedMinion));
                Assert.Equal(CardDB.cardIDEnum.EX1_612, Assert.Single(enchantments));
            }
            var untouchedHandCard = Assert.Single(untouched.owncards);
            Assert.Equal(CardDB.cardIDEnum.EX1_612, Assert.Single(untouchedHandCard.enchs));
            Assert.Equal(CardDB.cardIDEnum.EX1_612, Assert.Single(untouchedHandCard.conditionalList));
        }
    }

    [Fact]
    public void MinionTransformPreservesDestinationIdentityAndCopiesMutableState()
    {
        var source = new Minion { entitiyID = 11 };
        source.handcard.extraParam2 = 17;
        source.handcard.enchs.Add(CardDB.cardIDEnum.EX1_612);
        MinionEnchantments(source)?.Add(CardDB.cardIDEnum.EX1_612);

        var transformed = new Minion { entitiyID = 22 };
        transformed.setMinionToMinion(source);
        Assert.Equal(22, transformed.entitiyID);
        Assert.Equal(17, transformed.handcard.extraParam2);
        Assert.Equal(CardDB.cardIDEnum.EX1_612, Assert.Single(transformed.handcard.enchs));
        transformed.handcard.enchs.Clear();
        MinionEnchantments(transformed)?.Clear();

        Assert.Equal(CardDB.cardIDEnum.EX1_612, Assert.Single(source.handcard.enchs));
        if (MinionEnchantments(source) is { } enchantments)
            Assert.Equal(CardDB.cardIDEnum.EX1_612, Assert.Single(enchantments));
    }

    [Fact]
    public void HandCardReplacementCopiesListsAndRetainsDestinationIdentity()
    {
        var source = new Handmanager.Handcard { entity = 11, manacost = 3 };
        source.enchs.Add(CardDB.cardIDEnum.EX1_612);
        source.conditionalList.Add(CardDB.cardIDEnum.EX1_612);

        var replacement = new Handmanager.Handcard { entity = 22, position = 2 };
        replacement.setHCtoHC(source);
        Assert.Equal(22, replacement.entity);
        Assert.Equal(2, replacement.position);
        Assert.Equal(3, replacement.manacost);
        Assert.Equal(CardDB.cardIDEnum.EX1_612, Assert.Single(replacement.enchs));
        Assert.Equal(CardDB.cardIDEnum.EX1_612, Assert.Single(replacement.conditionalList));
        replacement.enchs.Clear();
        replacement.conditionalList.Clear();

        Assert.Equal(CardDB.cardIDEnum.EX1_612, Assert.Single(source.enchs));
        Assert.Equal(CardDB.cardIDEnum.EX1_612, Assert.Single(source.conditionalList));
    }

    private static IList<CardDB.cardIDEnum>? MinionEnchantments(Minion minion) =>
        typeof(Minion).GetField("enchs")?.GetValue(minion) as IList<CardDB.cardIDEnum>;

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
        var typedEnchants = MinionEnchantments(old);
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
