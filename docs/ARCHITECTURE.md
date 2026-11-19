# Architecture and boundaries

## Independent simulation engine

`Silverfish.Domain.GameState` stores stable player and entity IDs. Each player owns ordered zone indexes. The entity table stores the card reference, owner, controller and character state. An inverse index provides zone lookup. A structural edit replaces the related indexes together. Changing control preserves the entity ID and entrance order.

`Silverfish.Simulation.GameSimulator` generates supported legal actions and resolves one action into a new immutable state and an ordered event trace. Card definitions compose typed effects. The existing classic snapshots and the classic engine provide historical fixtures for comparison.

`Silverfish.Core.SimulationTurnPlanner` searches branches up to a depth and transition budget. It applies each candidate through the new simulator and evaluates the resulting state through a separate scoring function. The `plan` command runs it after the fixture's scripted actions.

The replay command starts from a fully specified offline scenario. Its rules cover turn start and draw, fatigue, burning cards, basic minion and spell play, basic combat, control transfer, and deathrattle waves. Card definitions can subscribe to turn boundaries, summoning, spell resolution, and minion death. A queue records eligible listeners and runs nested events before returning to the parent listener. Minion-death listeners share the wave's eligibility snapshot and act in entry order alongside deathrattles. The normal single-token deathrattle uses the slot recorded when the minion was removed.

`samples/simulation-triggers.json` exercises spell and death timing through the normal replay command. The replay includes the triggering entity, event subject when present, and checkpoint, so differences can be identified at the first differing transition. The current rules catalogue still needs damage and draw subscriptions, card-specific timing exceptions, advanced summon placements, enchantment layers, and hidden information modelling.

## Components

    Offline JSON or StandaloneGameMod state.snapshot
                      |
                      v
       Snapshot adapter (not yet implemented)
                      |
                      v
         Contracts.GameSnapshot (v1)
                      |
             SnapshotValidator
                      |
                      v
          IStrategyEngine.DecideAsync
             /                    \
    BaselineStrategyEngine    Future SilverfishPort
             \                    /
                      v
              Contracts.BotDecision
                      |
              CLI or future UI

The strategy interface has no reference to Libmem.NET, Unity, Mono, Hearthbuddy, Triton or the UI. The source and the algorithm can evolve separately.

## Present milestone

The CLI consumes offline JSON fixtures with explicitly enumerated candidate actions. BaselineStrategyEngine ranks them deterministically. This is a working protocol/proof-of-concept, not Silverfish's original game simulator.

The offline contract is not identical to CardResearchLab/StandaloneGameMod's state.snapshot payload. A separate, tested versioned translation layer is required before consuming its live data. Do not deserialize state.snapshot directly as Contracts.GameSnapshot.

The optional Libmem.NET project uses 2.5.0 and explicit Windows x64. At present, it only probes metadata of a PID selected by the caller; it does not reconstruct gameplay entities. Its existence must not be confused with a working game-state reader.

## Non-negotiable runtime invariants

1. Input data must be labeled with schema version, match ID and monotonic sequence.
2. Invalid or unsupported snapshots must fail closed, not silently generate guesses.
3. UI and strategy components never hold process handles.
4. Never infer an action is legal from baseline heuristics. Action candidates must originate from a trusted, separately verified provider.
5. Reject decisions if their match ID or sequence no longer matches the latest snapshot.
6. No gameplay execution is implemented here. Any future executor needs separate authorization, state revalidation, and an offline test harness.
7. Sensitive tokens and secrets must never enter game snapshots, logs, or the UI's decision telemetry.

## Integration design

The existing StandaloneGameMod has an authenticated localhost TCP envelope (protocolVersion=2) with state.snapshot event messages. A consumer should re-use that authenticated transport, select the correct process instance and session, verify sequence continuity, map game state into the model and record regression fixtures. Do not add game-specific or bot-specific dependencies to Libmem.NET.

The historical Silverfish AI uses its own Hrtprozis/Playfield structures. A mapper should transform verified GameSnapshot data into that model, then map the engine output back to CandidateAction identity. This must never be done by directly passing game process objects into Silverfish.Core.
