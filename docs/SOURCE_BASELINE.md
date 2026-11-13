# Source baseline / 源码基准

## Confirmed owner-supplied source

The authoritative baseline is the ZIP uploaded by the repository owner:

- Archive: `Hearthbuddy1.7.1(1).zip`
- ZIP root: `炉石兄弟-2026.11.05/`
- SHA-256: `1879f784ed5b6854d53e148fc444d0f65bd8afbd52ccc1b3d8cf56e23efb5071`
- AI path: `Routines/DefaultRoutine/Silverfish/`
- Target: classic Silverfish (`Ai`, `Hrtprozis`, `Playfield`, `Handmanager`, `CardDB`, `Movegenerator`), **not** any replacement/rewrite of the algorithm.

Actual ZIP contents were inspected in the working environment. The original archive has 19,405 entries; classic Silverfish has 19,141 C# files. The `cards/` subtree alone has 19,058 C# simulations, and the card definitions XML is ~23 MB. There is no Silverfish_v2 source directory.

## Work completed

`tools/import_classic.py` reproduced the owner-supplied source selection:

- 19,166 source/data files staged locally (19,135 C#).
- 27 files have relevant import-time API/using/path replacements.
- Six old client-runtime dependent files are quarantined and listed in `classic-import-manifest.json`.
- No Hearthbuddy executable, Triton binaries, GreyMagic binaries, or classic Silverfish v2 code are included in the staged source package.
- An independent `src/Silverfish.Classic/` project, compatibility boundary and smoke entry point have been created.

See `docs/CLASSIC_2026_11_05_IMPORT.md` for exact commands and evidence.

## Outstanding gates

1. Build the newly staged source under .NET 8 and resolve real compiler errors.
2. Run original `Ai.autoTester` against a recorded `BoardTester` fixture; compare generated decisions.
3. Translate authoritative game snapshots to the full classic Hrtprozis + Handmanager data set.
4. Return sequence-bound recommendations and integrate the new UI.

The raw `Imported/` tree is ignored by Git by default to avoid accidentally publishing almost 20,000 new files in the public repository before local validation. The current GitHub branch contains the reproducible importer and project scaffolding, but **the original classic AI has not yet passed independent compilation or regression tests**.

## Other references

Older GitHub forks and the owner's earlier Hearthbuddy code can be consulted for behavior comparison; they do not supersede the SHA-pinned 2026-11-05 archive.
