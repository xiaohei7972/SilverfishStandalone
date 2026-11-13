# Source baseline / 迁移基准

## Owner's selection

The project owner asked to prioritize their **latest Hearthbuddy release**, possibly the final one, while extracting the **classic Silverfish AI** (`Ai`, `Hrtprozis`, `Playfield`, `CardDB` and associated simulation code). Do not use alternative AI rewrites as the migration baseline.

**Status: source not yet received in this chat.** No file/archive or exact commit/tag corresponding to that latest update has been provided in a form that can be inspected. Do not assume it equals either public reference listed below. The initial repository foundation intentionally contains no imported code from either.

## Baseline acceptance checklist

When the authoritative archive/repository is supplied:

1. Record source URL or archive filename, version/date and commit SHA when available.
2. For an archive, record SHA-256 and file inventory; keep the input read-only.
3. Identify original license/attribution for each subtree and binary dependency, concentrating on the classic Silverfish strategy implementation.
4. Find strategy entry points, AI simulator, state collector, action adapter, UI and plugins.
5. Produce a source-to-target mapping table and dependency graph.
6. Set a known-good offline regression corpus before modifying the AI.
7. Import only authorized, independently buildable portions to a dedicated port project.

## Owner's existing related repositories (candidate references, not yet confirmed baseline)

A search of the owner's connected GitHub account found these additional projects:

- `xiaohei7972/Hearthbuddy_Refactor` — latest observed commit 2026-09-04 (refactor/build work).
- `xiaohei7972/Hearthbuddy_Origin` — latest observed commit 2026-05-05 (original host refactor).
- `xiaohei7972/Hearthbuddy_backed` — last observed commit 2025-11-12.
- `xiaohei7972/Hearthbuddy_SimTemplate` — last observed commit 2025-10-02.

These dates are repository commit dates, **not proof that any repository is the owner's mentioned newest Hearthbuddy release**. An authoritative exact source choice is still required before code migration.

## Non-authoritative research references

- https://github.com/lijhnihaoa/HB — useful inventory of a community-maintained Silverfish code tree.
- https://github.com/ChuckHearthstone/SilverFish — historical implementation.
- https://github.com/CardResearchLab/StandaloneGameMod — independent runtime transport and state projection.

The current code is a verified starting **architecture**, not an assertion that the latest upstream Silverfish engine has already been extracted.
