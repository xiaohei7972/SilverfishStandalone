"""Dependency-free regression tests for the selective classic-Silverfish importer."""
from pathlib import Path
from tempfile import TemporaryDirectory
from zipfile import ZipFile
import hashlib

from import_classic import stage


def test_synthetic_import() -> None:
    with TemporaryDirectory() as tmp:
        root = Path(tmp)
        archive = root / 'sample.zip'
        prefix = 'release/Routines/DefaultRoutine/Silverfish/'
        with ZipFile(archive, 'w') as z:
            z.writestr(prefix + 'ai/Ai.cs', 'using System.Windows;\nusing Triton.Game;\nclass Ai { void Update(){ Silverfish.Instance.updateCThunInfo(1, 2, 3); } }')
            z.writestr(prefix + 'ai/Hrtprozis.cs', 'class Hrtprozis {}')
            z.writestr(prefix + 'ai/PenalityManager.cs', 'class PenalityManager { void Score(){ ai.botBase.ToString(); } }')
            z.writestr(prefix + 'ai/Playfield/Playfield.Ctors.cs', 'class Playfield {}')
            z.writestr(prefix + 'data/CardDefs.xml', '<cards/>')
            z.writestr(prefix + 'ai/Zones.cs', 'using Triton.Game;')
            for name in ('Sim_TTN_092.cs', 'Sim_TTN_092t1.cs',
                         'Sim_TTN_092t2.cs', 'Sim_TTN_092t3.cs'):
                z.writestr(prefix + 'cards/1858-泰坦诸神/' + name,
                           'using Triton.Game;\nclass Broken { void Call() { GameState.Get(); } }')
            z.writestr(prefix + 'cards/../../bad.cs', 'evil')
            z.writestr('release/Hearthbuddy.exe', 'not-a-real-exe')
        digest = hashlib.sha256(archive.read_bytes()).hexdigest()
        out = root / 'output'
        report = stage(archive, out, expected_sha256=digest)
        assert report['imported_files'] == 9
        assert report['imported_cs_files'] == 8
        assert report['sha256'] == digest
        assert 'ai/Zones.cs' in report['quarantined']
        assert len(report['disabled_card_simulators']) == 4
        for name in ('Sim_TTN_092.cs', 'Sim_TTN_092t1.cs',
                     'Sim_TTN_092t2.cs', 'Sim_TTN_092t3.cs'):
            text = (out / 'src/Silverfish.Classic/Imported/cards/1858-泰坦诸神' / name).read_text()
            assert '// SILVERFISH_STANDALONE_AGGRAMAR_DISABLED' in text
            assert '// BEGIN DISABLED ORIGINAL' in text
            assert '// using Triton.Game;' in text
            assert '    class ' + name[:-3] + ' : SimTemplate' in text
            assert not any(line.strip().startswith('using Triton.Game;') for line in text.splitlines())
        assert (out / 'src/Silverfish.Classic/Imported/ai/Ai.cs').read_text() == '\n\nclass Ai { void Update(){ Hrtprozis.Instance.updateCThunInfo(1, 2, 3); } }'
        assert 'Ai.Instance.botBase' in (out / 'src/Silverfish.Classic/Imported/ai/PenalityManager.cs').read_text()
        assert not list(out.rglob('Hearthbuddy.exe'))
        assert not list(out.rglob('bad.cs'))
        try:
            stage(archive, root / 'wrong-sha', expected_sha256='0' * 64)
        except ValueError as err:
            assert 'SHA-256' in str(err)
        else:
            raise AssertionError('Expected digest mismatch to be rejected')


def test_monolithic_playfield_import() -> None:
    with TemporaryDirectory() as tmp:
        root = Path(tmp)
        archive = root / 'old-style.zip'
        prefix = 'old/Routines/DefaultRoutine/Silverfish/'
        with ZipFile(archive, 'w') as z:
            z.writestr(prefix + 'ai/Ai.cs', 'class Ai {}')
            z.writestr(prefix + 'ai/Hrtprozis.cs', 'class Hrtprozis {}')
            z.writestr(prefix + 'ai/Playfield.cs', 'class Playfield {}')
            z.writestr(prefix + 'data/CardDefs.xml', '<cards/>')
        report = stage(archive, root / 'out')
        assert report['imported_files'] == 4
        assert (root / 'out/src/Silverfish.Classic/Imported/ai/Playfield.cs').exists()


if __name__ == '__main__':
    test_synthetic_import()
    test_monolithic_playfield_import()
    print('PASS: archive hash, safe source selection, headless patching and quarantine')
