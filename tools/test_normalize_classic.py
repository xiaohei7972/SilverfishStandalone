"""Regression tests for idempotent classic-source host detachment."""
from pathlib import Path
from tempfile import TemporaryDirectory

from normalize_classic import ENEMY_HAND_MARKER, add_card_metadata_copy, isolate_minion_copies, normalize
from migrate_classic_deck_effects import migrate_deck_effect


def test_normalize() -> None:
    with TemporaryDirectory() as folder:
        root = Path(folder)
        imported = root / "src/Silverfish.Classic/Imported/ai"
        imported.mkdir(parents=True)
        (imported / "Ai.cs").write_text(
            "Silverfish.Instance.updateCThunInfo(1, 2, 3);\n"
            "public List<double> autoTester(bool printstuff) // normal replay\n"
            "    // Additional mode description.\n"
            "{\n"
            "    Playfield pMain = new Playfield();\n"
            "    pMain.prozis.turnDeck = bt.od;\n"
            "    if (printstuff)\n"
            "    {\n"
            "        pMain = new Playfield();\n"
            "    }\n"
            "}\n"
            "public void simmulateWholeTurn(BoardTester bt)\n"
            "{\n"
            "    Playfield p = new Playfield();\n"
            "    p.prozis.turnDeck = bt.od;\n"
            "    var replay = new Playfield();\n"
            "}\n"
            "public Playfield StartLive() => new Playfield();\n",
            encoding="utf-8",
        )
        (imported / "PenalityManager.cs").write_text(
            "return ai.botBase.getPlayfieldValue(p);",
            encoding="utf-8",
        )
        (imported / "BoardTester.cs").write_text(
            'if (s.StartsWith("水晶： ")) { ParseMana(); }\n'
            'foreach (string enchantment in enchs.split(" ")) { ParseEnchantment(enchantment); }\n'
            '    card.card.MODULAR_ENTITY_PART_1 = card.MODULAR_ENTITY_PART_1;\n'
            '    this.ownHero.handcard.card.type = CardDB.cardtype.HERO;\n'
            'catch (Exception e) { Environment.Exit(0); }',
            encoding="utf-8",
        )
        actions_path = imported / "Playfield/Playfield.Actions.cs"
        actions_path.parent.mkdir()
        actions_path.write_text(
            "public Minion FindMinionByEntityId(int entityID)\n"
            "{\n"
            "    if (entityID <= 0) return null;\n"
            "    if (entityID == this.ownHero.entitiyID) return this.ownHero;\n"
            "}\n",
            encoding="utf-8",
        )
        minion_path = imported / "Minion.cs"
        minion_path.write_text(
            "public List<CardDB.cardIDEnum> enchs = new();\n"
            "this.handcard = m.handcard;\n"
            "this.enchs = m.enchs;\n",
            encoding="utf-8",
        )
        handcard_path = imported / "Handmanager.cs"
        handcard_path.write_text(
            "    public Handcard(Handcard hc)\n"
            "    {\n"
            "        this.enchs = hc.enchs;\n"
            "        this.conditionalList = hc.conditionalList;\n"
            "    }\n"
            "    public void getDIYCard(CardDB.Card part1, CardDB.Card part2)\n"
            "    {\n"
            "        this.card.cost = part1.cost + part2.cost;\n"
            "    }\n",
            encoding="utf-8",
        )
        card_db_path = imported / "CardDB.cs"
        card_db_path.write_text("public class Card\n{\n}\n", encoding="utf-8")
        weapon_path = imported / "Weapon.cs"
        weapon_path.write_text("public Weapon(Weapon w)\n{\n    this.card = w.card;\n}\n", encoding="utf-8")
        cards_path = imported.parent / "cards" / "fixture"
        cards_path.mkdir(parents=True)
        weapon_effect_path = cards_path / "Sim_WW_010t.cs"
        weapon_effect_path.write_text(
            "/*\n    p.ownWeapon.card.TAG_SCRIPT_DATA_NUM_1++;\n*/\n"
            "if (canAttack)\n    p.ownWeapon.card.TAG_SCRIPT_DATA_NUM_1++;\n"
            "    //p.ownWeapon.card.TAG_SCRIPT_DATA_NUM_2++;\n"
            "    p.ownWeapon.card.TAG_SCRIPT_DATA_NUM_2++;\n"
            "    var saved = p.ownWeapon.card.TAG_SCRIPT_DATA_NUM_1;\n",
            encoding="utf-8",
        )
        hand_effect_path = cards_path / "Sim_EDR_572.cs"
        hand_effect_path.write_text("p.owncards[p.owncards.Count - 1].card.cost -= 1;\n", encoding="utf-8")
        token_effect_path = cards_path / "Sim_TTN_465.cs"
        token_effect_path.write_text(
            "    CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_465t);\n",
            encoding="utf-8",
        )
        moves_path = imported / "Movegenerator.cs"
        moves_path.write_text(
            "titan.handcard.card.TitanAbility = titan.handcard.card.GetTitanAbility();\n"
            "CardDB.Card ability = titan.handcard.card.TitanAbility[i];\n",
            encoding="utf-8",
        )
        playfield_path = imported / "Playfield.cs"
        playfield_path.write_text(
            "public class Playfield\n"
            "{\n"
            "    public Playfield(Playfield p, bool transToEnemy = false)\n"
            "    {\n"
            "        this.ownDeckSize = p.ownDeckSize;\n"
            "    }\n"
            "    public bool isEqualf(Playfield p)\n"
            "    {\n"
            "        return true;\n"
            "    }\n"
            "    public void drawACard(CardDB.cardIDEnum ss, bool own, bool nopen = false)\n"
            "    {\n"
            "        var c = CardDB.Instance.getCardDataFromID(s);\n"
            "    }\n"
            "}\n",
            encoding="utf-8",
        )
        deck_effect_path = cards_path / "Sim_WORK_013.cs"
        deck_effect_path.write_text(
            "    foreach (CardDB.Card card in p.ownDeck)\n"
            "    {\n"
            "        if (card.battlecry)\n"
            "        {\n"
            "            card.Attack += 1;\n"
            "            card.Health += 1;\n"
            "        }\n"
            "    }\n",
            encoding="utf-8",
        )

        first = normalize(root)
        assert sorted(first["modified"]) == [
            "ai/Ai.cs", "ai/BoardTester.cs", "ai/CardDB.cs", "ai/Handmanager.cs",
            "ai/Minion.cs", "ai/Movegenerator.cs", "ai/PenalityManager.cs",
            "ai/Playfield.cs", "ai/Playfield/Playfield.Actions.cs", "ai/Weapon.cs",
            "cards/fixture/Sim_EDR_572.cs", "cards/fixture/Sim_TTN_465.cs",
            "cards/fixture/Sim_WORK_013.cs", "cards/fixture/Sim_WW_010t.cs",
        ]
        ai = (imported / "Ai.cs").read_text(encoding="utf-8")
        assert "Hrtprozis.Instance.updateCThunInfo" in ai
        assert "prozis.turnDeck" not in ai
        assert ai.count("new Playfield(bt.od)") == 4
        assert "StartLive() => new Playfield();" in ai
        penalty = (imported / "PenalityManager.cs").read_text(encoding="utf-8")
        assert "Ai.Instance.botBase" in penalty
        board = (imported / "BoardTester.cs").read_text(encoding="utf-8")
        assert ENEMY_HAND_MARKER in board
        assert 'enemyNumberHand = Convert.ToInt32' in board
        assert 'enemyDecksize = Convert.ToInt32' in board
        assert 'if (s.StartsWith("水晶： "))' in board
        assert '.split(" ")' not in board
        assert "enchs.Split(' ')" in board
        assert "    card.card = card.card.CopyForSimulation();\n    card.card.MODULAR_ENTITY_PART_1" in board
        assert "    this.ownHero.handcard.card = this.ownHero.handcard.card.CopyForSimulation();" in board
        assert 'Environment.Exit(0)' not in board
        assert 'throw new FormatException("Invalid classic BoardTester input.", e);' in board
        actions = actions_path.read_text(encoding="utf-8")
        assert (
            "if (entityID == 0 && this.ownHero.entitiyID == 0) return this.ownHero;\n"
            "    if (entityID <= 0) return null;"
        ) in actions
        minion = minion_path.read_text(encoding="utf-8")
        assert "this.handcard = m.handcard?.CopyForMinion();" in minion
        assert "this.enchs = m.enchs is null ? null : new(m.enchs);" in minion
        handcard = handcard_path.read_text(encoding="utf-8")
        assert "    internal Handcard CopyForMinion()" in handcard
        assert "this.enchs = hc.enchs is null ? null : new(hc.enchs);" in handcard
        assert "this.conditionalList = hc.conditionalList is null ? null : new(hc.conditionalList);" in handcard
        assert "        this.card = this.card.CopyForSimulation();\n        this.card.cost" in handcard
        card_db = card_db_path.read_text(encoding="utf-8")
        assert "var copy = (Card)MemberwiseClone();" in card_db
        assert "copy.races = this.races is null ? null : new(this.races);" in card_db
        weapon = weapon_path.read_text(encoding="utf-8")
        assert "this.Overkill = w.Overkill;" in weapon
        assert "this.enchants = ClassicWeaponState.CopyEnchantments(w.enchants);" in weapon
        modern_card_db = "public class Card\n{\n    public Card Copy() => new Card();\n}\n"
        modern_card_db = add_card_metadata_copy(modern_card_db)
        assert "internal Card CopyForSimulation() => Copy();" in modern_card_db
        assert add_card_metadata_copy(modern_card_db) == modern_card_db
        weapon_effect = weapon_effect_path.read_text(encoding="utf-8")
        assert "/*\n    p.ownWeapon.card.TAG_SCRIPT_DATA_NUM_1++;\n*/" in weapon_effect
        assert "if (canAttack)\n    p.ownWeapon.CardForWrite().TAG_SCRIPT_DATA_NUM_1++;" in weapon_effect
        assert "    p.ownWeapon.CardForWrite().TAG_SCRIPT_DATA_NUM_2++;" in weapon_effect
        assert "    var saved = p.ownWeapon.card.TAG_SCRIPT_DATA_NUM_1;" in weapon_effect
        assert "    //p.ownWeapon.card.TAG_SCRIPT_DATA_NUM_2++;" in weapon_effect
        assert hand_effect_path.read_text(encoding="utf-8") == "p.owncards[p.owncards.Count - 1].CardForWrite().cost -= 1;\n"
        assert "TTN_465t).CopyForSimulation();" in token_effect_path.read_text(encoding="utf-8")
        assert moves_path.read_text(encoding="utf-8") == (
            "var titanAbilities = titan.handcard.card.GetTitanAbility();\n"
            "CardDB.Card ability = titanAbilities[i];\n"
        )
        playfield = playfield_path.read_text(encoding="utf-8")
        assert "public partial class Playfield" in playfield
        assert "if (!transToEnemy) this.ownDeck.AddRange(p.ownDeck);" in playfield
        assert "if (!HasSameKnownDeck(p)) return false;" in playfield
        assert "DrawCard(ss, own, nopen, null);" in playfield
        assert "knownCard ?? CardDB.Instance.getCardDataFromID(s)" in playfield
        deck_effect = deck_effect_path.read_text(encoding="utf-8")
        assert "CardDB.Card card = p.ownDeck[deckIndex];" in deck_effect
        assert "            card = p.DeckCardForWrite(deckIndex);\n            card.Attack += 1;" in deck_effect
        original = 'public string enchs = "";\nthis.enchs = m.enchs;\n'
        assert isolate_minion_copies(original) == original

        second = normalize(root)
        assert second["modified"] == []
        assert (imported / "BoardTester.cs").read_text(encoding="utf-8") == board
        assert actions_path.read_text(encoding="utf-8") == actions
        assert minion_path.read_text(encoding="utf-8") == minion
        assert handcard_path.read_text(encoding="utf-8") == handcard
        assert card_db_path.read_text(encoding="utf-8") == card_db
        assert weapon_path.read_text(encoding="utf-8") == weapon
        assert weapon_effect_path.read_text(encoding="utf-8") == weapon_effect
        assert playfield_path.read_text(encoding="utf-8") == playfield
        assert deck_effect_path.read_text(encoding="utf-8") == deck_effect


if __name__ == "__main__":
    test_normalize()
    recruit = (
        "\tpublic override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)\n"
        "    {\n"
        '        Log("}"); // {\n'
        "\t\tforeach (var card in p.DeckEstimate)\n"
        "        { KeepOldBody(); }\n"
        "\t}\n"
        "\tpublic override PlayReq[] GetPlayReqs() => new PlayReq[0];\n"
    )
    migrated = migrate_deck_effect(recruit, "Sim_LOOT_093")
    assert "p.RecruitFromDeck(ownplay, 3, card => card.cost <= 2);" in migrated
    assert "KeepOldBody" not in migrated
    assert "GetPlayReqs() => new PlayReq[0];" in migrated
    assert migrate_deck_effect(migrated, "Sim_LOOT_093") == migrated
    print("PASS: classic source patches are deterministic and idempotent")
