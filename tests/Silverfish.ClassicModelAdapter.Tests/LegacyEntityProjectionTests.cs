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
        Assert.True(definition.Titan);
        var printedAbilities = definition.TitanAbility;
        titan.handcard.card = definition;
        // The older generator reads the definition's tag directly.
        typeof(Minion).GetField("Titan")?.SetValue(titan, true);

        var actions = Movegenerator.Instance.getMoveList(playfield, false, false, true);

        Assert.Contains(actions, action =>
            action.actionType == actionEnum.useTitanAbility && action.own.entitiyID == titan.entitiyID);
        Assert.Same(printedAbilities, definition.TitanAbility);
    }

    [Fact]
    public void DeckBuffKeepsDuplicatesSeparateAcrossPlayfieldBranches()
    {
        var parent = CreatePlayfield();
        var battlecry = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_612);
        var ordinary = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_172);
        var effect = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.WORK_013);
        var printed = (battlecry.Attack, battlecry.Health);
        Assert.True(battlecry.battlecry);
        parent.ownDeck.AddRange(new[] { battlecry, ordinary, battlecry });
        parent.ownDeckSize = 3;
        var modified = new Playfield(parent);
        var sibling = new Playfield(parent);
        Assert.Equal(parent.ownDeck.Select(c => c.cardIDenum), modified.ownDeck.Select(c => c.cardIDenum));
        Assert.Empty(new Playfield(parent, true).ownDeck);

        effect.sim_card.getBattlecryEffect(modified, Assert.Single(modified.ownMinions), null, 0);

        Assert.Equal((printed.Attack + 1, printed.Health + 1), (modified.ownDeck[0].Attack, modified.ownDeck[0].Health));
        Assert.Equal((printed.Attack + 1, printed.Health + 1), (modified.ownDeck[2].Attack, modified.ownDeck[2].Health));
        Assert.NotSame(modified.ownDeck[0], modified.ownDeck[2]);
        Assert.Same(ordinary, modified.ownDeck[1]);
        Assert.Equal(printed, (parent.ownDeck[0].Attack, parent.ownDeck[0].Health));
        Assert.Equal(printed, (sibling.ownDeck[2].Attack, sibling.ownDeck[2].Health));
        Assert.Equal(printed, (battlecry.Attack, battlecry.Health));
        Assert.False(modified.isEqualf(sibling));
        var stateHash = typeof(Playfield).GetMethod("GetStateHash");
        if (stateHash is not null)
            Assert.NotEqual(stateHash.Invoke(modified, null), stateHash.Invoke(sibling, null));
    }

    [Fact]
    public void TargetedDrawCarriesDeckBuffsAndConsumesOneKnownSlot()
    {
        var parent = CreatePlayfield();
        var card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_612);
        var effect = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.WORK_013);
        var forest = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TOY_507);
        var printed = (card.cost, card.Attack, card.Health);
        parent.ownDeck.AddRange(new[] { card, card });
        parent.ownDeckSize = 2;
        var child = new Playfield(parent);
        effect.sim_card.getBattlecryEffect(child, Assert.Single(child.ownMinions), null, 0);

        forest.sim_card.useLocation(child, null, null);

        var drawn = Assert.Single(child.owncards);
        Assert.Equal(card.cardIDenum, drawn.card.cardIDenum);
        Assert.Equal((printed.cost - 1, printed.Attack + 1, printed.Health + 1),
            (drawn.manacost, drawn.card.Attack, drawn.card.Health));
        var remaining = Assert.Single(child.ownDeck);
        Assert.Equal((printed.cost, printed.Attack + 1, printed.Health + 1),
            (remaining.cost, remaining.Attack, remaining.Health));
        Assert.Equal(1, child.ownDeckSize);
        Assert.Equal(2, parent.ownDeckSize);
        Assert.Equal(2, parent.ownDeck.Count);
        Assert.Equal(printed, (card.cost, card.Attack, card.Health));
    }

    [Fact]
    public void TargetedDrawBurnsOneKnownCardWhenTheHandIsFull()
    {
        var playfield = CreatePlayfield();
        var card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_612);
        var forest = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TOY_507);
        playfield.ownDeck.Add(card);
        playfield.ownDeckSize = 1;
        for (var i = 0; i < 10; i++)
            playfield.owncards.Add(new Handmanager.Handcard(card) { entity = i + 100 });
        var cost = card.cost;

        forest.sim_card.useLocation(playfield, null, null);

        Assert.Empty(playfield.ownDeck);
        Assert.Equal(0, playfield.ownDeckSize);
        Assert.Equal(10, playfield.owncards.Count);
        Assert.Equal(cost, card.cost);
    }

    [Fact]
    public void TormentedDreadwingDrawsExactlyTwoDiscountedDragonSlots()
    {
        var parent = CreatePlayfield();
        var dragon = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_561);
        var other = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_172);
        var dreadwing = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EDR_572);
        Assert.Equal(CardDB.cardIDEnum.EDR_572, dreadwing.cardIDenum);
        Assert.Equal(TAG_RACE.DRAGON, (TAG_RACE)dragon.race);
        var cost = dragon.cost;
        parent.ownDeck.AddRange(new[] { other, dragon, dragon, dragon });
        parent.ownDeckSize = 4;
        var child = new Playfield(parent);

        dreadwing.sim_card.onDeathrattle(child, Assert.Single(child.ownMinions));

        Assert.Equal(2, child.owncards.Count);
        Assert.All(child.owncards, card =>
        {
            Assert.Equal(dragon.cardIDenum, card.card.cardIDenum);
            Assert.Equal(cost - 1, card.manacost);
        });
        Assert.NotSame(child.owncards[0].card, child.owncards[1].card);
        Assert.Equal(new[] { other, dragon }, child.ownDeck);
        Assert.Equal(2, child.ownDeckSize);
        Assert.Equal(4, parent.ownDeck.Count);
        Assert.Equal(cost, dragon.cost);
    }

    [Fact]
    public void TradingReturnsTheCardToTheActingBranchesDeckOnly()
    {
        CreatePlayfield(mana: 3);
        var definition = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.SW_072);
        Assert.True(definition.Tradeable);
        var host = Hrtprozis.Instance;
        var saved = host.turnDeck;
        try
        {
            host.turnDeck = new() { [CardDB.cardIDEnum.EX1_612] = 2 };
            var parent = new Playfield { ownDeckSize = 3 };
            parent.owncards.Add(new Handmanager.Handcard(definition) { entity = 91, position = 1 });
            var trade = new Playfield(parent);
            var sibling = new Playfield(parent);
            trade.doAction(new HREngine.Bots.Action(actionEnum.trade, trade.owncards[0], null, 0, null, 0, 0));

            Assert.False(host.turnDeck.ContainsKey(definition.cardIDenum));
            Assert.DoesNotContain(definition.cardIDenum, parent.CheckTurnDeckForType(CardDB.cardtype.MOB, 3));
            Assert.DoesNotContain(definition.cardIDenum, sibling.CheckTurnDeckForType(CardDB.cardtype.MOB, 3));
            Assert.Contains(definition.cardIDenum, trade.CheckTurnDeckForType(CardDB.cardtype.MOB, 3));
            Assert.Equal(3, trade.ownDeckSize);
            Assert.Equal(definition.cardIDenum, Assert.Single(trade.ownDeck).cardIDenum);
            Assert.Equal(CardDB.cardIDEnum.None, Assert.Single(trade.owncards).card.cardIDenum);

            var oneCardLeft = new Playfield(parent) { ownDeckSize = 1 };
            var lastCard = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_612);
            oneCardLeft.ownDeck.Add(lastCard);
            oneCardLeft.doAction(new HREngine.Bots.Action(actionEnum.trade, oneCardLeft.owncards[0], null, 0, null, 0, 0));
            Assert.Equal(definition.cardIDenum, Assert.Single(oneCardLeft.ownDeck).cardIDenum);
            Assert.Equal(lastCard.cardIDenum, Assert.Single(oneCardLeft.owncards).card.cardIDenum);
            Assert.Equal(1, oneCardLeft.ownDeckSize);
        }
        finally
        {
            host.turnDeck = saved;
        }
    }

    [Fact]
    public void EstimatedDrawConsumesOnlyTheRequestedCopiesInTheActingBranch()
    {
        CreatePlayfield();
        var host = Hrtprozis.Instance;
        var saved = host.turnDeck;
        try
        {
            var weapon = CardDB.cardIDEnum.CS2_097;
            host.turnDeck = new() { [weapon] = 3 };
            var parent = new Playfield { ownDeckSize = 3 };
            var drawing = new Playfield(parent);
            var sibling = new Playfield(parent);
            var forge = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.ICC_281);
            var simulation = forge.sim_card;
            // The two classic revisions use different spell callback signatures.
            var callback = simulation.GetType().GetMethods().Single(method =>
                method.DeclaringType == simulation.GetType() && method.Name == "onCardPlay");
            object?[] args = callback.GetParameters().Length == 5
                ? new object?[] { drawing, true, null, 0, new Handmanager.Handcard(forge) }
                : new object?[] { drawing, true, null, 0 };
            callback.Invoke(simulation, args);

            Assert.Equal(2, drawing.owncards.Count);
            Assert.All(drawing.owncards, card => Assert.Equal(weapon, card.card.cardIDenum));
            Assert.Equal(1, drawing.ownDeckSize);
            Assert.Equal(new[] { weapon }, drawing.CheckTurnDeckForType(CardDB.cardtype.WEAPON, 3));
            Assert.Equal(3, sibling.CheckTurnDeckForType(CardDB.cardtype.WEAPON, 3).Count);
            Assert.Equal(3, host.turnDeck[weapon]);
            Assert.Empty(new Playfield(parent, true).CheckTurnDeckForType(CardDB.cardtype.WEAPON, 3));
            Assert.Empty(parent.CheckTurnDeckForType(CardDB.cardtype.WEAPON, 0));
        }
        finally
        {
            host.turnDeck = saved;
        }
    }

    [Fact]
    public void PatchesConsumesOneDeckEntryAndCanSummonIndependentlyInSiblingBranches()
    {
        CreatePlayfield();
        var host = Hrtprozis.Instance;
        var saved = host.turnDeck;
        var savedPatches = host.patchesInDeck;
        try
        {
            var patchesId = CardDB.cardIDEnum.CFM_637;
            var pirate = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_146);
            Assert.Equal(CardDB.Race.PIRATE, pirate.race);
            host.turnDeck = new() { [patchesId] = 1 };
            host.patchesInDeck = true;
            var parent = new Playfield { ownDeckSize = 3 };
            parent.ownDeck.Add(CardDB.Instance.getCardDataFromID(patchesId));
            var first = new Playfield(parent);
            var second = new Playfield(parent);

            foreach (var branch in new[] { first, second })
            {
                branch.placeAmobSomewhere(new Handmanager.Handcard(pirate) { entity = 92 }, 0, branch.ownMinions.Count);
                Assert.Single(branch.ownMinions, minion => minion.handcard.card.cardIDenum == patchesId);
                Assert.Empty(branch.ownDeck);
                Assert.Equal(2, branch.ownDeckSize);
                Assert.DoesNotContain(patchesId, branch.CheckTurnDeckForType(CardDB.cardtype.MOB, 3));
            }
            Assert.Single(parent.ownDeck);
            Assert.Equal(3, parent.ownDeckSize);
            Assert.Equal(1, host.turnDeck[patchesId]);
            Assert.True(host.patchesInDeck);
        }
        finally
        {
            host.turnDeck = saved;
            host.patchesInDeck = savedPatches;
        }
    }

    private static Playfield CreatePlayfield(int mana = 1)
    {
        Settings.Instance.setFilePath(AppContext.BaseDirectory);
        Hrtprozis.Instance.setInstances();
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
