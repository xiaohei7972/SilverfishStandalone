#!/usr/bin/env python3
"""Idempotent, scoped headless compatibility fixes for classic Silverfish.

Applied to all input paths after import, including the verified owner ZIP.
The original source archive is never modified.
"""
from pathlib import Path
import json
import re
import sys

from isolate_classic_card_effects import isolate_imported_card_effects
from isolate_classic_deck import carry_known_deck_into_branches, draw_from_known_deck

PATCHES = {
    "ai/Ai.cs": [
        ("Silverfish.Instance.updateCThunInfo(", "Hrtprozis.Instance.updateCThunInfo("),
    ],
    "ai/PenalityManager.cs": [
        ("ai.botBase", "Ai.Instance.botBase"),
    ],
}

ENEMY_HAND_MARKER = "// HEADLESS_BOARDTESTER_ENEMY_HAND_COUNT"


def detach_card_before_write(source: str, receiver: str, field: str) -> str:
    pattern = rf"(?m)^([ \t]*){re.escape(receiver)}\.{field}\s*="

    def detach(match: re.Match) -> str:
        assignment = f"{match.group(1)}{receiver} = {receiver}.CopyForSimulation();\n"
        if source[:match.start()].endswith(assignment):
            return match.group(0)
        return assignment + match.group(0)

    return re.sub(pattern, detach, source)


def add_card_metadata_copy(source: str) -> str:
    if "internal Card CopyForSimulation()" in source:
        return source
    card = re.search(r"(?m)^([ \t]*)public class Card[ \t]*\n[ \t]*\{\n", source)
    if card is None:
        raise ValueError("Classic CardDB.Card class anchor not found")
    indent = card.group(1) + "    "
    if re.search(r"(?m)^\s*public Card Copy\(\)", source):
        # The November source already defines the card definition copy.
        method = f"{indent}internal Card CopyForSimulation() => Copy();"
    else:
        method = "\n".join(indent + line for line in (
            "internal Card CopyForSimulation()",
            "{",
            "    var copy = (Card)MemberwiseClone();",
            "    copy.trigers = this.trigers is null ? null : new(this.trigers);",
            "    copy.TitanAbility = this.TitanAbility is null ? null : new(this.TitanAbility);",
            "    copy.races = this.races is null ? null : new(this.races);",
            "    return copy;",
            "}",
        ))
    return source[:card.end()] + method + "\n\n" + source[card.end():]


def isolate_handcard_copies(source: str) -> str:
    for field in ("enchs", "conditionalList"):
        source = source.replace(
            f"this.{field} = hc.{field};",
            f"this.{field} = hc.{field} is null ? null : new(hc.{field});",
        )
    if "public void getDIYCard(CardDB.Card part1, CardDB.Card part2)" in source:
        source = detach_card_before_write(source, "this.card", "cost")

    # Minion formerly retained the entire Handcard object. Preserve all its
    # existing fields, including fields deliberately omitted by Handcard(hc).
    if "internal Handcard CopyForMinion()" in source:
        return source
    constructor = re.search(r"(?m)^([ \t]*)public Handcard\(Handcard hc\)", source)
    if constructor is None:
        raise ValueError("Classic Handcard copy constructor anchor not found")
    indent = constructor.group(1)
    method = "\n".join(indent + line for line in (
        "internal Handcard CopyForMinion()",
        "{",
        "    var copy = (Handcard)MemberwiseClone();",
        "    copy.enchs = this.enchs is null ? null : new(this.enchs);",
        "    copy.conditionalList = this.conditionalList is null ? null : new(this.conditionalList);",
        "    return copy;",
        "}",
    ))
    return source[:constructor.start()] + method + "\n\n" + source[constructor.start():]


def isolate_minion_copies(source: str) -> str:
    source = source.replace("this.handcard = m.handcard;", "this.handcard = m.handcard?.CopyForMinion();")
    if re.search(r"(?m)^\s*public List<CardDB\.cardIDEnum> enchs\b", source):
        return source.replace("this.enchs = m.enchs;", "this.enchs = m.enchs is null ? null : new(m.enchs);")
    return source


