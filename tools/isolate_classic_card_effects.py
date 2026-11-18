"""Move audited classic effects' writes onto branch-owned card instances."""

from pathlib import Path
import re


# Each target is the owner of the card being written, not a CardDB lookup.
CARD_WRITES = {
    "Sim_AV_114": {"maxCostSpell": ("cost",)},
    "Sim_AV_265": {"m.handcard": ("cost",)},
    "Sim_DEEP_001": {"BEASTcard": ("Attack", "Health"), "UNDEADcard": ("Attack", "Health")},
    "Sim_DEEP_004": {"triggerhc": ("cost",)},
    "Sim_EDR_572": {"p.owncards[p.owncards.Count - 1]": ("cost",)},
    "Sim_FIR_907": {"triggerMinion.handcard": ("TAG_SCRIPT_DATA_NUM_1",)},
    "Sim_FIR_911": {"hc": ("TAG_SCRIPT_DATA_NUM_1",)},
    "Sim_FIR_914": {"hc": ("TAG_SCRIPT_DATA_NUM_1",)},
    "Sim_FIR_916": {"hc": ("TAG_SCRIPT_DATA_NUM_1",)},
    "Sim_GDB_881": {"handcard": ("cost",)},
    "Sim_MIS_919": {"newHandCard": ("Attack", "Health")},
    "Sim_NX2_010": {"hc": ("Charge",)},
    "Sim_NX2_051": {"hc": ("reborn",)},
    "Sim_RLK_209": {"hc": ("lifesteal",)},
    "Sim_RLK_820": {"handcard": ("spellpowervalue",)},
    "Sim_RLK_826": {"handcard": ("spellpowervalue",)},
    "Sim_RLK_Prologue_RLK_826": {"handcard": ("spellpowervalue",)},
    "Sim_TLC_822": {"handcard": ("cost",)},
    "Sim_TTN_721": {"m.handcard": ("Elusive",), "randomAlly.handcard": ("Elusive",)},
    "Sim_TTN_721t2": {"m.handcard": ("Elusive",), "randomAlly.handcard": ("Elusive",)},
    "Sim_TTN_858": {"handCard": ("Attack", "Health")},
    "Sim_TTN_858t1": {"handCard": ("Attack", "Health")},
    "Sim_WW_010t": {"p.ownWeapon": ("TAG_SCRIPT_DATA_NUM_1", "TAG_SCRIPT_DATA_NUM_2")},
    "Playfield.Triggers": {"hc": ("Infused", "TAG_SCRIPT_DATA_NUM_2")},
    "Playfield.Debug": {"card": ("Quickdraw",)},
    "Playfield": {"hc": ("Infused", "TAG_SCRIPT_DATA_NUM_2"), "card": ("Quickdraw",)},
}

TEMPORARY_COPIES = {
    "Sim_CS2_053": (
        "CardDB.Card card = p.owncards[(p.owncards.Count -1)].card;",
        "CardDB.Card card = p.owncards[(p.owncards.Count -1)].CardForWrite();",
    ),
    "Sim_TOY_353": (
        "CardDB.Card card = CardDB.Instance.getCardDataFromID(cardID);",
        "CardDB.Card card = CardDB.Instance.getCardDataFromID(cardID).CopyForSimulation();",
    ),
    "Sim_TTN_465": (
        "CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_465t);",
        "CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_465t).CopyForSimulation();",
    ),
}


def isolate_direct_writes(source: str, writes: dict[str, tuple[str, ...]]) -> str:
    # The replacements are limited to standalone, audited writes. A write
    # stays a single statement even if it is under an unbraced condition.
    targets = "|".join(
        rf"{re.escape(owner)}\.card\.(?:{'|'.join(fields)})"
        for owner, fields in writes.items()
    )
    pattern = re.compile(
        rf"(?m)/\*[\s\S]*?\*/|^[ \t]*(?P<target>{targets})(?=[ \t]*(?:\+\+|--|[+*/%-]?=(?!=)))",
    )

    def use_owned_card(match: re.Match) -> str:
        if match.group("target") is None:
            return match.group(0)
        return match.group(0).replace(".card.", ".CardForWrite().", 1)

    return pattern.sub(use_owned_card, source)


def isolate_card_effects(source: str, stem: str) -> str:
    if stem in CARD_WRITES:
        source = isolate_direct_writes(source, CARD_WRITES[stem])
    if stem in TEMPORARY_COPIES:
        source = source.replace(*TEMPORARY_COPIES[stem])
    if stem == "Sim_DED_517" and "var kid = this.kid.CopyForSimulation();" not in source:
        source = re.sub(
            r"(?m)^([ \t]*)(int dmg = \(ownplay\) \? p\.getSpellDamageDamage\(8\))",
            r"\1var kid = this.kid.CopyForSimulation();\n\1\2",
            source,
        )
    if stem == "Movegenerator":
        initialization = "titan.handcard.card.TitanAbility = titan.handcard.card.GetTitanAbility();"
        lookup = "titan.handcard.card.TitanAbility[i]"
        if initialization in source and lookup in source:
            source = source.replace(initialization, "var titanAbilities = titan.handcard.card.GetTitanAbility();")
            source = source.replace(lookup, "titanAbilities[i]")
    return source


def isolate_imported_card_effects(imported: Path) -> list[str]:
    card_names = set(CARD_WRITES) | set(TEMPORARY_COPIES) | {"Sim_DED_517"}
    files = [
        path for path in (imported / "cards").rglob("Sim_*.cs") if path.stem in card_names
    ]
    files.extend(imported / "ai" / name for name in (
        "Movegenerator.cs",
        "Action/Movegenerator.cs",
        "Playfield.cs",
        "Playfield/Playfield.Triggers.cs",
        "Playfield/Playfield.Debug.cs",
    ))
    modified = []
    for path in files:
        if not path.is_file():
            continue
        before = path.read_text(encoding="utf-8-sig")
        after = isolate_card_effects(before, path.stem)
        if after != before:
            path.write_text(after, encoding="utf-8")
            modified.append(path.relative_to(imported).as_posix())
    return modified
