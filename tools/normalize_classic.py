#!/usr/bin/env python3
"""Apply idempotent, narrowly scoped host-detachment fixes to staged classic AI.

Run after any input mode, including the SHA-pinned source-only work package.
The original archive stays unmodified.
"""
from pathlib import Path
import json
import sys

PATCHES = {
    "ai/Ai.cs": [
        ("Silverfish.Instance.updateCThunInfo(", "Hrtprozis.Instance.updateCThunInfo("),
    ],
    "ai/PenalityManager.cs": [
        ("ai.botBase", "Ai.Instance.botBase"),
    ],
}

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
    report = {
        "purpose": "headless source normalization only; does not modify original ZIP",
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
