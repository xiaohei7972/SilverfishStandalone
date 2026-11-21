"""Check Sim card comments or repair missing headers from CardDefs.xml."""

import argparse
import re
import xml.etree.ElementTree as ET
from collections import defaultdict
from pathlib import Path

CARDS = Path(__file__).resolve().parents[1] / "src/Silverfish.Simulation/Cards"
NAMESPACE = re.compile(r"namespace Silverfish\.Simulation\.Cards(?:\.Enchantments)?;")
DISPLAY_MARKUP = re.compile(r"\[x\]|</?[bi]>", re.IGNORECASE)
DESCRIPTION_TEMPLATE = re.compile(r"(?:\{\d+\}\s*)+")
FIELD_LABEL = re.compile(r"^(?:中文卡名|英文卡名|中文描述|英文描述)\s*[:：;；]")
SCAFFOLD_COMMENTS = ("// [CardSim(", "// 规则由施加附魔的单卡 Sim 引用。")


def card_comment_lines(entity):
    def localized(enum_id, language):
        element = entity.find(f"Tag[@enumID='{enum_id}']/{language}")
        value = "".join(element.itertext()) if element is not None else ""
        return " ".join(DISPLAY_MARKUP.sub("", value.replace(r"\n", " ")).split())

    card_id = entity.attrib["CardID"]
    chinese_text = localized(184, "zhCN")
    english_text = localized(184, "enUS")
    return (
        localized(185, "zhCN") or f"卡名未收录（{card_id}）",
        localized(185, "enUS") or f"Name unavailable ({card_id})",
        chinese_text or ("中文卡面描述未收录。" if english_text else "无卡面描述。"),
        english_text or ("English card text unavailable." if chinese_text else "No card text."),
    )


def location_comment_identity(entity):
    def tag_value(enum_id):
        tag = entity.find(f"Tag[@enumID='{enum_id}']")
        return tag.get("value") if tag is not None else None

    if tag_value(202) != "39":
        return None
    return tuple(tag_value(enum_id) for enum_id in (183, 199, 48, 45)), tag_value(321) == "1"


def resolve_description_templates(comments, locations):
    collectible_locations = defaultdict(list)
    for card_id, (identity, collectible) in locations.items():
        if collectible:
            lines = comments[card_id]
            collectible_locations[(*lines[:2], identity)].append(lines[2:])

    for card_id, lines in comments.items():
        chinese_template = DESCRIPTION_TEMPLATE.fullmatch(lines[2])
        english_template = DESCRIPTION_TEMPLATE.fullmatch(lines[3])
        if not (chinese_template or english_template):
            continue
        identity, _ = locations.get(card_id, (None, False))
        matches = collectible_locations.get((*lines[:2], identity), [])
        chinese, english = matches[0] if len(matches) == 1 else ("", "")
        comments[card_id] = (
            *lines[:2],
            template_description(chinese, "卡库仅提供描述模板。") if chinese_template else lines[2],
            template_description(english, "Card data contains only a description template.") if english_template else lines[3],
        )


def template_description(text, fallback):
    unavailable = {"无卡面描述。", "中文卡面描述未收录。", "No card text.", "English card text unavailable."}
    return text if text and text not in unavailable and not DESCRIPTION_TEMPLATE.fullmatch(text) else fallback


def header_span(source):
    lines = source.splitlines(keepends=True)
    namespace = next((i for i, line in enumerate(lines) if NAMESPACE.fullmatch(line.strip().lstrip("\ufeff"))), None)
    if namespace is None:
        raise ValueError("缺少单卡 Sim 命名空间。")

    start = namespace + 1
    while start < len(lines) and not lines[start].strip():
        start += 1
    end = start
    while end < len(lines) and end < start + 4:
        line = lines[end].rstrip("\r\n")
        if not (line == "//" or line.startswith("// ")) or line.startswith(SCAFFOLD_COMMENTS):
            break
        end += 1
    return lines, start, end


def has_card_comments(source):
    lines, start, end = header_span(source)
    content = [line[2:].strip() for line in lines[start:end]]
    return len(content) == 4 and all(
        line and line not in {"（无）", "(无)"} and not FIELD_LABEL.match(line)
        and not DESCRIPTION_TEMPLATE.fullmatch(line) for line in content
    )


def insert_card_comments(source, comments):
    lines, start, end = header_span(source)
    newline = "\r\n" if "\r\n" in source else "\n"
    return "".join(lines[:start]) + "".join(f"// {line}{newline}" for line in comments) + "".join(lines[end:])


def repair_headers(paths, card_defs):
    pending = {}
    for path in paths:
        source = path.read_bytes().decode("utf-8")
        if not has_card_comments(source):
            pending[path] = source
    if not pending:
        return 0

    needed = {path.stem[4:] for path in pending}
    comments = {}
    locations = {}
    for _, entity in ET.iterparse(card_defs, events=("end",)):
        if entity.tag == "Entity":
            card_id = entity.get("CardID")
            location = location_comment_identity(entity)
            if card_id in needed or location is not None:
                comments[card_id] = card_comment_lines(entity)
            if location is not None:
                locations[card_id] = location
            entity.clear()
    missing = needed - comments.keys()
    if missing:
        raise ValueError(f"CardDefs.xml 缺少这些卡牌 ID {'、'.join(sorted(missing))}")

    resolve_description_templates(comments, locations)
    for path, source in pending.items():
        path.write_bytes(insert_card_comments(source, comments[path.stem[4:]]).encode("utf-8"))
    return len(pending)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    mode = parser.add_mutually_exclusive_group(required=True)
    mode.add_argument("--check", action="store_true")
    mode.add_argument("--repair", type=Path, metavar="CARDDEFS_XML")
    parser.add_argument("--cards", type=Path, default=CARDS)
    args = parser.parse_args()

    paths = sorted(args.cards.rglob("Sim_*.cs"))
    if not paths:
        raise SystemExit(f"找不到 Sim 文件。{args.cards}")
    if args.repair:
        print(f"已补齐 {repair_headers(paths, args.repair)} 个 Sim 的卡牌注释。")
        return

    invalid = [path for path in paths if not has_card_comments(path.read_bytes().decode("utf-8"))]
    if invalid:
        for path in invalid[:20]:
            print(path)
        raise SystemExit(f"{len(invalid)} 个 Sim 缺少规范的四行卡牌注释。")
    print(f"{len(paths)} 个 Sim 的卡牌注释通过检查。")


if __name__ == "__main__":
    main()
