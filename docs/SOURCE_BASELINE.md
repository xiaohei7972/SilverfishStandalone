# Source baseline / 迁移基准

## Owner's selection

The project owner asked to prioritize their **latest Hearthbuddy / Silverfish update**, possibly the final one, rather than treating older online forks as authoritative.

**Status: source not yet received in this chat.** No file/archive or exact commit/tag corresponding to that latest update has been provided in a form that can be inspected. Do not assume it equals either public reference listed below. The initial repository foundation intentionally contains no imported code from either.

## Baseline acceptance checklist

When the authoritative archive/repository is supplied:

1. Record source URL or archive filename, version/date and commit SHA when available.
2. For an archive, record SHA-256 and file inventory; keep the input read-only.
3. Identify original license/attribution for each subtree and binary dependency.
4. Find strategy entry points, AI simulator, state collector, action adapter, UI and plugins.
5. Produce a source-to-target mapping table and dependency graph.
6. Set a known-good offline regression corpus before modifying the AI.
7. Import only authorized, independently buildable portions to a dedicated port project.

## Non-authoritative research references

- https://github.com/lijhnihaoa/HB — useful inventory of a community-maintained Silverfish code tree.
- https://github.com/ChuckHearthstone/SilverFish — historical implementation.
- https://github.com/CardResearchLab/StandaloneGameMod — independent runtime transport and state projection.

The current code is a verified starting **architecture**, not an assertion that the latest upstream Silverfish engine has already been extracted.
