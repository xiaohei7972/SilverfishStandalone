"""Audit legacy Sim declarations and maintain the PlayReq block of each constructed Sim.

Usage: python tools/migrate_play_requirements.py <legacy Silverfish directory> [--check]
"""
import argparse
import csv
import io
import re
import xml.etree.ElementTree as ET
from collections import deque
from pathlib import Path

from sim_comments import (
    card_comment_lines, has_card_comments, insert_card_comments, location_comment_identity, resolve_description_templates,
)

ROOT = Path(__file__).resolve().parents[1]
CARDS = ROOT / "src/Silverfish.Simulation/Cards"
MANIFEST = ROOT / "docs/play-requirements.tsv"
BEGIN = "    // <legacy-play-requirements>"
END = "    // </legacy-play-requirements>"
LEGACY_BLOCK = re.compile(re.escape(BEGIN) + r".*?" + re.escape(END), re.S)
AUTHORED_REQUIREMENTS = re.compile(r"^[ \t]*internal[ \t]+static[ \t]+PlayReqSet[ \t]+PlayReqs\b", re.M)
METHOD = re.compile(r"override\s+PlayReq\[\]\s+(GetPlayReqs|GetUseAbilityReqs)\s*\(\s*\)\s*\{")
REQ = re.compile(r"new\s+PlayReq\s*\(\s*(?:CardDB\.)?ErrorType2\.\s*(\w+)\s*(?:,\s*([\w.]+)\s*)?\)")
MODE = re.compile(r"^(?:BG(?:\d|_)|TB_|BTA_|PVPDR_|PVPDR|LETL_|BOM_|BOTA_|DALA_|ULDA_|DRGA_|LOOTA_|GILA_|TRLA_|BRMA|NAX[0-9]|LOEA|KARA_|ICCA|Story_|Prologue_|TUTR_|ONYXIA_BOSS)", re.I)
SPECIAL = {"CardRace": "CARDRACE", "markOfEvil": "MARK_OF_EVIL", "IMP": "IMP", "Treant": "TREANT",
           "StarshipPiece": "STARSHIP_PIECE", "Starship": "STARSHIP", "Protoss": "PROTOSS", "Wisp": "WISP"}
# Tags absent from the public GAME_TAG enum retain their CardDefs numbers.
TAG_IDS = {"TREANT": 2831, "WISP": 3881}
EXPLICIT_ROOTS = {"GAME_005", "CS2_017", "CS2_034", "CS2_049", "CS2_056", "CS2_083b", "CS2_101",
                  "CS2_102", "CS1h_001", "DS1h_292", "AT_132_PALADIN", "AT_132_HUNTER"}


def read_metadata(path):
    data = {}
    comments = {}
    locations = {}
    for _, node in ET.iterparse(path, events=("end",)):
        if node.tag != "Entity":
            continue
        tags = {tag.get("name"): tag.get("value") for tag in node.findall("Tag")}
        refs = {t.get("cardID") for t in node.iter() if t.get("cardID")}
        card_id = node.attrib["CardID"]
        data[card_id] = (tags.get("COLLECTIBLE") == "1", tags.get("CARD_SET"), refs)
        comments[card_id] = card_comment_lines(node)
        if (location := location_comment_identity(node)) is not None:
            locations[card_id] = location
        node.clear()
    resolve_description_templates(comments, locations)
    return data, comments


def declarations(source):
    found = {}
    for method in METHOD.finditer(source):
        end = method.end()
        depth = 1
        while depth:
            depth += (source[end] == "{") - (source[end] == "}")
            end += 1
        body = source[method.end():end - 1]
        rules = REQ.findall(body)
        if len(rules) != len(re.findall(r"new\s+PlayReq\s*\(", body)):
            raise ValueError(body)
        if not rules and re.search(r"return\s+playReqs\s*;", body):
            rules = [("CopiedPlayRequirements", "")]
        found[method[1]] = rules
    return found


def render_rule(rule):
    name, value = rule
    if name == "CopiedPlayRequirements":
        return "PlayReq.Copied"
    if value.startswith("CardDB.Specialtags."):
        tag = SPECIAL[value.split(".")[-1]]
        parameter = "GAME_TAG." + tag if tag not in TAG_IDS else f"(GAME_TAG){TAG_IDS[tag]}"
    elif name == "REQ_TARGET_WITH_RACE" and value:
        race = {"11": "UNDEAD", "14": "MURLOC", "15": "DEMON", "17": "MECHANICAL", "18": "ELEMENTAL",
                "20": "BEAST", "23": "PIRATE", "24": "DRAGON", "38": "EGG"}.get(value, value.split(".")[-1])
        parameter = "TAG_RACE." + ("BEAST" if race == "PET" else race)
    else:
        parameter = value
    return f"new(ErrorType.{name}" + (f", {parameter}" if parameter else "") + ")"


