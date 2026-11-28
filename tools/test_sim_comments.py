import tempfile
import unittest
from pathlib import Path
from unittest.mock import patch

import migrate_play_requirements
import sim_comments


class SimCommentTests(unittest.TestCase):
    def test_location_template_uses_only_a_unique_matching_collectible_location(self):
        names = ("练习地标", "Practice Location")
        description = ("冻结一个随从。", "Freeze a minion.")
        template = ("{0}{1}", "{0} {1}")
        identity = ("1691", "4", "3", "2")
        other_identity = ("1691", "4", "3", "3")

        for duplicate in (False, True):
            with self.subTest(duplicate=duplicate):
                comments = {"actual": (*names, *description), "variant": (*names, *template),
                            "other": (*names, "消灭一个随从。", "Destroy a minion.")}
                locations = {"actual": (identity, True), "variant": (identity, False),
                             "other": (identity if duplicate else other_identity, True)}
                sim_comments.resolve_description_templates(comments, locations)
                expected = ("卡库仅提供描述模板。", "Card data contains only a description template.") if duplicate else description
                self.assertEqual((*names, *expected), comments["variant"])

    def test_playreq_migration_writes_bilingual_comments_and_preserves_authored_rules(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            legacy = root / "legacy"
            (legacy / "data").mkdir(parents=True)
            xml = legacy / "data/CardDefs.xml"
            xml.write_text(
                """<CardDefs build="1"><Entity CardID="TEST_001" ID="1">
                <Tag enumID="185"><zhCN>练习法术</zhCN><enUS>Practice Spell</enUS></Tag>
                <Tag enumID="184"><zhCN>抽一张牌。</zhCN><enUS>[x]&lt;b&gt;Draw&lt;/b&gt;
                a card.</enUS></Tag>
                <Tag name="COLLECTIBLE" value="1"/>
                <Tag name="CARD_SET" value="1635"/>
                </Entity></CardDefs>""",
                encoding="utf-8",
            )
            legacy_sim = legacy / "cards/1635-怀旧/Sim_TEST_001.cs"
            legacy_sim.parent.mkdir(parents=True)
            legacy_sim.write_text(
                """class Sim_TEST_001 : SimTemplate
                {
                    public override PlayReq[] GetPlayReqs()
                    {
                        return new[] { new PlayReq(ErrorType2.REQ_TARGET_TO_PLAY) };
                    }
                }""",
                encoding="utf-8",
            )
            cards = root / "Cards"
            with patch.multiple(migrate_play_requirements, ROOT=root, CARDS=cards, MANIFEST=root / "manifest.tsv"):
                migrate_play_requirements.migrate(legacy, check=False)
                migrate_play_requirements.migrate(legacy, check=True)

            generated = cards / "1635-怀旧/Sim_TEST_001.cs"
            source = generated.read_text(encoding="utf-8")
            lines, start, end = sim_comments.header_span(source)
            self.assertEqual(lines[start:end], [
                "// 练习法术\n",
                "// Practice Spell\n",
                "// 抽一张牌。\n",
                "// Draw a card.\n",
            ])
            self.assertIn("ErrorType.REQ_TARGET_TO_PLAY", source)

            without_comments = "".join(lines[:start] + lines[end:])
            generated.write_text(without_comments, encoding="utf-8")
            self.assertFalse(sim_comments.has_card_comments(without_comments))
            self.assertEqual(1, sim_comments.repair_headers([generated], xml))
            self.assertEqual(source, generated.read_text(encoding="utf-8"))
            self.assertEqual(0, sim_comments.repair_headers([generated], xml))

            authored = source.replace(migrate_play_requirements.BEGIN + "\n", "").replace(
                migrate_play_requirements.END + "\n", "").replace("REQ_TARGET_TO_PLAY", "REQ_TARGET_IF_AVAILABLE")
            generated.write_text(authored, encoding="utf-8")
            manifest = root / "manifest.tsv"
            with patch.multiple(migrate_play_requirements, ROOT=root, CARDS=cards, MANIFEST=manifest):
                migrate_play_requirements.migrate(legacy, check=True)
            self.assertEqual(authored, generated.read_text(encoding="utf-8"))
            self.assertIn("GetPlayReqs\tREQ_TARGET_TO_PLAY\t", manifest.read_text(encoding="utf-8"))


if __name__ == "__main__":
    unittest.main()
