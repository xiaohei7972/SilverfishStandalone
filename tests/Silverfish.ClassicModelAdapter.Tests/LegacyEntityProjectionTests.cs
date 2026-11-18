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
        var original = CreatePlayfield();
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

    [Fact]
    public void ModularHandCardFromBoardLogKeepsThePrintedCardUntouched()
    {
        Settings.Instance.setFilePath(AppContext.BaseDirectory);
        var printed = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_612);
        var component = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_172);
        var printedCost = printed.cost;
        var printedAttack = printed.Attack;
        var unknownType = CardDB.Instance.unknownCard.type;
        Assert.Equal(CardDB.cardIDEnum.CS2_172, component.cardIDenum);
        Assert.True(int.TryParse(component.dbfId, out var componentId) && componentId > 0);

        var lines = Enumerable.Repeat("####", 20).Append("Own Handcards:")
            .Append($" 1 modular {printedCost} entity 77 EX1_612 0 0 0 {componentId} {componentId} 0")
            .Append($" 2 printed {printedCost} entity 78 EX1_612 0 0 0 0 0 0");
        Assert.True(new BoardTester(string.Join('\n', lines)).datareaded);
        var hand = Handmanager.Instance.handCards;
        var modular = Assert.Single(hand, card => card.entity == 77);
        var ordinary = Assert.Single(hand, card => card.entity == 78);

        Assert.Equal(component.cost * 2, modular.card.cost);
        Assert.Equal(component.Attack * 2, modular.card.Attack);
        Assert.Equal(printedCost, ordinary.card.cost);
        Assert.Equal(printedAttack, ordinary.card.Attack);
        Assert.Equal(printedCost, printed.cost);
        Assert.Equal(printedAttack, printed.Attack);
        Assert.Equal(0, printed.MODULAR_ENTITY_PART_1);
        Assert.Equal(0, printed.MODULAR_ENTITY_PART_2);
        Assert.Equal(unknownType, CardDB.Instance.unknownCard.type);
    }

    [Fact]
    public void DiyHandCardCompositionDoesNotMutateTheSiblingCard()
    {
        // The November import added an entry point on Handcard itself.
        var compose = typeof(Handmanager.Handcard).GetMethod("getDIYCard");
        if (compose is null)
            return;

        var printed = new CardDB.Card { cost = 3, Attack = 4, Health = 3 };
        var edited = new Handmanager.Handcard(printed);
        var sibling = new Handmanager.Handcard(edited);
        var part = new CardDB.Card { cost = 2, Attack = 1, Health = 5 };
        compose.Invoke(edited, new object[] { part, part });

        Assert.Equal(4, edited.card.cost);
        Assert.Equal(2, edited.card.Attack);
        Assert.Equal(10, edited.card.Health);
        Assert.Same(printed, sibling.card);
        Assert.Equal(3, sibling.card.cost);
        Assert.Equal(4, sibling.card.Attack);
        Assert.Equal(3, sibling.card.Health);
    }

    [Fact]
    public void PlayfieldCopiesWeaponStateAndDetachesTheEnchantmentGraph()
    {
        var original = CreatePlayfield();
        var weapon = original.ownWeapon;
        weapon.pID = 7;
        weapon.Overkill = true;
        weapon.HonorableKill = true;
        var effect = new Entity { Id = 51, CardId = "EX1_612" };
        var attachment = new Entity { Id = 52 };
        effect.Tags[GAME_TAG.TAG_SCRIPT_DATA_NUM_1] = 7;
        effect.Attachments.Add(attachment);
        attachment.Attachments.Add(effect);
        weapon.enchants.Add(new miniEnch(CardDB.cardIDEnum.EX1_612, 11, 1, 2, effect));
        weapon.enchants.Add(new miniEnch(CardDB.cardIDEnum.EX1_612, 12, 1, 0, effect));

        var left = new Playfield(original).ownWeapon;
        var right = new Playfield(original).ownWeapon;
        Assert.Equal(7, left.pID);
        Assert.True(left.Overkill);
        Assert.True(left.HonorableKill);
        Assert.Equal(2, left.enchants.Count);
        var copied = left.enchants[0];
        Assert.Equal(CardDB.cardIDEnum.EX1_612, copied.CARDID);
        Assert.Equal(11, copied.creator);
        Assert.Equal(1, copied.controllerOfCreator);
        Assert.Equal(2, copied.copyDeathrattle);
        Assert.Equal("EX1_612", copied.entity.CardId);
        Assert.Equal(7, copied.entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1));
        Assert.Same(copied.entity, left.enchants[1].entity);
        Assert.Same(copied.entity, Assert.Single(Assert.Single(copied.entity.Attachments).Attachments));

        copied.creator = 15;
        copied.entity.Tags[GAME_TAG.TAG_SCRIPT_DATA_NUM_1] = 99;
        copied.entity.Attachments.Clear();
        left.enchants.Clear();

        foreach (var untouched in new[] { weapon, right })
        {
            Assert.Equal(2, untouched.enchants.Count);
            Assert.Equal(11, untouched.enchants[0].creator);
            Assert.Equal(7, untouched.enchants[0].entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1));
            Assert.Equal(52, Assert.Single(untouched.enchants[0].entity.Attachments).Id);
        }
    }

    [Fact]
    public void WeaponScriptCountersStayOnTheAttackingBranch()
    {
        var parent = CreatePlayfield();
        var definition = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.WW_010t);
        Assert.Equal(CardDB.cardIDEnum.WW_010t, definition.cardIDenum);
        var firstCounter = definition.TAG_SCRIPT_DATA_NUM_1;
        var secondCounter = definition.TAG_SCRIPT_DATA_NUM_2;
        parent.ownWeapon.equip(definition);
        var attacking = new Playfield(parent);
        var waiting = new Playfield(parent);

        for (var attack = 0; attack < 2; attack++)
            definition.sim_card.afterHeroattack(attacking, attacking.ownHero, attacking.enemyHero);

        Assert.Equal(parent.ownMinions.Count + 2, attacking.ownMinions.Count);
        Assert.Equal(firstCounter + 2, attacking.ownWeapon.card.TAG_SCRIPT_DATA_NUM_1);
        Assert.Equal(secondCounter + 2, attacking.ownWeapon.card.TAG_SCRIPT_DATA_NUM_2);
        Assert.Equal(firstCounter, waiting.ownWeapon.card.TAG_SCRIPT_DATA_NUM_1);
        Assert.Equal(secondCounter, waiting.ownWeapon.card.TAG_SCRIPT_DATA_NUM_2);
        Assert.Equal(firstCounter, parent.ownWeapon.card.TAG_SCRIPT_DATA_NUM_1);
        Assert.Equal(secondCounter, parent.ownWeapon.card.TAG_SCRIPT_DATA_NUM_2);
        Assert.Equal(firstCounter, definition.TAG_SCRIPT_DATA_NUM_1);
        Assert.Equal(secondCounter, definition.TAG_SCRIPT_DATA_NUM_2);
    }

    [Fact]
    public void ConditionalLifestealDoesNotRewriteThePrintedSpell()
    {
        var playfield = CreatePlayfield(mana: 10);
        var definition = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.RLK_209);
        Assert.Equal(CardDB.cardIDEnum.RLK_209, definition.cardIDenum);
        Assert.False(definition.lifesteal);
        var cast = new Handmanager.Handcard(definition);
        var unplayed = new Handmanager.Handcard(cast);
        var startingHealth = playfield.enemyHero.Hp;

        definition.sim_card.onCardPlay(playfield, true, null, 0, cast);

        Assert.True(cast.card.lifesteal);
        Assert.False(unplayed.card.lifesteal);
        Assert.False(definition.lifesteal);
        Assert.True(playfield.enemyHero.Hp < startingHealth);
    }

    [Fact]
    public void ForgeOfWillsCreatesDifferentGiantsWithoutChangingTheirTemplate()
    {
        var parent = CreatePlayfield();
        var forge = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_465);
        var giant = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_465t);
        Assert.Equal(CardDB.cardIDEnum.TTN_465, forge.cardIDenum);
        Assert.Equal(CardDB.cardIDEnum.TTN_465t, giant.cardIDenum);
        var printed = (giant.Attack, giant.Health, giant.dormant);
        var left = new Playfield(parent);
        var right = new Playfield(parent);
        var firstTarget = Assert.Single(left.ownMinions);
        var secondTarget = Assert.Single(right.ownMinions);
        secondTarget.Angr = 6;
        secondTarget.Hp = 2;

        forge.sim_card.useLocation(left, null, firstTarget);
        forge.sim_card.useLocation(right, null, secondTarget);

        var firstGiant = Assert.Single(left.ownMinions, m => m.handcard.card.cardIDenum == giant.cardIDenum);
        var secondGiant = Assert.Single(right.ownMinions, m => m.handcard.card.cardIDenum == giant.cardIDenum);
        Assert.Equal((4, 3), (firstGiant.Angr, firstGiant.Hp));
        Assert.Equal((6, 2), (secondGiant.Angr, secondGiant.Hp));
        Assert.Equal((4, 3), (firstGiant.handcard.card.Attack, firstGiant.handcard.card.Health));
        Assert.Equal(printed, (giant.Attack, giant.Health, giant.dormant));
    }

    [Fact]
    public void TitanActionGenerationDoesNotWriteBackToThePrintedCard()
    {
        var playfield = CreatePlayfield();
        var titan = Assert.Single(playfield.ownMinions);
        var definition = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_858);
        Assert.Equal(CardDB.cardIDEnum.TTN_858, definition.cardIDenum);
        var printedAbilities = definition.TitanAbility;
        titan.handcard.card = definition;
        titan.Titan = true;

        var actions = Movegenerator.Instance.getMoveList(playfield, false, false, true);

        Assert.Contains(actions, action =>
            action.actionType == actionEnum.useTitanAbility && action.own.entitiyID == titan.entitiyID);
        Assert.Same(printedAbilities, definition.TitanAbility);
    }

    private static Playfield CreatePlayfield(int mana = 1)
    {
        Settings.Instance.setFilePath(AppContext.BaseDirectory);
        var fixture = new BoardTester(
            $"""
            水晶： {mana} / {mana} [我方英雄] 法师 生命值:( 30 + 0 奥秘数: 0 ) [敌方英雄] 法师 生命值:( 3 + 0 奥秘数: 0 )
            [我方场面] 肯瑞托法师 ( 4 / 3 )
            Enemy cards: 0
            Enemy deck: 0
            """);
        Assert.True(fixture.datareaded);
        Ai.Instance.botBase = new Behavior丨通用丨不设惩罚();
        return new Playfield();
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
