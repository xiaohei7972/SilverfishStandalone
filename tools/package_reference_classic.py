#!/usr/bin/env python3
"""Pack classic Silverfish from an older reference checkout for CI diagnostics.
The packed reference MUST NOT be mistaken for the owner-provided 2026-11-05 ZIP.
"""
import argparse
from pathlib import Path
from zipfile import ZipFile, ZIP_DEFLATED

ALLOWED = {".cs", ".xml", ".json", ".txt"}


def package(source_repo: Path, output: Path) -> tuple[int, int]:
    source = (source_repo / "Routines" / "DefaultRoutine" / "Silverfish").resolve()
    if not source.is_dir() or not (source / "ai" / "Ai.cs").is_file():
        raise ValueError(f"Classic Silverfish source path is missing: {source}")

    output = output.resolve()
    output.parent.mkdir(parents=True, exist_ok=True)
    count, total = 0, 0
    with ZipFile(output, "w", compression=ZIP_DEFLATED, compresslevel=6) as archive:
        for file in sorted(source.rglob("*")):
            if not file.is_file() or file.is_symlink():
                continue
            if file.suffix.lower() not in ALLOWED:
                continue
            relative = file.relative_to(source).as_posix()
            if "silverfish_v2" in relative.lower():
                continue
            total += file.stat().st_size
            if total > 256 * 1024 * 1024:
                raise ValueError("Classic source exceeds expected maximum size")
            archive.write(file, "reference/Routines/DefaultRoutine/Silverfish/" + relative)
            count += 1

    if count < 10:
        raise ValueError("Classic reference inventory is unexpectedly small")
    return count, total


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("source_repo", type=Path)
    parser.add_argument("output_archive", type=Path)
    args = parser.parse_args()
    count, total = package(args.source_repo, args.output_archive)
    print(f"Prepared {count} classic files ({total:,} uncompressed bytes)")
    print("SOURCE MODE: older diagnostic reference, NOT authoritative 2026-11-05 release")


if __name__ == "__main__":
    main()