def isolate_weapon_copies(source: str) -> str:
    if "ClassicWeaponState.CopyEnchantments(w.enchants)" in source:
        return source
    constructor = re.search(
        r"(?m)^([ \t]*)public Weapon\(Weapon w\)\s*\{\n", source,
    )
    if constructor is None:
        raise ValueError("Classic Weapon copy constructor anchor not found")
    indent = constructor.group(1) + "    "
    assignments = "\n".join(indent + line for line in (
        "this.pID = w.pID;",
        "this.Overkill = w.Overkill;",
        "this.HonorableKill = w.HonorableKill;",
        "if (w.enchants is { Count: > 0 })",
        "    this.enchants = ClassicWeaponState.CopyEnchantments(w.enchants);",
    ))
    return source[:constructor.end()] + assignments + "\n" + source[constructor.end():]


def isolate_monolithic_playfield(source: str) -> str:
    source = re.sub(r"(?m)^([ \t]*)public class Playfield\b", r"\1public partial class Playfield", source)
    return draw_from_known_deck(carry_known_deck_into_branches(source))


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
    # A pre-patched 2026-11-05 source bundle already contains both fields;
    # do not inject duplicate parser branches when staging it through Actions.
    chinese_start = before.find("private void setupFromChnFile")
    mana_at = before.find('if (s.StartsWith("水晶： "))', chinese_start)
    chinese_prefix = before[chinese_start:mana_at] if chinese_start >= 0 and mana_at >= 0 else ""
    already_supported = (
        'if (s.StartsWith("Enemy cards: "))' in chinese_prefix
        and 'if (s.StartsWith("Enemy deck: "))' in chinese_prefix
    )
    if ENEMY_HAND_MARKER not in before and not already_supported:
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
    # The old routine used Environment.Exit(0) on invalid board input.
    # That kills an embedding process and falsely marks a failed CI smoke as
    # a successful exit. The detached library must surface a normal error.
    after = after.replace(
        'Environment.Exit(0);',
        'throw new FormatException("Invalid classic BoardTester input.", e);',
    )
    # The November source uses a host string extension in the enchantment parser.
    after = after.replace('.split(" ")', ".Split(' ')")
    after = detach_card_before_write(after, "card.card", "MODULAR_ENTITY_PART_1")
    after = detach_card_before_write(after, "this.ownHero.handcard.card", "type")
    after = detach_card_before_write(after, "this.enemyHero.handcard.card", "type")
    if after != before:
        path.write_text(after, encoding="utf-8")
        modified[relative] = True

    # The Chinese BoardTester assigns the friendly hero ID 0. The November
    # action resolver must recognize that hero when simulating enemy attacks.
    relative = "ai/Playfield/Playfield.Actions.cs"
    path = imported / relative
    if path.is_file():
        before = path.read_text(encoding="utf-8-sig")
        after = re.sub(
            r"(public Minion FindMinionByEntityId\(int entityID\)\s*\{\n)"
            r"([ \t]*)(if \(entityID <= 0\) return null;)",
            r"\1\2if (entityID == 0 && this.ownHero.entitiyID == 0) return this.ownHero;\n\2\3",
            before,
        )
        if after != before:
            path.write_text(after, encoding="utf-8")
            modified[relative] = True

    for relative, isolate in (
        ("ai/CardDB.cs", add_card_metadata_copy),
        ("ai/CardDB/CardDB.cs", add_card_metadata_copy),
        ("ai/Handmanager.cs", isolate_handcard_copies),
        ("ai/Minion.cs", isolate_minion_copies),
        ("ai/Weapon.cs", isolate_weapon_copies),
        ("ai/Playfield.cs", isolate_monolithic_playfield),
        ("ai/Playfield/Playfield.Ctors.cs", carry_known_deck_into_branches),
        ("ai/Playfield/Playfield.HandDeck.cs", draw_from_known_deck),
    ):
        path = imported / relative
        if path.is_file():
            before = path.read_text(encoding="utf-8-sig")
            after = isolate(before)
            if after != before:
                path.write_text(after, encoding="utf-8")
                modified[relative] = True

    report = {
        "purpose": "headless normalization, offline fixture fields and branch copy isolation",
        "modified": sorted([*modified, *isolate_imported_card_effects(imported)]),
    }
    (root / "classic-headless-patches.json").write_text(
        json.dumps(report, indent=2, ensure_ascii=False) + "\n",
        encoding="utf-8",
    )
    return report

if __name__ == "__main__":
    root = Path(sys.argv[1]) if len(sys.argv) > 1 else Path(".")
    print(json.dumps(normalize(root), ensure_ascii=False, indent=2))
