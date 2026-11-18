"""Move audited classic deck effects onto the common draw and recruit operations."""

import re


MINION = "card.type == CardDB.cardtype.MOB"
SPELL = "card.type == CardDB.cardtype.SPELL"


def race(name: str) -> str:
    return f"Playfield.MatchesDeckRace(card, CardDB.Race.{name})"


def recruit(side: str, count: int, predicate: str = "", after: str = "") -> str:
    match = f", card => {predicate}" if predicate else ""
    callback = f", afterSummon: minion => {after}" if after else ""
    return f"p.RecruitFromDeck({side}, {count}{match}{callback});"


def draw(side: str, count: int, predicate: str, reduction: int = 0) -> str:
    discount = f", costReduction: {reduction}" if reduction else ""
    return f"p.DrawMatchingDeckCards({side}, {count}, card => {predicate}{discount});"


SPELL_PLAY = "onCardPlay"
BATTLECRY = "getBattlecryEffect"
DEATHRATTLE = "onDeathrattle"
TURN_END = "onTurnEndsTrigger"
HERO_ATTACK = "afterHeroattack"
MURLOC = f"{MINION} && {race('MURLOC')}"
FEL_SPELL = f"{SPELL} && card.SpellSchool == CardDB.SpellSchool.FEL"

DECK_EFFECT_BODIES = {
    "AT_079": {BATTLECRY: "p.PutSecretsFromDeck(own.own, 5);"},
    "AV_100": {BATTLECRY: (
        f"if (own.own && !p.HasMatchingDeckCard(card => {MINION} && card.cost >= own.handcard.card.cost))\n"
        f"    {recruit('own.own', 2)}"
    )},
    "AV_312": {BATTLECRY: (
        "if (target is null) return;\n"
        "int recruitCost = target.handcard.card.cost + 1;\n"
        "p.minionGetDestroyed(target);\n"
        "p.doDmgTriggers();\n"
        + recruit("own.own", 1, "card.cost == recruitCost")
    )},
    "AV_336": {BATTLECRY: recruit("own.own", 1, race("PET"), "p.minionGetRush(minion)")},
    "AV_343": {BATTLECRY: draw("own.own", 1, f"{SPELL} && card.cost <= 3", reduction=3)},
    "BAR_329": {BATTLECRY: recruit("own.own", 3, "card.deathrattle && card.cost <= 5")},
    "BAR_330": {DEATHRATTLE: draw("m.own", 1, f"{MINION} && card.deathrattle")},
    "BT_486": {TURN_END: (
        "if (triggerEffectMinion.own && turnEndOfOwner)\n"
        f"    {recruit('true', 1, race('DEMON'))}"
    )},
    "CFM_905": {SPELL_PLAY: draw("ownplay", 3, f"{MINION} && card.cost == 1")},
    "CORE_BAR_330": {DEATHRATTLE: draw("m.own", 1, f"{MINION} && card.deathrattle")},
    "DED_508": {SPELL_PLAY: recruit("ownplay", 2, after="p.minionGetBuffed(minion, p.deckAngrBuff, p.deckHpBuff)")},
    "DINO_131": {DEATHRATTLE: recruit("m.own", 1, race("PET"), "minion.lifesteal = true")},
    "DINO_411": {BATTLECRY: draw("own.own", 1, f"{MINION} && card.Attack == 0")},
    "EDR_485": {DEATHRATTLE: draw("m.own", 1, f"{MINION} && card.cost >= 7")},
    "FB_Champs_LOOT_306": {DEATHRATTLE: recruit("m.own", 1, race("DEMON"))},
    "FIR_929": {DEATHRATTLE: (
        "if (m.own)\n"
        f"    {draw('true', 1, f'{SPELL} && card.SpellSchool == CardDB.SpellSchool.FIRE')}\n"
        "else\n"
        "    p.drawACard(CardDB.cardIDEnum.None, false);"
    )},
    "GDB_463": {"OnSpellburst": draw("m.own", 1, race("DRAENEI"))},
    "GIL_634": {BATTLECRY: "p.PutSecretsFromDeck(own.own, 1);", DEATHRATTLE: "p.PutSecretsFromDeck(m.own, 1);"},
    "ICC_086": {SPELL_PLAY: "p.PutSecretsFromDeck(ownplay, 5);"},
    "ICC_089": {SPELL_PLAY: (
        "if (ownplay)\n"
        f"    {draw('true', 2, MURLOC)}\n"
        "else\n"
        "    for (int i = 0; i < 2; i++) p.drawACard(CardDB.cardIDEnum.None, false);"
    )},
    "LOOT_184": {DEATHRATTLE: recruit("m.own", 1, "card.cost == 8")},
    "LOOT_306": {DEATHRATTLE: recruit("m.own", 1, race("DEMON"))},
    "LOOT_309": {SPELL_PLAY: (
        "p.minionGetArmor(ownplay ? p.ownHero : p.enemyHero, 6);\n"
        + recruit("ownplay", 1, "card.cost <= 4",
                  "{ if (minion.handcard.card.cost == 4) p.evaluatePenality -= 20; }")
    )},
    "LOOT_314": {DEATHRATTLE: recruit("m.own", 2, "card.cost <= 4")},
    "LOOT_370": {SPELL_PLAY: recruit("ownplay", 1)},
    "LOOT_375": {BATTLECRY: recruit("m.own", 1, "card.cost <= 4")},
    "LOOT_380": {HERO_ATTACK: recruit("hero.own", 1)},
    "LOOT_511": {BATTLECRY: recruit("m.own", 1, race("PET")), DEATHRATTLE: recruit("m.own", 1, race("PET"))},
    "LOOT_540": {TURN_END: (
        "if (triggerEffectMinion.own == turnEndOfOwner)\n"
        f"    {recruit('triggerEffectMinion.own', 1, race('DRAGON'))}"
    )},
    "SCH_610": {SPELL_PLAY: recruit("ownplay", 2, f"card.cost <= 5 && {race('PET')}",
                                    "{ p.minionGetBuffed(minion, p.deckAngrBuff, p.deckHpBuff); p.minionGetRush(minion); }")},
    "SW_021": {BATTLECRY: recruit("m.own", 1), DEATHRATTLE: recruit("m.own", 1)},
    "SW_043": {BATTLECRY: (
        f"int drawn = {draw('own.own', 1, FEL_SPELL, reduction=2)}\n"
        "if (drawn > 0)\n"
        "    p.evaluatePenality -= 10;"
    )},
    "SW_095": {SPELL_PLAY: draw("ownplay", 1, "card.overload > 0")},
    "TOY_641": {DEATHRATTLE: draw("m.own", 1, f"{MINION} && {race('DEMON')}", reduction=2)},
    "TSC_034": {SPELL_PLAY: (
        "if (own.own)\n"
        f"    {draw('true', 3, MURLOC)}\n"
        "else\n"
        "    for (int i = 0; i < 3; i++) p.drawACard(CardDB.cardIDEnum.None, false);"
    )},
    "TTN_800t3": {SPELL_PLAY: draw("ownplay", 3, "card.overload > 0")},
    "VAC_960": {HERO_ATTACK: (
        "if (own.own && p.ownWeapon.card.cardIDenum == CardDB.cardIDEnum.VAC_960)\n"
        f"    {recruit('true', 1, 'card.cost == 1')}"
    )},
}

