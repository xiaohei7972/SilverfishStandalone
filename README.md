# SilverfishStandalone

A standalone .NET 8 foundation for Hearthstone game-state analysis and the future extraction of the historical Silverfish decision engine.

**Current state (Phase 1 foundation):** This repository already contains a runnable offline snapshot -> decision -> JSON pipeline, deterministic tests, a baseline strategy for validating the pipeline, and a separate Libmem.NET process-probe integration. **The baseline strategy is NOT the original Silverfish AI.** The historical algorithm has not yet been ported.

## Goals

- Remove dependencies on Hearthbuddy.exe, Triton, GreyMagic, and their runtime APIs.
- Keep strategy computation separate from memory access, game transport, and the UI.
- Import the historical Silverfish simulator only after a file-by-file provenance, license, and dependency review.
- Reuse CardResearchLab/StandaloneGameMod's state.snapshot stream via a future schema adapter.
- Use Libmem.NET for Windows process inspection and a separately tested read-only game adapter.
- Keep all game operations outside of the strategy engine; the first milestone is decision analysis and replay only.

## Requirements

.NET 8 SDK. No Hearthbuddy executable, game DLL, or running game is required to run the offline demo and tests. The optional Libmem.NET integration is Windows x64 only.

## Run the offline demo

    dotnet run --project src/Silverfish.Cli/Silverfish.Cli.csproj -- samples/lethal.json

The CLI prints a structured decision and candidate ranking. It never interacts with a live game.

## Run unit tests

    dotnet test tests/Silverfish.Tests/Silverfish.Tests.csproj -c Release

## Optional Libmem.NET integration (Windows x64)

    dotnet build src/Silverfish.Integrations.Libmem/Silverfish.Integrations.Libmem.csproj -c Release

This component currently inspects an explicitly selected process (PID, architecture, modules). It does not read Hearthstone gameplay data or perform injection. It is intentionally isolated from the offline strategy pipeline.

## Code map

- src/Silverfish.Contracts — versioned input/output snapshots and action identities.
- src/Silverfish.Core — validation, strategy interface, deterministic baseline evaluator.
- src/Silverfish.Cli — offline JSON runner (proof of a running pipeline).
- src/Silverfish.Integrations.Libmem — optional Windows x64 process-probe adapter using Libmem.NET 2.5.0.
- tests/Silverfish.Tests — regression and validation tests.
- docs/ARCHITECTURE.md — strict dependency boundaries and integration steps.
- docs/SILVERFISH_PORT.md — planned migration of the actual Silverfish engine.
- docs/THIRD_PARTY.md — attribution and license-review checklist.

The legal-action candidates in the demo are fixture data, not reconstructed from a live game. A trustworthy game-specific action generator and versioned snapshot adapter are prerequisites to using live data.

This repository deliberately does not include copyrighted Hearthbuddy/Triton assemblies, original Silverfish source, anti-cheat bypass features, or gameplay automation.

## Classic Silverfish (owner's 2026-11-05 archive)

The selected source archive is pinned by SHA-256 and can be staged without importing the old host. See [latest classic source import](docs/CLASSIC_2026_11_05_IMPORT.md).

Run `python tools/test_import_classic.py`, then `python tools/import_classic.py <archive.zip> --dest .` (pass the pinned `--sha256` from the guide). The new `src/Silverfish.Classic/` project and smoke runner are only buildable after source staging. **The actual classic AI port is not yet build-verified or connected to GameSnapshot.**
