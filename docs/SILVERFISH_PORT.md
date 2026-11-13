# Silverfish extraction plan

## Source candidates (review required)

- Historical reference: https://github.com/ChuckHearthstone/SilverFish
- Newer community implementation: https://github.com/lijhnihaoa/HB
- Existing internal transport: https://github.com/CardResearchLab/StandaloneGameMod

In the community HB source, the first files to study are:

- Routines/DefaultRoutine/DefaultRoutine.cs — Hearthbuddy/Triton-facing IRoutine; NOT a drop-in library.
- Routines/DefaultRoutine/Silverfish/ai/Ai.cs — search orchestration and bestmove; contains UI coupling.
- Routines/DefaultRoutine/Silverfish/ai/Hrtprozis.cs — aggregated state expected by the simulator.
- Routines/DefaultRoutine/Silverfish/Playfield/ — game simulation and rule implementation.
- CardDB, Handmanager, PenalityManager, Behavior and related simulator helpers.

These are research references, not yet imported or independently compiled here.

## Ordered porting work

1. Inventory files, upstream origins, licenses and transitive dependencies. Check actual licensing per file.
2. Extract dependency graph. Categorize: pure simulation, data model, file-backed data, log/settings, Triton/IRoutine, and UI.
3. Build an isolated SilverfishPort project with only legally reusable source and no Hearthbuddy executable or Triton reference.
4. Replace global game state capture with an explicit per-request board state. Initially support only a narrow, documented card/rule subset.
5. Introduce a snapshot-to-Hrtprozis translator, with missing-field diagnostics rather than unsafe default values.
6. Replace UI/log/singleton coupling with injected interfaces where needed, while retaining deterministic regression tests.
7. Compare the port against known offline test fixtures from the upstream engine. Validate every newly supported mechanic.
8. Replace BaselineStrategyEngine behind IStrategyEngine only when independent calculation tests pass.

## First useful completion criterion

    JSON fixture -> validated GameSnapshot -> independent Silverfish simulation -> BotDecision

The output must be possible without Hearthbuddy.exe, Triton, GreyMagic, a running game, or a UI. This criterion is NOT yet met by the baseline implementation: the baseline is an intentionally limited architectural smoke test.

## Version notes

Modern Hearthstone card rules, triggers, choice modes, secrets, buffs and special mechanics evolve continuously. The existence of a card in an upstream database is not evidence that its simulation is correct.