def migrate(legacy, check):
    data, comments = read_metadata(legacy / "data/CardDefs.xml")
    parents, methods, locations, refs = {}, {}, {}, {}
    for path in sorted((legacy / "cards").rglob("Sim_*.cs")):
        text = re.sub(r"/\*.*?\*/|//[^\n]*", "", path.read_text("utf-8-sig", errors="replace"), flags=re.S)
        cls = re.search(r"\bclass\s+Sim_(\w+)\s*:\s*(\w+)", text)
        if not cls:
            continue
        card = cls[1]
        locations[card] = path
        parents[card] = cls[2].removeprefix("Sim_")
        methods[card] = declarations(text)
        refs[card] = set(re.findall(r"(?:cardIDEnum\.|\"|new\s+Sim_)([A-Za-z]\w+)", text))
    roots = {c for c, (collectible, set_id, _) in data.items()
             if collectible and set_id not in {"1453", "1586"} and not MODE.match(c)}
    roots |= {c for c in data if re.match(r"^(?:HERO_\d{2}(?:[a-z]\w*)?|VAN_HERO_\d{2}bp\w*)$", c)}
    roots |= EXPLICIT_ROOTS
    for card in data:
        parent = re.match(r"^((?:[A-Z0-9]+_)*[A-Z0-9]+_\d+)[a-zA-Z]\w*$", card)
        if parent:
            refs.setdefault(parent[1], set()).add(card)
    # Non-collectible constructed reward pools and alternate prints with independent IDs.
    pools = {"PART_": "GVG_044", "UNG_999t": "UNG_089", "REV_79": "REV_990",
             "DAL_614": "DAL_415", "DAL_615": "DAL_415", "DAL_739": "DAL_415",
             "VAN_DREAM_": "VAN_EX1_572", "VAN_EX1_tk33": "VAN_EX1_323",
             "VAN_NEW1_033": "VAN_NEW1_031", "CORE_CS2_073": "CS2_073",
             "TOY_400t": "TOY_700", "DEEP_999t": "DEEP_018",
             "ULD_616": "DAL_415", "DMF_519_COPY": "DMF_519", "LOOT_998": "LOOT_357"}
    for card in locations:
        for prefix, owner in pools.items():
            if card.startswith(prefix):
                refs.setdefault(owner, set()).add(card)
        if re.match(r"^(?:CS1h_001|CS2_034|CS2_049|CS2_101|DS1h_292|AT_132_HUNTER)_H", card):
            roots.add(card)
    queue = deque(sorted(roots))
    origin = {c: "constructed" for c in roots}
    while queue:
        card = queue.popleft()
        for child in sorted(refs.get(card, set()) | data.get(card, (False, None, set()))[2]):
            if child not in origin and child in data and not MODE.match(child) and "Puzzle" not in child:
                origin[child] = card
                queue.append(child)

    existing = {p.stem[4:]: p for p in CARDS.rglob("Sim_*.cs")}
    rows = io.StringIO()
    manifest = csv.writer(rows, delimiter="\t", lineterminator="\n")
    manifest.writerow(["CardId", "Scope/source", "Legacy Sim", "Declaration owner", "Method", "Legacy conditions", "Independent Sim"])
    changes = []
    for card in sorted(locations):
        inherited = {}
        current = card
        while current in methods:
            for kind, rules in methods[current].items():
                inherited.setdefault(kind, (current, rules))
            current = parents[current]
        if not inherited:
            continue
        included = card in origin or (card in existing and "[CardSim(" in existing[card].read_text("utf8"))
        dest = existing.get(card, CARDS / locations[card].parent.name / f"Sim_{card}.cs")
        for method, (owner, rules) in sorted(inherited.items()):
            manifest.writerow([card, origin.get(card, "starter" if included else "outside"), locations[card].relative_to(legacy).as_posix(),
                               owner, method, ";".join(n + (f"({v})" if v else "") for n, v in rules),
                               dest.relative_to(ROOT).as_posix() if included else ""])
        if not included:
            continue
        fields = []
        for kind in ("GetPlayReqs", "GetUseAbilityReqs"):
            rules = inherited.get(kind, ("", []))[1]
            fields.append("[" + ", ".join(map(render_rule, rules)) + "]")
        block = BEGIN + "\n    internal static PlayReqSet PlayReqs { get; } = new(\n        " + ",\n        ".join(fields) + ");\n" + END
        text = dest.read_text("utf-8-sig") if dest.exists() else f"namespace Silverfish.Simulation.Cards;\n\ninternal static class Sim_{card}\n{{\n}}\n"
        legacy_block = LEGACY_BLOCK.search(text)
        authored = text if legacy_block is None else text[:legacy_block.start()] + text[legacy_block.end():].lstrip("\n")
        if AUTHORED_REQUIREMENTS.search(authored):
            # The manifest retains the original conditions even when a Sim owns the runtime rules.
            new = authored
        elif legacy_block is not None:
            new = text[:legacy_block.start()] + block + text[legacy_block.end():]
        else:
            start = text.index("{", text.index("class Sim_")) + 1
            new = text[:start] + "\n" + block + "\n" + text[start:]
        if "using Silverfish.Domain.Client;" not in new:
            new = "using Silverfish.Domain.Client;\n" + new
        if not has_card_comments(new):
            if card not in comments:
                raise ValueError(f"CardDefs.xml 缺少卡牌 {card}，无法生成 Sim 注释。")
            new = insert_card_comments(new, comments[card])
        if new != text:
            changes.append(dest)
            if not check:
                dest.parent.mkdir(parents=True, exist_ok=True)
                dest.write_text(new, "utf8")
    if not MANIFEST.exists() or MANIFEST.read_text("utf8") != rows.getvalue():
        changes.append(MANIFEST)
        if not check:
            MANIFEST.write_text(rows.getvalue(), "utf8")
    if changes and check:
        raise SystemExit(f"PlayReq migration differs in {len(changes)} files: {changes[0]}")
    print(f"PlayReq migration: {len(changes)} files changed")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("legacy", type=Path)
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    migrate(args.legacy, args.check)
