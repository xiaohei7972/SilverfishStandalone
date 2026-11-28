"""Route classic simulation reads and audited deck events through Playfield."""

import re


ESTIMATED_DRAWS = {
    "Sim_EDR_843": ("spellcard, ownplay", "mobcard, ownplay"),
    "Sim_EDR_843a": ("spellcard, ownplay",),
    "Sim_EDR_843b": ("mobcard, ownplay",),
    "Sim_EDR_843t1": ("spellcard, ownplay", "mobcard, ownplay"),
    "Sim_GDB_302": ("drawElemental, true",),
    "Sim_GDB_310": ("cid, m.own",),
    "Sim_GVG_015": ("card[0], ownplay",),
    "Sim_ICC_281": ("cid, ownplay",),
    "Sim_KAR_061": ("drawPed, true", "drawDragon, true", "drawMurloc, true"),
    "Sim_WON_095": ("card[0], ownplay",),
}


def method_block(signature: str, *, comments: bool = False) -> str:
    prefix = r"(?:^[ \t]*//[^\n]*\n)*" if comments else ""
    return (
        rf"(?ms){prefix}^(?P<indent>[ \t]*){signature}[^\n]*\n"
        r"(?:[ \t]*//[^\n]*\n)*(?P=indent)\{\n.*?^(?P=indent)\}"
    )


def seed_replay_deck(source: str) -> str:
    def use_fixture(match: re.Match) -> str:
        body = match[0]
        return re.sub(r"(?m)^[ \t]*\w+\.prozis\.turnDeck = bt\.od;[^\n]*\n", "", body).replace(
            "new Playfield()", "new Playfield(bt.od)",
        )

    return re.sub(
        method_block(r"public (?:List<double> autoTester\(|void simmulateWholeTurn\(BoardTester bt\))"),
        use_fixture,
        source,
    )


def isolate_playfield_deck_estimate(source: str) -> str:
    source = re.sub(
        r"(?m)^([ \t]*)public Playfield\(\)[ \t]*\n\1\{\n",
        r"\1public Playfield() : this(Hrtprozis.Instance.turnDeck) { }\n\n"
        r"\1internal Playfield(IReadOnlyDictionary<CardDB.cardIDEnum, int> deckEstimate)\n"
        r"\1{\n\1    SeedDeckEstimate(deckEstimate);\n",
        source,
    )
    source = re.sub(
        r"(?m)^([ \t]*)this\.patchesInDeck = p\.patchesInDeck;",
        r"\1this.patchesInDeck = !transToEnemy && p.patchesInDeck;\n"
        r"\1if (!transToEnemy) DeckEstimate = p.DeckEstimate;",
        source,
    )
    source = re.sub(
        method_block(r"private void HandleTrade\(Action a\)"),
        lambda match: f"{match['indent']}private void HandleTrade(Action a) => "
        f"TradeIntoDeck(a.{'hc' if 'a.hc.card.TradeCost' in match[0] else 'card'});",
        source,
    )
    source = re.sub(
        method_block(r"private void HandlePatchesSummon\(Action a\)", comments=True) + r"\n\n?",
        "",
        source,
    )
    source = re.sub(r"(?m)^[ \t]*HandlePatchesSummon\(a\);[^\n]*\n", "", source)
    source = re.sub(
        r"(?ms)(?:^[ \t]*//[^\n]*\n)*^([ \t]*)CardDB\.Card patches = CardDB\.Instance\.getCardDataFromID\(CardDB\.cardIDEnum\.CFM_637\);\n"
        r"(?:(?!^\1addMinionToBattlefield\(m\);).)*?this\.prozis\.patchesInDeck.*?"
        r"^\1addMinionToBattlefield\(m\);",
        r"\1addMinionToBattlefield(m);\n\1TrySummonPatches(hc, this.ownMinions.Count);",
        source,
    )
    source = re.sub(r"\bprozis\.turnDeck\b", "DeckEstimate", source)
    source = source.replace("int quantity = kvp.Value;", "int quantity = Math.Min(kvp.Value, num - totalCount);")
    if "if (num <= 0) return matchingCards;" not in source:
        source = re.sub(
            r"(?m)^([ \t]*)var matchingCards = new List<CardDB\.cardIDEnum>\(\);\n([ \t]*int totalCount = 0;)",
            r"\1var matchingCards = new List<CardDB.cardIDEnum>();\n"
            r"\1if (num <= 0) return matchingCards;\n\2",
            source,
        )
    return source.replace("this.drawACard(kvp.Key, true, true);", "this.DrawFromDeckEstimate(kvp.Key, true);")


def isolate_estimated_card_effect(source: str, stem: str) -> str:
    source = re.sub(r"\bp\.prozis\.turnDeck\b", "p.DeckEstimate", source)
    for args in ESTIMATED_DRAWS.get(stem, ()):
        pattern = re.escape(args).replace(r",\ ", r",[ \t]*")
        source = re.sub(rf"\bp\.drawACard\({pattern}\)", f"p.DrawFromDeckEstimate({args})", source)
    if stem == "Sim_CFM_637" and "bool summoned = false;" in source and "p.ownDeck.Remove(deckCard);" in source:
        source = re.sub(r"(?m)(?:^[ \t]*//[^\n]*\n)*^[ \t]*bool summoned = false;\n\n?", "", source)
        source = re.sub(method_block(r"public override void onMinionWasSummoned\(", comments=True) + r"\n", "", source)
    return source
