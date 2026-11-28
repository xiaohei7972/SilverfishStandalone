#!/usr/bin/env python3
"""Temporarily disable four Aggramar effect simulators while keeping old code commented.

This patch is idempotent. It never changes the original Hearthbuddy archive.
The empty implementations are NOT correct gameplay behavior.
"""
from pathlib import Path
import argparse
import json

PREFIX = Path('src/Silverfish.Classic/Imported/cards/1858-泰坦诸神')
SCRIPTS = ('Sim_TTN_092.cs', 'Sim_TTN_092t1.cs', 'Sim_TTN_092t2.cs', 'Sim_TTN_092t3.cs')
MARKER = '// SILVERFISH_STANDALONE_AGGRAMAR_DISABLED'


def disabled_source(filename: str, original: str | None) -> str:
    if filename not in SCRIPTS:
        raise ValueError(f'Not an approved Aggramar simulator: {filename}')
    classname = filename.removesuffix('.cs')
    note = (
        f'{MARKER}\n'
        f'// {classname}: effect deliberately disabled pending an independent simulator fix.\n'
        '// Do not interpret the empty SimTemplate as correct gameplay behavior.\n'
    )
    if original:
        note += '// BEGIN DISABLED ORIGINAL\n'
        note += ''.join('// ' + line + '\n' for line in original.splitlines())
        note += '// END DISABLED ORIGINAL\n'
    else:
        note += '// Original effect code is available in Hearthbuddy1.7.1(1).zip.\n'
    return (
        note + '\nnamespace HREngine.Bots\n{\n'
        f'    class {classname} : SimTemplate\n'
        '    {\n'
        '        // Intentionally no effect simulation (temporary).\n'
        '    }\n}\n'
    )


def disable(root: Path, *, allow_missing: bool = True) -> list[str]:
    disabled = []
    for filename in SCRIPTS:
        target = root / PREFIX / filename
        if not target.exists() and not allow_missing:
            continue
        original = target.read_text(encoding='utf-8-sig') if target.exists() else None
        if original is not None and MARKER in original:
            disabled.append(filename)
            continue
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_text(disabled_source(filename, original), encoding='utf-8')
        disabled.append(filename)
    manifest = root / 'classic-import-manifest.json'
    if manifest.exists():
        info = json.loads(manifest.read_text(encoding='utf-8'))
        quarantined = info.setdefault('quarantined', {})
        for filename in SCRIPTS:
            quarantined.pop('cards/1858-泰坦诸神/' + filename, None)
        info['disabled_card_simulators'] = [
            'cards/1858-泰坦诸神/' + name for name in SCRIPTS
        ]
        source = root / 'src/Silverfish.Classic/Imported'
        info['imported_files'] = sum(1 for path in source.rglob('*') if path.is_file())
        info['imported_cs_files'] = sum(1 for _ in source.rglob('*.cs'))
        manifest.write_text(json.dumps(info, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
    return disabled


def main() -> None:
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--root', type=Path, default=Path('.'))
    p.add_argument('--existing-only', action='store_true', help='Do not create missing stub files')
    args = p.parse_args()
    result = disable(args.root, allow_missing=not args.existing_only)
    print('DISABLED_AGGRAMAR_EFFECTS=' + ','.join(result))


if __name__ == '__main__':
    main()
