"""Regression tests for idempotent classic-source host detachment."""
from pathlib import Path
from tempfile import TemporaryDirectory

from normalize_classic import ENEMY_HAND_MARKER, add_card_metadata_copy, isolate_minion_copies, normalize


def test_normalize() -> None:
    with TemporaryDirectory() as folder:
        root = Path(folder)
        imported = root / "src/Silverfish.Classic/Imported/ai"
        imported.mkdir(parents=True)
        (imported / "Ai.cs").write_text(
            "Silverfish.Instance.updateCThunInfo(1, 2, 3);",
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

        first = normalize(root)
        assert sorted(first["modified"]) == [
            "ai/Ai.cs", "ai/BoardTester.cs", "ai/CardDB.cs", "ai/Handmanager.cs",
            "ai/Minion.cs", "ai/PenalityManager.cs",
            "ai/Playfield/Playfield.Actions.cs", "ai/Weapon.cs",
        ]
        ai = (imported / "Ai.cs").read_text(encoding="utf-8")
        assert "Hrtprozis.Instance.updateCThunInfo" in ai
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


if __name__ == "__main__":
    test_normalize()
    print("PASS: classic source patches are deterministic and idempotent")
