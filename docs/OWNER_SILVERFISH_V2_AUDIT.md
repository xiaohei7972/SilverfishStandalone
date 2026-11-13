# Owner Silverfish V2 source audit (research baseline only)

## Candidate repository

- Owner: `xiaohei7972`
- Repository: `xiaohei7972/Hearthbuddy_Silverfish` (private)
- Latest observed main commit: `8d9f3b6fa8cac6b2104499675e86c78f2dbcbda5` (2026-07-31).
- This is a candidate. It has NOT been confirmed as the owner's newly mentioned latest/final Hearthbuddy distribution.

## Verified from README and project documentation

The V2 tree lives in `Routines/DefaultRoutine/Silverfish_v2/`. Unlike historical Silverfish, it has its own entity/tag model, zone model, playfield, `ActionV2` and search orchestration. The original Silverfish simulator was intentionally removed from the current main path, according to its README.

The project's `LIVE_BRIDGE_PROGRESS.md` documents:

- `HearthbuddyStateSnapshotV2` and `HearthbuddyStateSnapshotImporterV2` that convert an explicit snapshot into V2 `PlayfieldV2`.
- `HearthbuddyStateImporterV2` that acquires live state through old Hearthbuddy/Triton APIs.
- `HearthbuddyActionExecutorV2` that maps V2 actions to old client operations.
- `SilverfishV2LiveImportDiagnostics`, `SilverfishV2MultiStepDiagnostics`, and action/choice diagnostics.
- The ability to run some search and action tests on synthetic scenarios, **not evidence of production game compatibility**.

The project's `DESIGN.md` says V2 deliberately does **not** use historical `Hrtprozis`, `Minion`, `Handmanager`, and old `Ai.Instance` as its main model.

## Architectural implication

**If this V2 implementation is part of the owner's confirmed latest source, prefer extracting the existing V2 independent core rather than mapping every snapshot through historical `Hrtprozis`.** The previous Hrtprozis adapter proposal applies only if the chosen source is the historical engine.

Target architecture:

    Authenticated StandaloneGameMod state.snapshot
                       |
                schema validator
                       |
                  V2 importer
                       |
              standalone PlayfieldV2
                       |
                standalone AiV2
                       |
              versioned BotDecision
                       |
              offline CLI / future UI

Preserve V2 internal model semantics. Remove the in-game `HearthbuddyStateImporterV2`, `HearthbuddyActionExecutorV2` and all Triton references from the core. Expose these only as separate optional adapters.

## Proposed port order

1. Pin source version once latest/final baseline is supplied.
2. Inventory `Silverfish_v2` files and all third-party license notices.
3. Build `Silverfish.V2.Core` independent of `DefaultRoutine` using only authorized sources.
4. Port the existing V2 offline smoke harness and ensure tests pass *before* changing rule logic.
5. Adapt `HearthbuddyStateSnapshotImporterV2` input to an external, versioned snapshot model.
6. Use the already existing authenticated local `state.snapshot` transport; do not pass Triton objects across the boundary.
7. Return recommended actions in a source/target entity-ID-safe model tied to `matchId` + `sequence`.
8. Build UI and decision diagnostics around this stable interface.

No V2 source has been copied into SilverfishStandalone yet. All this document's statements about the source are from the owner's connected repository documentation.