CALL_TO_ARMS = (
    "int recruited = p.RecruitFromDeck(ownplay, 3, card => card.cost <= 2);\n"
    "if (recruited == 3 && p.ownMinions.Count < 7) p.evaluatePenality -= 30;"
)
DECK_EFFECT_BODIES.update({
    "LOOT_093": {SPELL_PLAY: (
        "bool hasDeckData = p.ownDeck.Count > 0 || p.DeckEstimate.Count > 0;\n"
        + CALL_TO_ARMS
        + "\nelse if (ownplay && !hasDeckData && p.ownMinions.Count < 4) p.evaluatePenality -= 200;"
    )},
    "FB_Champs_LOOT_093": {SPELL_PLAY: CALL_TO_ARMS},
})

LEGACY_TRIGGER_NAMES = {
    ("SW_021", BATTLECRY): "public override void onDeathrattle(Playfield p, Minion m)",
    ("TSC_034", SPELL_PLAY): "public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)",
}


BODY_TOKENS = re.compile(r'''//[^\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'|[{}]''', re.S)


def rewrite_deck_method(source: str, signature: str, body: str, new_signature: str | None = None,
                        additional_method: str = "") -> str:
    start = re.search(rf"(?m)^([ \t]*)({signature}[^\n]*)\n[ \t]*\{{", source)
    if start is None:
        return source

    # The imported sources mix tabs and spaces, even within one method.
    depth = 1
    for token in BODY_TOKENS.finditer(source, start.end()):
        depth += int(token[0] == "{") - int(token[0] == "}")
        if depth == 0:
            break
    else:
        raise ValueError(f"Unclosed classic deck effect method {start[2]}")

    previous = source[start.end():token.start()]
    if not re.search(r"\bin (?:p|this)\.DeckEstimate\b", previous):
        return source

    indent = start[1]
    lines = "\n".join(indent + "    " + line for line in body.splitlines())
    method = f"{indent}{new_signature or start[2]}\n{indent}{{\n{lines}\n{indent}}}"
    if additional_method:
        method += "\n\n" + "\n".join(indent + line for line in additional_method.splitlines())
    return source[:start.start()] + method + source[token.end():]


def migrate_deck_effect(source: str, stem: str) -> str:
    card = stem.removeprefix("Sim_")
    for method, body in DECK_EFFECT_BODIES.get(card, {}).items():
        signature = LEGACY_TRIGGER_NAMES.get((card, method))
        extra = ""
        if card == "GIL_634" and method == BATTLECRY and "void onDeathrattle(" not in source:
            extra = "\n".join((
                "public override void onDeathrattle(Playfield p, Minion m)",
                "{", "    p.PutSecretsFromDeck(m.own, 1);", "}",
            ))
        source = rewrite_deck_method(
            source, rf"public override void {method}\(", body, signature, extra,
        )
    return source


def migrate_playfield_deck_effects(source: str) -> str:
    return rewrite_deck_method(
        source, r"public void SummonMurlocs\(Minion attacker\)", "\n".join((
            "if (attacker.own)",
            "    RecruitFromDeck(true, 2, card => MatchesDeckRace(card, CardDB.Race.MURLOC));",
            "else",
            "{",
            "    var murloc = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_168);",
            "    for (int i = 0; i < 2 && enemyDeckSize > 0 && enemyMinions.Count < 7; i++)",
            "    {",
            "        enemyDeckSize--;",
            "        callKid(murloc, enemyMinions.Count, false);",
            "    }",
            "}",
        )),
    )
