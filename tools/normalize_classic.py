#!/usr/bin/env python3
"""Idempotent, scoped headless compatibility fixes for classic Silverfish.

Applied to all input paths after import, including the verified owner ZIP.
The original source archive is never modified.
"""
from pathlib import Path
import json
import re
import sys

PATCHES = {
    "ai/Ai.cs": [
        ("Silverfish.Instance.updateCThunInfo(", "Hrtprozis.Instance.updateCThunInfo("),
    ],
    "ai/PenalityManager.cs": [
        ("ai.botBase", "Ai.Instance.botBase"),
    ],
}

ENEMY_HAND_MARKER = "// HEADLESS_BOARDTESTER_ENEMY_HAND_COUNT"

def normalize(root: Path) -> dict:
    imported = root / "src" / "Silverfish.Classic" / "Imported"
    modified = {}
    for relative, replacements in PATCHES.items():
        path = imported / relative
        if not path.is_file():
            raise FileNotFoundError(f"Expected classic source file missing: {relative}")
        before = path.read_text(encoding="utf-8-sig")
        after = before
        for needle, replacement in replacements:
            after = after.replace(needle, replacement)
        if after != before:
            path.write_text(after, encoding="utf-8")
            modified[relative] = True

    # Extend only BoardTester's small Chinese fixture parser. The original
    # default (5 enemy cards) remains unchanged; tests must supply the value.
    relative = "ai/BoardTester.cs"
    path = imported / relative
    if not path.is_file():
        raise FileNotFoundError(f"Expected classic source file missing: {relative}")
    before = path.read_text(encoding="utf-8-sig")
    after = before
    if ENEMY_HAND_MARKER not in before:
        pattern = r'(?m)^([ \t]*)if\s*\(s\.StartsWith\("水晶： "\)\)'
        def inject(match):
            indent = match.group(1)
            return (
                f'{indent}{ENEMY_HAND_MARKER}\n'
                f'{indent}if (s.StartsWith("Enemy cards: "))\n'
                f'{indent}{{\n'
                f"{indent}    enemyNumberHand = Convert.ToInt32(s.Split(' ')[2]);\n"
                f'{indent}    continue;\n'
                f'{indent}}}\n'
                f'{indent}if (s.StartsWith("Enemy deck: "))\n'
                f'{indent}{{\n'
                f"{indent}    enemyDecksize = Convert.ToInt32(s.Split(' ')[2]);\n"
                f'{indent}    continue;\n'
                f'{indent}}}\n'
                f'{indent}if (s.StartsWith("水晶： "))'
            )
        after, replacements = re.subn(pattern, inject, before, count=1)
        if replacements != 1:
            raise ValueError("BoardTester Chinese snapshot parser anchor not found")
    if after != before:
        path.write_text(after, encoding="utf-8")
        modified[relative] = True

    report = {
        "purpose": "headless normalization and explicit offline fixture fields",
        "modified": sorted(modified),
    }
    (root / "classic-headless-patches.json").write_text(
        json.dumps(report, indent=2, ensure_ascii=False) + "\n",
        encoding="utf-8",
    )
    return report

if __name__ == "__main__":
    root = Path(sys.argv[1]) if len(sys.argv) > 1 else Path(".")
    print(json.dumps(normalize(root), ensure_ascii=False, indent=2))
