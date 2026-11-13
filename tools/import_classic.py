#!/usr/bin/env python3
"""Prepare a headless classic-Silverfish source tree from an owner's archive.

The tool only imports allow-listed text under Routines/DefaultRoutine/Silverfish.
It preserves the game simulator/card scripts; it does not copy the Hearthbuddy host.
Some old client-facing files are quarantined until their mechanics are ported.
"""
import argparse
import hashlib
import json
from pathlib import Path, PurePosixPath
import re
import zipfile

MARKER = 'Routines/DefaultRoutine/Silverfish/'
EXCLUDE = {
    'Helpers/Extensions.cs': 'Hearthbuddy input/HSCard API adapter',
    'ai/Zones.cs': 'Triton Game.Mapping Card wrapper, not used by the simulator',
    'cards/1858-泰坦诸神/Sim_TTN_092.cs': 'calls live GameState in a card simulator',
    'cards/1858-泰坦诸神/Sim_TTN_092t1.cs': 'calls live GameState in a card simulator',
    'cards/1858-泰坦诸神/Sim_TTN_092t2.cs': 'calls live GameState in a card simulator',
    'cards/1858-泰坦诸神/Sim_TTN_092t3.cs': 'calls live GameState in a card simulator',
}
REMOVE_USING = re.compile(r'^\s*using\s+(?:(?:Triton|IronPython|Microsoft\.Scripting|log4net|System\.Windows)(?:\.[\w.]+)?|Logger\s*=\s*Triton\.[\w.]+)\s*;\s*$', re.M)
REPLACE = {
    'TritonHs.GameState.GetEntity(': 'DetachedEntityRegistry.GetEntity(',
    'Silverfish.Instance.updateCThunInfo(': 'Hrtprozis.Instance.updateCThunInfo(',
    'Silverfish.Instance.BehaviorPath': 'ClassicBehaviorPaths.Paths',
    'DefaultRoutineSettings.Instance.MaxWide': 'Settings.Instance.maxwide',
    'Path.Combine(AppContext.BaseDirectory, "Routines", "DefaultRoutine", "Silverfish", "Hsreplay")': 'Path.Combine(AppContext.BaseDirectory, "Hsreplay")',
    'foreach (string file in Directory.GetFiles(directoryPath, "*.json"))': 'foreach (string file in Directory.Exists(directoryPath) ? Directory.GetFiles(directoryPath, "*.json") : Array.Empty<string>())',
}
MAX_TOTAL_BYTES = 256 * 1024 * 1024


def stage(archive: Path, destination: Path, expected_sha256: str | None = None) -> dict:
    with archive.open('rb') as source:
        archive_digest = hashlib.file_digest(source, 'sha256').hexdigest()
    if expected_sha256 and archive_digest.lower() != expected_sha256.lower():
        raise ValueError('Source archive SHA-256 does not match the selected baseline')

    output = destination / 'src' / 'Silverfish.Classic' / 'Imported'
    output.mkdir(parents=True, exist_ok=True)
    copied, transformed, skipped, total = [], [], {}, 0
    with zipfile.ZipFile(archive) as z:
        for entry in z.infolist():
            path = PurePosixPath(entry.filename)
            if entry.is_dir() or path.is_absolute() or '..' in path.parts or '\\' in entry.filename:
                continue
            key = entry.filename
            if MARKER not in key:
                continue
            relative = key.split(MARKER, 1)[1]
            if not relative or relative.startswith('/') or 'silverfish_v2' in relative.lower():
                continue
            file = PurePosixPath(relative)
            if file.suffix.lower() not in {'.cs', '.xml', '.json', '.txt'}:
                continue
            if relative in EXCLUDE:
                skipped[relative] = EXCLUDE[relative]
                continue
            total += entry.file_size
            if total > MAX_TOTAL_BYTES:
                raise ValueError('Uncompressed archive data is larger than expected')
            data = z.read(entry)
            if relative.endswith('.cs'):
                content = data.decode('utf-8-sig')
                old = content
                content = REMOVE_USING.sub('', content)
                for before, after in REPLACE.items():
                    content = content.replace(before, after)
                data = content.encode('utf-8')
                if content != old:
                    transformed.append(relative)
            target = output.joinpath(*file.parts)
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_bytes(data)
            copied.append(relative)

    if 'ai/Ai.cs' not in copied or 'ai/Hrtprozis.cs' not in copied:
        raise ValueError('Archive does not contain the classic Silverfish AI core')
    if not any(p in copied for p in ('ai/Playfield/Playfield.Ctors.cs', 'ai/Playfield.cs')) or 'data/CardDefs.xml' not in copied:
        raise ValueError('Archive lacks required simulation or card data files')

    report = {
        'source_archive': archive.name,
        'sha256': archive_digest,
        'imported_files': len(copied),
        'imported_cs_files': sum(x.endswith('.cs') for x in copied),
        'transformed_files': len(transformed),
        'quarantined': skipped,
        'root': 'src/Silverfish.Classic/Imported',
        'note': 'Source staged; requires dotnet build and offline regression verification.'
    }
    (destination / 'classic-import-manifest.json').write_text(
        json.dumps(report, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
    return report


def main() -> None:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument('archive', type=Path)
    ap.add_argument('--dest', type=Path, default=Path('.'))
    ap.add_argument('--sha256', help='Optional pinned source archive SHA-256')
    a = ap.parse_args()
    print(json.dumps(stage(a.archive, a.dest, a.sha256), ensure_ascii=False, indent=2))


if __name__ == '__main__':
    main()
