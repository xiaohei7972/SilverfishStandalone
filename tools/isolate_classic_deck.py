"""Attach the known-deck implementation to classic Playfield and audited card effects."""

import re


DECK_BUFFS = {
    "Sim_CORE_DMF_709": "Attack",
    "Sim_DEEP_036": "Attack",
    "Sim_DEEP_999t4": "Attack",
    "Sim_DINO_421": "Attack",
    "Sim_DMF_709": "Attack",
    "Sim_RLK_828": "Attack",
    "Sim_SCH_609": "Attack",
    "Sim_TIME_609t2": "Attack",
    "Sim_TSC_943": "Attack",
    "Sim_VAC_321": "TAG_SCRIPT_DATA_NUM_1",
    "Sim_WORK_013": "Attack",
}

DECK_DRAWS = {"Sim_EDR_572", "Sim_TID_704", "Sim_TOY_507", "Sim_WW_816"}
DECK_EFFECTS = set(DECK_BUFFS) | DECK_DRAWS


def carry_known_deck_into_branches(source: str) -> str:
    if "this.ownDeck.AddRange(p.ownDeck);" not in source:
        source = re.sub(
            r"(?m)^([ \t]*)(this\.ownDeckSize = p\.ownDeckSize;)",
            r"\1if (!transToEnemy) this.ownDeck.AddRange(p.ownDeck);\n\1\2",
            source,
        )
    if "if (!HasSameKnownDeck(p)) return false;" not in source:
        source = re.sub(
            r"(?m)^([ \t]*)public bool (isEqual\(Playfield p, bool logg\)|isEqualf\(Playfield p\))"
            r"([ \t]*\n[ \t]*\{\n)",
            r"\1public bool \2\3\1    if (!HasSameKnownDeck(p)) return false;\n",
            source,
        )
    if "hash = MixKnownDeckState(hash);" not in source:
        source = re.sub(
            r"(?m)^([ \t]*)(mixStateHash\(ref hash, this\.ownDeckSize\);)",
            r"\1\2\n\1hash = MixKnownDeckState(hash);",
            source,
        )
    return source


def draw_from_known_deck(source: str) -> str:
    draw = re.compile(
        r"(?ms)^(?P<indent>[ \t]*)public void drawACard\("
        r"CardDB\.cardIDEnum ss, bool own, bool nopen = false\)[ \t]*\n"
        r"[ \t]*\{\n(?P<body>.*?)^(?P=indent)\}"
    )
    match = draw.search(source)
    if match is None:
        if "private void DrawCard(CardDB.cardIDEnum ss, bool own, bool nopen, CardDB.Card knownCard)" in source:
            return source
        raise ValueError("Classic card-ID draw overload anchor not found")

    indent = match["indent"]
    body = match["body"].replace("CardDB.Instance.getCardDataFromID(s)", "knownCard ?? CardDB.Instance.getCardDataFromID(s)")
    replacement = (
        f"{indent}public void drawACard(CardDB.cardIDEnum ss, bool own, bool nopen = false) =>\n"
        f"{indent}    DrawCard(ss, own, nopen, null);\n\n"
        f"{indent}private void DrawCard(CardDB.cardIDEnum ss, bool own, bool nopen, CardDB.Card knownCard)\n"
        f"{indent}{{\n{body}{indent}}}"
    )
    return source[:match.start()] + replacement + source[match.end():]


def isolate_deck_effect(source: str, stem: str) -> str:
    old_loop = re.compile(
        r"(?ms)^(?P<indent>[ \t]*)foreach \(CardDB\.Card (?P<card>card|battlecryCard) in p\.ownDeck\)"
        r"[ \t]*\n(?P=indent)\{\n(?P<body>.*?)^(?P=indent)\}"
    )

    def own_slot(match: re.Match) -> str:
        body = match["body"]
        if stem in DECK_BUFFS:
            field = DECK_BUFFS[stem]
            body = re.sub(
                rf"(?m)^([ \t]*)(card\.{field}(?:[ \t]*[+*/%-]?=|[ \t]*\+\+))",
                r"\1card = p.DeckCardForWrite(deckIndex);\n\1\2",
                body,
                count=1,
            )
        elif stem == "Sim_TOY_507":
            body = body.replace("CardDB.Instance.getCardDataFromID(battlecryCard.cardIDenum)", "battlecryCard")
            body = re.sub(
                r"(?m)^([ \t]*)card\.cost--;\s*\n[ \t]*p\.drawACard\(battlecryCard\.cardIDenum, true, true\);",
                r"\1p.DrawKnownDeckCard(deckIndex, costReduction: 1);",
                body,
            )
        elif stem == "Sim_TID_704":
            body = body.replace("p.drawACard(card.cardIDenum, triggerMinion.own);", "p.DrawKnownDeckCard(deckIndex);")
        elif stem == "Sim_WW_816":
            body = body.replace("p.drawACard(card.cardIDenum, ownplay);", "p.DrawKnownDeckCard(deckIndex);")
            body = body.replace("if ((TAG_RACE)card.race == TAG_RACE.DRAGON)", "if (ownplay && (TAG_RACE)card.race == TAG_RACE.DRAGON)")
            body = body.replace("continue;", "break;")
        elif stem == "Sim_EDR_572":
            body = re.sub(
                r"(?m)^([ \t]*)p\.drawACard\(card\.cardIDenum, m\.own\);\s*\n"
                r"[ \t]*p\.owncards\[p\.owncards\.Count - 1\]\.(?:CardForWrite\(\)|card)\.cost -= 1;",
                r"\1p.DrawKnownDeckCard(deckIndex, costReduction: 1);",
                body,
            )
            body = body.replace("if ((TAG_RACE)card.race == TAG_RACE.DRAGON)", "if (m.own && (TAG_RACE)card.race == TAG_RACE.DRAGON)")
            body = body.replace("continue;", "break;")
        if body == match["body"]:
            return match.group(0)

        indent = match["indent"]
        inside = indent + ("\t" if indent.endswith("\t") else "    ")
        return (
            f"{indent}for (int deckIndex = 0; deckIndex < p.ownDeck.Count; deckIndex++)\n"
            f"{indent}{{\n{inside}CardDB.Card {match['card']} = p.ownDeck[deckIndex];\n{body}{indent}}}"
        )

    return old_loop.sub(own_slot, source)
