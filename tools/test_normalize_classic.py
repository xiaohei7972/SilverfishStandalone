"""Regression tests for idempotent classic-source host detachment."""
from pathlib import Path
from tempfile import TemporaryDirectory

from normalize_classic import normalize, ENEMY_HAND_MARKER


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
            'if (s.StartsWith("水晶： ")) { ParseMana(); }',
            encoding="utf-8",
        )

        first = normalize(root)
        assert sorted(first["modified"]) == [
            "ai/Ai.cs", "ai/BoardTester.cs", "ai/PenalityManager.cs",
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

        second = normalize(root)
        assert second["modified"] == []
        assert (imported / "BoardTester.cs").read_text(encoding="utf-8") == board


if __name__ == "__main__":
    test_normalize()
    print("PASS: classic source patches are deterministic and idempotent")
